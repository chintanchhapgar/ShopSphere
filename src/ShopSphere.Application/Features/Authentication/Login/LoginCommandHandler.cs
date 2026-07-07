using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<TokenResponse>> Handle(
     LoginCommand request,
     CancellationToken cancellationToken)
    {
        var token = await _identityService.LoginAsync(
            request.Email,
            request.Password);

        if (token is null)
        {
            return Result<TokenResponse>.Failure(
                AuthenticationErrors.InvalidCredentials);
        }

        return Result<TokenResponse>.Success(
            token,
            "Login successful.");
    }
}