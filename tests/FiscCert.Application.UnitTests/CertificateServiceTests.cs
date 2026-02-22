using FiscCert.Application.Abstractions;
using FiscCert.Application.Constants;
using FiscCert.Application.DTO;
using FiscCert.Application.Exceptions;
using FiscCert.Application.Services;
using FiscCert.Domain;
using Moq;
using System.Security.Cryptography;

namespace FiscCert.Application.UnitTests;

public class CertificateServiceTests
{
    private readonly Mock<ICertificateRepository> _repositoryMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<ICertificateReader> _certificateReaderMock;
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly CertificateService _sut;

    public CertificateServiceTests()
    {
        _repositoryMock = new Mock<ICertificateRepository>();
        _storageServiceMock = new Mock<IStorageService>();
        _certificateReaderMock = new Mock<ICertificateReader>();
        _encryptionServiceMock = new Mock<IEncryptionService>();

        _sut = new CertificateService(
            _repositoryMock.Object,
            _storageServiceMock.Object,
            _certificateReaderMock.Object,
            _encryptionServiceMock.Object);
    }

    [Fact]
    public async Task UploadCertificateAsync_ShouldReturnId_WhenEverythingIsValid()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var economicGroupId = Guid.NewGuid();
        var fileStream = new MemoryStream([1, 2, 3]);
        var password = "senha_correta";

        var inputDto = new UploadCertificateDto(tenantId, fileStream, password, economicGroupId);

        var fakeMetadata = new CertificateMetadata(
            "Empresa Teste",
            "12.345.678/0001-90",
            "SERIAL123",
            DateTime.UtcNow.AddYears(1));

        _certificateReaderMock
            .Setup(r => r.ReadCertificateAsync(fileStream, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeMetadata);

        // Act
        var resultId = await _sut.UploadCertificateAsync(inputDto);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);

        _storageServiceMock.Verify(s => s.UploadAsync(
            fileStream,
            It.Is<string>(path => path.StartsWith(tenantId.ToString()) && path.EndsWith(".pfx")),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<Certificate>(c =>
                c.TenantId == tenantId &&
                c.OwnerName == fakeMetadata.OwnerName &&
                c.EconomicGroupId == economicGroupId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UploadCertificateAsync_ShouldThrowInvalidCertificatePasswordException_WhenPasswordIsWrong()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var fileStream = new MemoryStream();
        var inputDto = new UploadCertificateDto(tenantId, fileStream, "senha_errada");

        _certificateReaderMock
            .Setup(r => r.ReadCertificateAsync(fileStream, "senha_errada", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CryptographicException("Wrong password"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidCertificatePasswordException>(
            () => _sut.UploadCertificateAsync(inputDto));

        Assert.Equal(ErrorMessages.InvalidCertificatePassword, exception.Message);

        _storageServiceMock.Verify(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Certificate>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
