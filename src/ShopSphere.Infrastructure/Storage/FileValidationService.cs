using Microsoft.Extensions.Options;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Infrastructure.Storage;

public sealed class FileValidationService : IFileValidationService
{
    private readonly FileStorageOptions _options;

    public FileValidationService(
        IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public Result Validate(
        string fileName,
        long fileSize)
    {
        if (fileSize <= 0)
        {
            return Result.Failure(ProductImageErrors.EmptyFile);
        }

        if (fileSize > _options.MaxFileSizeInBytes)
        {
            return Result.Failure(ProductImageErrors.FileTooLarge);
        }

        var extension = Path.GetExtension(fileName);

        if (!_options.AllowedExtensions.Any(x =>
                x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Failure(ProductImageErrors.InvalidExtension);
        }

        return Result.Success();
    }
}