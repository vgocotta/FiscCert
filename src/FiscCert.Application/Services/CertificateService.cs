using FiscCert.Application.Abstractions;
using FiscCert.Application.Constants;
using FiscCert.Application.DTO;
using FiscCert.Application.Exceptions;
using FiscCert.Domain;
using System.Security.Cryptography;

namespace FiscCert.Application.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;
    private readonly IStorageService _storageService;
    private readonly ICertificateReader _certificateReader;
    private readonly IEncryptionService _encryptionService;

    public CertificateService(ICertificateRepository repository, IStorageService storageService, ICertificateReader certificateReader, IEncryptionService encryptionService)
    {
        _repository = repository;
        _storageService = storageService;
        _certificateReader = certificateReader;
        _encryptionService = encryptionService;
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var certificates = await _repository.GetAllByTenantAsync(tenantId, cancellationToken);

        var dtos = certificates.Select(c => new CertificateDto
        (
            c.Id,
            c.OwnerName,
            c.FederalInscription,
            c.ExpirationDate,
            c.IsRevoked
        ));

        return dtos;
    }

    public async Task<Guid> UploadCertificateAsync(UploadCertificateDto input, CancellationToken cancellationToken = default)
    {
        CertificateMetadata metadata;

        try
        {
            metadata = await _certificateReader.ReadCertificateAsync(input.FileStream, input.Password, cancellationToken);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidCertificatePasswordException(ErrorMessages.InvalidCertificatePassword, ex);
        }
        var certificateId = Guid.NewGuid();
        var blobName = $"{input.TenantId}/{certificateId}.pfx";

        var encryptedPassword = _encryptionService.Encrypt(input.Password);

        Certificate certificate = new(
            certificateId,
            input.TenantId,
            blobName,
            metadata.OwnerName,
            metadata.FederalInscription,
            metadata.SerialNumber,
            metadata.ExpirationDate,
            encryptedPassword,
            input.EconomicGroupId);

        if (input.FileStream.CanSeek)
        {
            input.FileStream.Position = 0;
        }

        await _storageService.UploadAsync(input.FileStream, blobName, cancellationToken);

        await _repository.AddAsync(certificate, cancellationToken);

        return certificate.Id;
    }
}
