using FiscCert.Domain;
using Xunit;

namespace FiscCert.UnitTests;

public class CertificateTests
{

    [Fact]
    public void Constructor_ShouldCreateCertificate_WhenAllParametersAreValid()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var economicGroupId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(365);

        // Act
        var cert = new Certificate(
            Guid.NewGuid(),
            tenantId,
            "tenants/123/certs/file.pfx", 
            "Test Company",               
            "12.345.678/0001-90",         
            "SERIE123456",                
            expirationDate,
            "password",
            economicGroupId
        );

        // Assert
        Assert.NotNull(cert);
        Assert.Equal(tenantId, cert.TenantId);
        Assert.Equal(economicGroupId, cert.EconomicGroupId);
        Assert.Equal("SERIE123456", cert.SerialNumber);
        Assert.False(cert.IsRevoked);
    }

    [Fact]
    public void Constructor_ShouldCreateCertificate_WhenEconomicGroupIsNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var cert = new Certificate(
            Guid.NewGuid(),
            tenantId,
            "path/to/file",
            "Test Company",
            "CPF123",
            "SERIE123",
            DateTime.UtcNow,
            "password"
        );

        // Assert
        Assert.Null(cert.EconomicGroupId);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenBlobPathIsEmpty()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Certificate(
            Guid.NewGuid(),
            tenantId,
            "",
            "Company",
            "CPF",
            "SERIALNUMBER",
            DateTime.UtcNow,
            "password"
        ));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenSerialNumberIsEmpty()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Certificate(
            Guid.NewGuid(),
            tenantId,
            "path/file",
            "Empresa",
            "CPF",
            "",
            DateTime.UtcNow,
            "password"
        ));
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenDateIsFutureAndNotRevoked()
    {
        // Arrange
        DateTime referenceDate = new(2026, 1, 1);
        DateTime expirationDate = new(2026, 1, 2);
        var cert = new Certificate(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            "path", 
            "Name", 
            "Doc", 
            "Serial", 
            expirationDate,
            "password");

        // Act
        var isValid = cert.IsValid(referenceDate);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenDateIsPast()
    {
        // Arrange
        DateTime referenceDate = new(2026, 1, 3);
        DateTime expirationDate = new(2026, 1, 2);

        var cert = new Certificate(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            "path", 
            "Name", 
            "Doc", 
            "Serial", 
            expirationDate,
            "password");

        // Act
        var isValid = cert.IsValid(referenceDate);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenRevoked()
    {
        // Arrange
        DateTime referenceDate = new(2026, 1, 1);
        DateTime expirationDate = new(2026, 1, 2);
        var cert = new Certificate(Guid.NewGuid(), Guid.NewGuid(), "path", "Name", "Doc", "Serial", expirationDate, "password");

        // Act
        cert.Revoke(); 
        var isValid = cert.IsValid(referenceDate);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void AssignToEconomicGroup_ShouldUpdateGroupId()
    {
        // Arrange
        var cert = new Certificate(Guid.NewGuid(), Guid.NewGuid(), "path", "Name", "Doc", "Serial", DateTime.UtcNow, "password");
        var newGroupId = Guid.NewGuid();

        // Act
        cert.AssignToEconomicGroup(newGroupId);

        // Assert
        Assert.Equal(newGroupId, cert.EconomicGroupId);
    }

    [Fact]
    public void RemoveFromEconomicGroup_ShouldSetGroupIdToNull()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var cert = new Certificate(Guid.NewGuid(), Guid.NewGuid(), "path", "Name", "Doc", "Serial", DateTime.UtcNow, "password", groupId);

        // Act
        cert.RemoveFromEconomicGroup();

        // Assert
        Assert.Null(cert.EconomicGroupId);
    }

    [Fact]
    public void AddRepresentedCompany_ShouldAddToCollection()
    {
        // Arrange
        var cert = new Certificate(Guid.NewGuid(), Guid.NewGuid(), "path", "Name", "Doc", "Serial", DateTime.UtcNow, "password");

        // Act
        cert.AddRepresentedCompany("Filial 01", "CNPJ-Filial");

        // Assert
        Assert.Single(cert.RepresentedCompanies);
        Assert.Contains(cert.RepresentedCompanies, c => c.CompanyName == "Filial 01");
    }
}