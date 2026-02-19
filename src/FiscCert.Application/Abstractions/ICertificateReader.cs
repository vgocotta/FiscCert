using FiscCert.Application.DTO;

namespace FiscCert.Application.Abstractions;

public interface ICertificateReader
{
    Task<CertificateMetadata> ReadCertificateAsync(Stream fileStream, string password, CancellationToken cancellationToken = default);
}
