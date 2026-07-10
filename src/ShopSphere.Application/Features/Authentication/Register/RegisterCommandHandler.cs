using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    public RegisterCommandHandler(
        IIdentityService identityService, INotificationService notificationService)
    {
        _identityService = identityService;
        _notificationService = notificationService;
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

        await _notificationService.SendWelcomeEmailAsync(
            new WelcomeEmailModel(
                $"{request.FirstName} {request.LastName}".Trim(),
                request.Email!),
            cancellationToken);

        return Result.Success("Registration completed successfully.");
    }
}