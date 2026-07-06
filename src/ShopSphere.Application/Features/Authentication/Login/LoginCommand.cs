using MediatR;
using ShopSphere.Contracts.Authentication;

namespace ShopSphere.Application.Features.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<TokenResponse?>;