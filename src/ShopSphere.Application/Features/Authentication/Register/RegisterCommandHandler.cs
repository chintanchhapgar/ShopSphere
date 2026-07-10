using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IBackgroundJobService _backgroundJobs;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IBackgroundJobService backgroundJobs)
    {
        _identityService = identityService;
        _backgroundJobs = backgroundJobs;
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

        if (!response.Succeeded || response.UserId is null)
        {
            return Result.Failure(
                AuthenticationErrors.RegistrationFailed);
        }

        _backgroundJobs.Enqueue<IEmailJob>(
            x => x.SendWelcomeAsync(response.UserId.Value));

        return Result.Success(
            "Registration completed successfully.");
    }
}