namespace ShopSphere.Contracts.AI;

public sealed record ChatRequest(
    string Message,
    List<ChatMessage>? History = null);

public sealed record ChatMessage(
    string Role,      // "user" or "assistant"
    string Content);

public sealed record ChatResponse(
    string Message,
    List<ProductSuggestion>? Products = null);

public sealed record ProductSuggestion(
    Guid Id,
    string Name,
    decimal Price,
    string? ImageUrl,
    string Category);