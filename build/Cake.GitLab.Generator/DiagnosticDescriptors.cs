using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator;

internal static class DiagnosticDescriptors
{

#pragma warning disable RS2008 // Enable analyzer release tracking
    public static readonly DiagnosticDescriptor RequiredTypeNotFound =
        new DiagnosticDescriptor(
            id: "CGL001",
            title: "Requried type class not found",
            messageFormat: "The type {0} requried by the source generator was not found",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
}
