using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FiscCert.Infrastructure.Cryptography;

namespace FiscCert.Infrastructure.Tests.Cryptography;

public class CertificateReaderTests
{
    private readonly CertificateReader _sut = new();

    [Fact]
    public async Task ExtractMetadataAsync_ShouldExtractData_UsingIcpBrasilPattern()
    {
        // Arrange
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=VITOR E CIA LTDA:12345678000199",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var cert = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));
        
        var password = "senha_teste";
        var pfxBytes = cert.Export(X509ContentType.Pfx, password);
        using var stream = new MemoryStream(pfxBytes);

        // Act
        var result = await _sut.ReadCertificateAsync(stream, password);

        // Assert
        Assert.Equal("VITOR E CIA LTDA", result.OwnerName);
        Assert.Equal("12345678000199", result.FederalInscription);
        Assert.NotNull(result.SerialNumber);
    }
}