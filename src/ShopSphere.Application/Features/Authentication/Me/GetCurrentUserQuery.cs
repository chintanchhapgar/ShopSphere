using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.Me;

public sealed record GetCurrentUserQuery
    : IRequest<Result<CurrentUserResponse>>;