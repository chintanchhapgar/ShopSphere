namespace ShopSphere.Infrastructure.Email.Models;

public sealed class EmailTemplateOptions
{
    public string CompanyName { get; init; } = "ShopSphere";

    public string SupportEmail { get; init; } = string.Empty;

    public string WebsiteUrl { get; init; } = string.Empty;

    public string LogoUrl { get; init; } = string.Empty;
}