namespace ShopSphere.Application.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync(
    string templateName,
    Dictionary<string, string> placeholders);
}