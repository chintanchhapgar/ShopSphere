using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Me;

public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
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

    public async Task<Result<CurrentUserResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            return Result<CurrentUserResponse>.Failure(
                AuthenticationErrors.Unauthorized);
        }

        var user = await _identity.GetCurrentUserAsync(
            _currentUser.UserId);

        if (user is null)
        {
            return Result<CurrentUserResponse>.Failure(
                UserErrors.NotFound);
        }

        return Result<CurrentUserResponse>.Success(
            user,
            "User retrieved successfully.");
    }
}