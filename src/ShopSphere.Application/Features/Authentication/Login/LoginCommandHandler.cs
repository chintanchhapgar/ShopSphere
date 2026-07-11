using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<LoginCommandHandler> _logger;
    public LoginCommandHandler(IIdentityService identityService, ILogger<LoginCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
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
            _logger.LogWarning(
                "Failed login attempt for {Email}.",
                request.Email);

            return Result<TokenResponse>.Failure(
                AuthenticationErrors.InvalidCredentials);
        }

        _logger.LogInformation(
            "User ({Email}) logged in successfully.",
            request.Email);

        return Result<TokenResponse>.Success(
            token,
            "Login successful.");
    }
}