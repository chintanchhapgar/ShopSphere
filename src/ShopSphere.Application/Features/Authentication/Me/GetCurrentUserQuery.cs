using MediatR;

namespace ShopSphere.Application.Features.Authentication.Me;

public sealed record GetCurrentUserQuery()
    : IRequest<CurrentUserResponse?>;