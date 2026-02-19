using FiscCert.Domain;
using Microsoft.EntityFrameworkCore;

namespace FiscCert.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Certificate> Certificates => Set<Certificate>();
    // public DbSet<EconomicGroup> EconomicGroups => Set<EconomicGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
