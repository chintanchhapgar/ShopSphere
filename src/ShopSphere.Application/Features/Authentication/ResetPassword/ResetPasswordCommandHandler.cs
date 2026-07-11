using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.Application.Features.Authentication.ResetPassword;

public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _identityService.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword);

        if (!success)
        {
            return Result.Failure(
                AuthenticationErrors.InvalidPasswordResetToken);
        }

        return Result.Success(
            "Password reset successfully.");
    }
}