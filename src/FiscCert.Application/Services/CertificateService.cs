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

    public async Task<string> GetCertificatePasswordAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var certificate = await _repository.GetByIdAsync(id, cancellationToken);

        if (certificate == null || certificate.TenantId != tenantId)
        {
            throw new Exception("Certificado não encontrado ou acesso negado.");
        }

        return _encryptionService.Decrypt(certificate.EncryptedPassword);
    }

    public async Task RevokeCertificateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var certificate = await _repository.GetByIdAsync(id, cancellationToken);

        if (certificate == null || certificate.TenantId != tenantId)
        {
            throw new Exception("Certificado não encontrado ou acesso negado.");
        }

        certificate.Revoke();

        await _repository.UpdateAsync(certificate, cancellationToken);
    }

    public async Task DeleteCertificateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var certificate = await _repository.GetByIdAsync(id, cancellationToken);

        if (certificate == null || certificate.TenantId != tenantId)
        {
            throw new Exception("Certificado não encontrado ou acesso negado.");
        }

        await _storageService.DeleteFileAsync(certificate.BlobPath, cancellationToken);

        await _repository.DeleteAsync(certificate, cancellationToken);
    }

    public async Task<(byte[] Content, string FileName)> DownloadCertificateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        var certificate = await _repository.GetByIdAsync(id, cancellationToken);

        if (certificate == null || certificate.TenantId != tenantId)
        {
            throw new Exception("Certificado não encontrado ou acesso negado.");
        }

        var content = await _storageService.GetFileAsync(certificate.BlobPath, cancellationToken);

        // Remove espaços e caracteres especiais do nome da empresa para montar um nome de arquivo limpo
        var safeOwnerName = string.Join("_", certificate.OwnerName.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"{safeOwnerName}.pfx";

        return (content, fileName);
    }
}
