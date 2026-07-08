using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Interfaces;

public interface IFileValidationService
{
    Result Validate(
        string fileName,
        long fileSize);
}