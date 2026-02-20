using FiscCert.Domain;
using FiscCert.Infrastructure.Persistence;
using FiscCert.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscCert.Infrastructure.Tests.Persistence;

public class CertificateRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CertificateRepository _sut;

    public CertificateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _sut = new CertificateRepository(_dbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldSaveCertificateToDatabase()
    {
        // Arrange
        var certificate = new Certificate(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "tenant/123.pfx",
            "EMPRESA TESTE",
            "12345678000199",
            "SERIAL123",
            DateTime.UtcNow.AddYears(1),
            null);

        // Act
        await _sut.AddAsync(certificate);

        // Assert
        var savedCert = await _dbContext.Certificates.FirstOrDefaultAsync(c => c.Id == certificate.Id);
        Assert.NotNull(savedCert);
        Assert.Equal("12345678000199", savedCert.FederalInscription);
    }
}