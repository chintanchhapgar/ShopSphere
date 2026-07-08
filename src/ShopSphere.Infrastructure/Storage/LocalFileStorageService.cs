using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Infrastructure.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageOptions _options;

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options)
    {
        _environment = environment;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        var uploadsPath = Path.Combine(
            _environment.WebRootPath,
            _options.UploadFolder);

        Directory.CreateDirectory(uploadsPath);

        var extension = Path.GetExtension(fileName);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(
            uploadsPath,
            storedFileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/{_options.UploadFolder}/{storedFileName}";
    }

    public Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        var relativePath = fileUrl
            .TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        var filePath = Path.Combine(
            _environment.WebRootPath,
            relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}