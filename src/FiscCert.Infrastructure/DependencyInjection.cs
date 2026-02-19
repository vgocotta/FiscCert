using FiscCert.Application.Abstractions;
using FiscCert.Infrastructure.Cryptography;
using FiscCert.Infrastructure.Persistence;
using FiscCert.Infrastructure.Persistence.Repositories;
using FiscCert.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiscCert.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<ICertificateRepository, CertificateRepository>();

        // Register other infrastructure services (e.g., storage, certificate reader)
        services.AddScoped<IStorageService, LocalDiskStorageService>();
        services.AddScoped<ICertificateReader, CertificateReader>();

        return services;
    }
}
