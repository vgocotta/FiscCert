using FiscCert.Application.Abstractions;

namespace FiscCert.Infrastructure.Storage;

public class LocalDiskStorageService : IStorageService
{
    private readonly string _basePath;

    public LocalDiskStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "SecureStorage");

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, blobPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found in server.");
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        return Task.FromResult(stream);
    }

    public async Task<string> UploadAsync(Stream fileStream, string blobPath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, blobPath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using FileStream fileStreamDestination = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await fileStream.CopyToAsync(fileStreamDestination, cancellationToken);

        return blobPath;
    }

    public Task DeleteFileAsync(string blobPath, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_basePath, blobPath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public async Task<byte[]> GetFileAsync(string blobPath, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_basePath, blobPath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found in server.");
        }

        return await File.ReadAllBytesAsync(filePath, cancellationToken);
    }
}
