using FiscCert.Application.Abstractions;
using FiscCert.Domain;
using Microsoft.EntityFrameworkCore;

namespace FiscCert.Infrastructure.Persistence.Repositories;

public class CertificateRepository : ICertificateRepository
{

    private readonly ApplicationDbContext _dbContext;

    public CertificateRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        await _dbContext.Certificates.AddAsync(certificate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        _dbContext.Certificates.Update(certificate);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        _dbContext.Certificates.Remove(certificate);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Certificate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Certificates
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Certificate>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Certificates
            .AsNoTracking()
            .Where(c => c.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }
}
