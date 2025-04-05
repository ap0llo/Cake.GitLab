using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator;

internal static class Extensions
{
    public static string GetFullName(this INamespaceOrTypeSymbol symbol)
    {
        string name;
        if (symbol.ContainingNamespace is not null && GetFullName(symbol.ContainingNamespace) is string parentName && !String.IsNullOrEmpty(parentName))
        {
            name = $"{parentName}.{symbol.Name}";
        }
        else
        {
            name = symbol.Name;
        }

        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.Arity > 0)
            {
                name += $"<{String.Join(",", namedTypeSymbol.TypeArguments.Select(GetFullName))}>";
            }
        }

        return name;
    }

    public static bool TryGetSymbolByMetadataName(this GeneratorExecutionContext generatorContext, string metadataName, out INamedTypeSymbol? symbol)
    {
        symbol = generatorContext.Compilation.GetTypeByMetadataName(metadataName);

        if (symbol is null)
        {
            generatorContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RequiredTypeNotFound, location: null, metadataName));
            return false;
        }
        else
        {
            return true;
        }
    }
}
