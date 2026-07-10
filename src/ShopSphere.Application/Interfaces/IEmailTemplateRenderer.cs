namespace ShopSphere.Application.Interfaces;

public interface IEmailTemplateRenderer
{
    string Render(
        string templateName,
        IDictionary<string, string> placeholders);
}