using MediatR;
using ShopSphere.Application.Interfaces;
namespace ShopSphere.Application.Features.Authentication.Me;

public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse?>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identity;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUser,
        IIdentityService identity)
    {
        _currentUser = currentUser;
        _identity = identity;
    }

    public async Task<CurrentUserResponse?> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return null;

        return await _identity.GetCurrentUserAsync(
            _currentUser.UserId);
    }
}