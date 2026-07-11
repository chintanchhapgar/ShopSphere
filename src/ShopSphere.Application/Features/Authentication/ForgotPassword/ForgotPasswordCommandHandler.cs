using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IBackgroundJobService _backgroundJobs;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IBackgroundJobService backgroundJobs)
    {
        _identityService = identityService;
        _backgroundJobs = backgroundJobs;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var result =
            await _identityService.GeneratePasswordResetTokenAsync(
                request.Email);

        if (result is not null)
        {
            _backgroundJobs.Enqueue<IEmailJob>(
                x => x.SendPasswordResetAsync(
                    result.UserId,
                    result.Token));
        }

        return Result.Success(
            "If an account exists with this email, a password reset link has been sent.");
    }
}