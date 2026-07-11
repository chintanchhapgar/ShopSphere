using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword)
    : IRequest<Result>;