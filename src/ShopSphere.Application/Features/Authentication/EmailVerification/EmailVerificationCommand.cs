using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.EmailVerification;

public sealed record EmailVerificationCommand(
    string Email,
    string Token)
    : IRequest<Result>;