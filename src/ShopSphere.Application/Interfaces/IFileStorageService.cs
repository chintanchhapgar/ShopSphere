public interface IFileStorageService
{
    Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken);
}