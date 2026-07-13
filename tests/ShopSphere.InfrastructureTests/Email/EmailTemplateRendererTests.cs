using FluentAssertions;
using Microsoft.Extensions.Options;
using ShopSphere.Infrastructure.Email.Models;
using ShopSphere.Infrastructure.Email.Rendering;

namespace ShopSphere.InfrastructureTests.Email;

public sealed class EmailTemplateRendererTests
{
    private readonly EmailTemplateRenderer _renderer;

    public EmailTemplateRendererTests()
    {
        var options = Options.Create(
            new EmailTemplateOptions
            {
                CompanyName = "ShopSphere",
                SupportEmail = "support@shopsphere.com",
                WebsiteUrl = "https://shopsphere.com"
            });

        _renderer = new EmailTemplateRenderer(options);
    }

    [Fact]
    public async Task RenderAsync_Should_Replace_Template_Placeholders()
    {
        // Arrange
        var placeholders = new Dictionary<string, string>
        {
            ["CustomerName"] = "John",
            ["OrderNumber"] = "ORD-1001",
            ["TotalAmount"] = "1500.00"
        };

        // Act
        var html = await _renderer.RenderAsync(
            "OrderPlaced",
            placeholders);

        // Assert
        html.Should().Contain("John");
        html.Should().Contain("ORD-1001");
        html.Should().Contain("1500.00");
    }

    [Fact]
    public async Task RenderAsync_Should_Replace_Global_Placeholders()
    {
        // Act
        var html = await _renderer.RenderAsync(
            "Welcome",
            new Dictionary<string, string>
            {
                ["CustomerName"] = "John Doe"
            });

        // Assert
        html.Should().Contain("ShopSphere");
        html.Should().Contain("support@shopsphere.com");
        html.Should().Contain("https://shopsphere.com");
        html.Should().Contain(DateTime.UtcNow.Year.ToString());
    }

    [Fact]
    public async Task RenderAsync_Should_Inject_Body_Into_Layout()
    {
        // Act
        var html = await _renderer.RenderAsync(
            "Welcome",
            new Dictionary<string, string>
            {
                ["CustomerName"] = "John Doe"
            });

        // Assert
        html.Should().NotContain("{{Content}}");
        html.Should().Contain("John Doe");
    }

    [Fact]
    public async Task RenderAsync_Should_Return_Rendered_Html()
    {
        var html = await _renderer.RenderAsync(
            "Welcome",
            new Dictionary<string, string>
            {
                ["CustomerName"] = "John Doe"
            });

        html.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RenderAsync_Should_Leave_Unknown_Placeholders_Unchanged()
    {
        var html = await _renderer.RenderAsync(
            "Welcome",
            new Dictionary<string, string>());

        html.Should().Contain("{{");
    }

    [Fact]
    public async Task RenderAsync_Should_Throw_When_Template_Does_Not_Exist()
    {
        // Act
        Func<Task> action = async () =>
            await _renderer.RenderAsync(
                "UnknownTemplate",
                []);

        // Assert
        await action.Should()
            .ThrowAsync<FileNotFoundException>();
    }
}