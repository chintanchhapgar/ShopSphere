using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Authentication.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<Result>;