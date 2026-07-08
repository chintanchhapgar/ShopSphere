public sealed record AddCartItemRequest(
    Guid ProductId,
    int Quantity);