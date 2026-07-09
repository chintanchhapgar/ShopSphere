using System.Reflection;

namespace ShopSphere.Infrastructure.Email.Helpers;

public static class TemplateReader
{
    private static readonly Assembly Assembly =
        typeof(TemplateReader).Assembly;

    private const string BaseNamespace =
        "ShopSphere.Infrastructure.Email.Templates";

    public static string Read(string fileName)
    {
        var resourceName =
            $"{BaseNamespace}.{fileName}";

        using var stream =
            Assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new FileNotFoundException(
                $"Embedded email template '{resourceName}' was not found.");
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}