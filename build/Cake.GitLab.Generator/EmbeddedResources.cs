using System;
using System.IO;
using System.Reflection;

namespace Cake.GitLab.Generator;

internal static class EmbeddedResources
{
    public static string GetContent(string logicalName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(logicalName);

        if (stream is null)
            throw new InvalidOperationException($"Embedded resource '{logicalName}' does not exist");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
