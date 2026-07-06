using MediatR;

namespace ShopSphere.Application.Authentication.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<Guid>;