using FiscCert.Domain;

namespace FiscCert.Application.Abstractions;

public interface ICertificateRepository
{
    Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task DeleteAsync(Certificate certificate, CancellationToken cancellationToken = default);

    Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Certificate>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
