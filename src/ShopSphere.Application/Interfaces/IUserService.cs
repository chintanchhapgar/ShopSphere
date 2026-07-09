namespace ShopSphere.Application.Interfaces;

public interface IUserService
{
   Task<UserInfo?> GetByIdAsync(
    string userId,
    CancellationToken cancellationToken = default);
}

public sealed record UserInfo(
    string Id,
    string FullName,
    string Email);