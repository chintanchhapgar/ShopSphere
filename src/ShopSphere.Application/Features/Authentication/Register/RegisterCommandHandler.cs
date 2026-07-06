using MediatR;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<RegisterResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        if (!result.Succeeded)
        {
            return new RegisterResponse(
                false,
                string.Join(", ", result.Errors));
        }

        return new RegisterResponse(
            true,
            "User registered successfully.");
    }
}