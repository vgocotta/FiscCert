using FiscCert.Application.DTO;

namespace FiscCert.Application.Abstractions;

public interface ICertificateService
{
    Task<Guid> UploadCertificateAsync(UploadCertificateDto input, CancellationToken cancellationToken = default);
}