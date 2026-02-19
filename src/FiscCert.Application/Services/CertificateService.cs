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

    public CertificateService(ICertificateRepository repository, IStorageService storageService, ICertificateReader certificateReader)
    {
        _repository = repository;
        _storageService = storageService;
        _certificateReader = certificateReader;
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

        Certificate certificate = new(input.TenantId, blobName, metadata.OwnerName, metadata.FederalInscription, metadata.SerialNumber, metadata.ExpirationDate);

        if (input.FileStream.CanSeek)
        {
            input.FileStream.Position = 0;
        }

        await _storageService.UploadAsync(input.FileStream, blobName, cancellationToken);

        await _repository.AddAsync(certificate, cancellationToken);

        return certificate.Id;
    }
}
