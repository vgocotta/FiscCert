namespace FiscCert.Application.Abstractions;

public interface IStorageService
{
    Task<string> UploadAsync(Stream fileStream, string blobName, CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string blobPath, CancellationToken cancellationToken);
}
