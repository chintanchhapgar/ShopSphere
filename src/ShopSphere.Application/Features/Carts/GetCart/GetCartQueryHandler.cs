using MediatR;
using ShopSphere.Application.Features.Carts.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.GetCart;

public sealed class GetCartQueryHandler
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly ICartQueries _queries;
    private readonly ICurrentUserService _currentUser;

    public GetCartQueryHandler(
        ICartQueries queries,
        ICurrentUserService currentUser)
    {
        _queries = queries;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var customerId))
        {
            return Result<CartDto>.Failure(
                new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var cart = await _queries.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        return Result<CartDto>.Success(
            cart ?? new CartDto([], 0));
    }
}