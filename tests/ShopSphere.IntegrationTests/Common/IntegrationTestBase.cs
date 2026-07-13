using System.Net.Http.Json;

namespace ShopSphere.IntegrationTests.Common;

public abstract class IntegrationTestBase
    : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(
        CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected async Task<T?> ReadAsync<T>(
        HttpResponseMessage response)
    {
        return await response.Content
            .ReadFromJsonAsync<T>();
    }
}