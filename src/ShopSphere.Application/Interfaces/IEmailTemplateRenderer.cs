namespace ShopSphere.Application.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> Render(
    string templateName,
    Dictionary<string, string> placeholders);
}