using MediatR;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<TokenResponse>>;