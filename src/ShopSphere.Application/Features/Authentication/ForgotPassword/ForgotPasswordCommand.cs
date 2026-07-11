using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email)
    : IRequest<Result>;