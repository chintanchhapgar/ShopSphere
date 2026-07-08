namespace ShopSphere.Infrastructure.Storage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string Provider { get; set; } = "Local";

    public string UploadFolder { get; set; } = "uploads";

    public long MaxFileSizeInBytes { get; set; } = 5 * 1024 * 1024;

    public string[] AllowedExtensions { get; set; } =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];
}