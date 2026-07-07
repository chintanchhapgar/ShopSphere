using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(
    RegisterCommand request,
    CancellationToken cancellationToken)
    {
        var response = await _identityService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        if (!response.Succeeded)
        {
            // We'll improve this later to return actual Identity errors.
            return Result.Failure(AuthenticationErrors.RegistrationFailed);
        }

        return Result.Success("Registration completed successfully.");
    }
}