using FiscCert.Domain;

namespace FiscCert.UnitTest;

public class CertificateTests
{

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenDateIsFutureAndNotRevoked()
    {
        // Arrange
        DateTime referenceDate = new(2026, 1, 1);
        DateTime expirationDate = new(2026, 1, 2);
        Certificate certificate = new("Test Owner", "12345678000190", expirationDate);

        // Act
        bool isValid = certificate.IsValid(referenceDate);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenDateIsPast()
    {
        // Arrange
        DateTime referenceDate = new(2026, 1, 3);
        DateTime expirationDate = new(2026, 1, 2);
        Certificate certificate = new("Test Owner", "12345678000190", expirationDate);
     
        // Act
        bool isValid = certificate.IsValid(referenceDate);
        
        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenCertificateIsRevoked()
    {
        // Arrange
        DateTime referenceDate = new(2026, 1, 1);
        DateTime expirationDate = new(2026, 1, 2);
        Certificate certificate = new("Test Owner", "12345678000190", expirationDate);
        certificate.Revoke();
     
        // Act
        bool isValid = certificate.IsValid(referenceDate);
        
        // Assert
        Assert.False(isValid);
    }
}
