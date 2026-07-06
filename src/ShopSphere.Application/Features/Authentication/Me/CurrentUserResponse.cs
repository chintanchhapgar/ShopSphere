namespace ShopSphere.Application.Features.Authentication.Me;

public sealed record CurrentUserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    IList<string> Roles);