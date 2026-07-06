namespace ShopSphere.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
     string? Email { get; }
}