using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using ShopSphere.Contracts.AI;
using ShopSphere.Domain.Interfaces;
using System.Text.Json;

namespace ShopSphere.Infrastructure.AI;

public interface IAIChatService
{
    Task<ChatResponse> SendMessageAsync(ChatRequest request, string? userId, CancellationToken ct = default);
}

public sealed class OpenAIService : IAIChatService
{
    private readonly ChatClient _client;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OpenAIService> _logger;
    private readonly string _model;

    public OpenAIService(
        IConfiguration config,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        ILogger<OpenAIService> logger)
    {
        var apiKey = config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API Key missing");
        _model = config["OpenAI:Model"] ?? "gpt-4o-mini";

        _client = new ChatClient(_model, apiKey);
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<ChatResponse> SendMessageAsync(
        ChatRequest request,
        string? userId,
        CancellationToken ct = default)
    {
        try
        {
            // ── Get top 20 products for context ──────────────────────────────
            var products = await _productRepository.GetAllProductsAsync(ct);
            var productContext = string.Join("\n", products.Take(20).Select(p =>
                $"- {p.Name} (₹{p.BasePrice}) | Category: {p.Category.Name ?? "N/A"} | Brand: {p.Brand.Name ?? "N/A"} | ID: {p.Id}"));

            // ── System Prompt ────────────────────────────────────────────────
            var systemPrompt = $@"You are a helpful shopping assistant for ShopSphere, an e-commerce platform.

Your role:
- Help customers find products from our catalog
- Answer questions about products, orders, shipping, and returns
- Recommend products based on customer needs
- Be friendly, concise, and professional
- Use Indian Rupees (₹) for prices

Available Products in our catalog:
{productContext}

Rules:
- ONLY recommend products from the list above
- When recommending products, include the product ID in this format: [PRODUCT_ID:guid-here]
- Keep responses under 150 words
- If asked about orders, ask for the order number
- If unsure, politely say so
- Never make up prices or products

Format for recommendations:
When suggesting a product, use this exact format at the end of your message:
[PRODUCT_ID:id1] [PRODUCT_ID:id2]";

            // ── Build message history ────────────────────────────────────────
            var messages = new List<OpenAI.Chat.ChatMessage>
            {
                new SystemChatMessage(systemPrompt)
            };

            // Add conversation history
            if (request.History is not null)
            {
                foreach (var msg in request.History.TakeLast(10))
                {
                    if (msg.Role == "user")
                        messages.Add(new UserChatMessage(msg.Content));
                    else if (msg.Role == "assistant")
                        messages.Add(new AssistantChatMessage(msg.Content));
                }
            }

            // Current message
            messages.Add(new UserChatMessage(request.Message));

            // ── Call OpenAI ──────────────────────────────────────────────────
            var completion = await _client.CompleteChatAsync(
                messages,
                new ChatCompletionOptions
                {
                    Temperature = 0.7f,
                    MaxOutputTokenCount = 500,
                });

            var responseText = completion.Value.Content[0].Text;

            // ── Extract product IDs from response ────────────────────────────
            var productIds = ExtractProductIds(responseText);
            var suggestedProducts = new List<ProductSuggestion>();

            foreach (var id in productIds)
            {
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product is not null)
                {
                    suggestedProducts.Add(new ProductSuggestion(
                        product.Id,
                        product.Name,
                        product.BasePrice,
                        product.Images.FirstOrDefault()?.ImageUrl,
                        product.Category.Name ?? "General"));
                }
            }

            // Clean [PRODUCT_ID:xxx] tags from response
            var cleanMessage = System.Text.RegularExpressions.Regex.Replace(
                responseText,
                @"\[PRODUCT_ID:[a-f0-9\-]+\]",
                "").Trim();

            return new ChatResponse(cleanMessage, suggestedProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI chat error");
            return new ChatResponse(
                "I'm having trouble right now. Please try again later.",
                null);
        }
    }

    private static List<Guid> ExtractProductIds(string text)
    {
        var pattern = @"\[PRODUCT_ID:([a-f0-9\-]+)\]";
        var matches = System.Text.RegularExpressions.Regex.Matches(text, pattern);

        return matches
            .Select(m => m.Groups[1].Value)
            .Where(id => Guid.TryParse(id, out _))
            .Select(Guid.Parse)
            .Distinct()
            .ToList();
    }
}