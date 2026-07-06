using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Authentication;

namespace ShopSphere.Application.Features.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, TokenResponse?>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<TokenResponse?> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(
            request.Email,
            request.Password);
    }
}