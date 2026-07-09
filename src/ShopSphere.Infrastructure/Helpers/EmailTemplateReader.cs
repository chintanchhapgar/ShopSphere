namespace ShopSphere.Infrastructure.Helpers;

public static class EmailTemplateReader
{
    public static string Read(string fileName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "EmailTemplates",
            fileName);

        return File.ReadAllText(path);
    }
}