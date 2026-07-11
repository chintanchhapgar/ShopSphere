using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.EmailVerification;

public sealed class EmailVerificationCommandHandler
    : IRequestHandler<EmailVerificationCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IBackgroundJobService _backgroundJobs;

    public EmailVerificationCommandHandler(
        IIdentityService identityService,
        IBackgroundJobService backgroundJobs)
    {
        _identityService = identityService;
        _backgroundJobs = backgroundJobs;
    }

    public async Task<Result> Handle(
        EmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _identityService.VerifyEmailAsync(
            request.Email,
            request.Token);

        if (!success)
        {
            return Result.Failure(
                AuthenticationErrors.InvalidEmailVerificationToken);
        }

        var userId = await _identityService.GetUserIdByEmailAsync(
            request.Email);

        if (userId.HasValue)
        {
            _backgroundJobs.Enqueue<IEmailJob>(
                x => x.SendWelcomeAsync(userId.Value));
        }

        return Result.Success(
            "Email verified successfully.");
    }
}