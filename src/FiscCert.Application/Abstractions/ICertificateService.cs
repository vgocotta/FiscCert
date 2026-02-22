using FiscCert.Application.DTO;

namespace FiscCert.Application.Abstractions;

public interface ICertificateService
{
    Task<Guid> UploadCertificateAsync(UploadCertificateDto input, CancellationToken cancellationToken = default);
    Task<IEnumerable<CertificateDto>> GetCertificatesAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<List<CertificateDto>> GetCertificatesAsync(Guid tenantId, string? searchTerm, string? orderBy, CancellationToken cancellationToken);
    Task<string> GetCertificatePasswordAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task RevokeCertificateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
    Task DeleteCertificateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);

    Task<(byte[] Content, string FileName)> DownloadCertificateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
}