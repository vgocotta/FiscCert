using FiscCert.Api.Controllers;
using FiscCert.Api.Requests;
using FiscCert.Application.Abstractions;
using FiscCert.Application.DTO;
using FiscCert.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FiscCert.Api.Tests.Controllers;

public class CertificatesControllerTests
{
    private readonly Mock<ICertificateService> _certificateServiceMock;
    private readonly CertificatesController _sut;

    public CertificatesControllerTests()
    {
        _certificateServiceMock = new Mock<ICertificateService>();
        _sut = new CertificatesController(_certificateServiceMock.Object);
    }

    [Fact]
    public async Task Upload_ShouldReturnOk_WhenCertificateIsValid()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "fake-file-content"u8.ToArray();
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(content));

        var request = new UploadCertificateRequest
        {
            File = fileMock.Object,
            Password = "senha",
            TenantId = Guid.NewGuid()
        };

        var expectedId = Guid.NewGuid();

        _certificateServiceMock
            .Setup(s => s.UploadCertificateAsync(It.IsAny<UploadCertificateDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.Upload(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var request = new UploadCertificateRequest { File = fileMock.Object, Password = "errada", TenantId = Guid.NewGuid() };

        _certificateServiceMock
            .Setup(s => s.UploadCertificateAsync(It.IsAny<UploadCertificateDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidCertificatePasswordException());

        // Act
        var result = await _sut.Upload(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task GetPassword_ShouldReturnOkWithPassword_WhenSuccessful()
    {
        // Arrange
        var certId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var expectedPassword = "senha_descriptografada";

        _certificateServiceMock.Setup(s => s.GetCertificatePasswordAsync(certId, tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPassword);

        // Act
        var result = await _sut.GetPassword(certId, tenantId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var value = okResult.Value;
        var passwordProperty = value?.GetType().GetProperty("Password")?.GetValue(value, null);

        Assert.Equal(expectedPassword, passwordProperty);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var certId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        _certificateServiceMock.Setup(s => s.DeleteCertificateAsync(certId, tenantId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Delete(certId, tenantId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}