using ShopSphere.Contracts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Application.Interfaces
{
    public interface IAIChatService
    {
        Task<ChatResponse> SendMessageAsync(ChatRequest request, string? userId, CancellationToken ct = default);

        Task<string> GenerateProductDescriptionAsync(
           string productName,
           string category,
           string brand,
           string? shortInfo,
           CancellationToken ct = default);
    }
}
