using Microsoft.Extensions.Options;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.Email.Helpers;
using ShopSphere.Infrastructure.Email.Models;

namespace ShopSphere.Infrastructure.Email.Rendering;

public sealed class EmailTemplateRenderer
    : IEmailTemplateRenderer
{
    private readonly EmailTemplateOptions _options;

    public EmailTemplateRenderer(
        IOptions<EmailTemplateOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> RenderAsync(
        string templateName,
        Dictionary<string, string> placeholders)
    {
        var layout = TemplateReader.Read("Layout.html");

        var body = TemplateReader.Read(
            $"{templateName}.html");

        // Inject the email body into the layout first.
        var html = layout.Replace(
            "{{Content}}",
            body);

        // Replace global placeholders everywhere (layout + body).
        html = html
            .Replace("{{CompanyName}}", _options.CompanyName)
            .Replace("{{SupportEmail}}", _options.SupportEmail)
            .Replace("{{WebsiteUrl}}", _options.WebsiteUrl)
            .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());

        // Replace template-specific placeholders.
        foreach (var placeholder in placeholders)
        {
            html = html.Replace(
                $"{{{{{placeholder.Key}}}}}",
                placeholder.Value);
        }

        return html;
    }
}