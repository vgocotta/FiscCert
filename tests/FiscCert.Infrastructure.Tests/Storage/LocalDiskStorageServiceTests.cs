using FiscCert.Infrastructure.Storage;

namespace FiscCert.Infrastructure.Tests.Storage;

public class LocalDiskStorageServiceTests
{
    private readonly LocalDiskStorageService _sut = new();

    [Fact]
    public async Task UploadAndDownload_ShouldSaveAndRetrieveFileSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var blobPath = $"{tenantId}/test-cert.pfx";
        var dummyContent = new byte[] { 1, 2, 3, 4, 5 };
        using var uploadStream = new MemoryStream(dummyContent);

        // Act - Upload
        var savedPath = await _sut.UploadAsync(uploadStream, blobPath);

        // Act - Download
        byte[] downloadedContent;
        using (var downloadStream = await _sut.DownloadAsync(savedPath))
        {
            using var memoryStream = new MemoryStream();
            await downloadStream.CopyToAsync(memoryStream);
            downloadedContent = memoryStream.ToArray();
        }

        // Assert
        Assert.Equal(blobPath, savedPath);
        Assert.Equal(dummyContent, downloadedContent);

        // Cleanup
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "SecureStorage", tenantId.ToString());
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, true);
        }
    }
}