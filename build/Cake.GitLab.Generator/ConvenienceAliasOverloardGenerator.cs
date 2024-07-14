using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator;

/// <summary>
/// Source generator that generates additional overloads for all Cake aliases in the GitLabAliases class
/// </summary>
/// <remarks>
/// By default, Cake aliases are extension methods or ICakeContext and the GitLabAliases require a <c>GitLabConnection</c> to be passed in.
/// To make it more convenient to call the GitLab aliases from a Cake script, the <c>connection</c> parameter may be omitted of the cotext implements <c>IGitLabConnectionCakeContext</c>.
/// <para>
/// To avoid having to define aditionall overloads manually, this generator generates an extension method on <c>IGitLabConnectionCakeContext</c> for each alias method and call the original alaias with the conenction retrieed from the <c>IGitLabConnectionCakeContext</c> context.
/// </para>
/// </remarks>
[Generator]
public class ConvenienceAliasOverloardGenerator : ISourceGenerator
{
    private const string s_Indent = "    ";

    public void Initialize(GeneratorInitializationContext context)
    { }

    public void Execute(GeneratorExecutionContext context)
    {
        // Find required type symbols
        if (!TryGetSymbolByMetadataName(context, "Cake.GitLab.GitLabAliases", out var gitLabAliasesSymbol) ||
           !TryGetSymbolByMetadataName(context, "Cake.Core.ICakeContext", out var cakeContextSymbol) ||
           !TryGetSymbolByMetadataName(context, "Cake.GitLab.GitLabConnection", out var gitlabConnectionSymbol) ||
           !TryGetSymbolByMetadataName(context, "Cake.GitLab.IGitLabConnectionCakeContext", out var gitlabConnectionCakeContextSymbol) ||
           !TryGetSymbolByMetadataName(context, "System.Threading.Tasks.Task", out var systemThreadingTasksTaskSymbol))
        {
            return;
        }

        var aliases = gitLabAliasesSymbol!
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(method =>
                method.DeclaredAccessibility == Accessibility.Public &&
                method.IsStatic &&
                method.Parameters.Length >= 2 &&
                SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, cakeContextSymbol) &&
                SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, gitlabConnectionSymbol)
            );


        if (!aliases.Any())
        {
            return;
        }

        var sourceBuilder = new StringBuilder();

        var indent = 0;

        BeginFile(sourceBuilder);

        Namespace(sourceBuilder, GetFullName(gitLabAliasesSymbol!.ContainingNamespace));

        BeginLine(sourceBuilder, indent);
        sourceBuilder.Append($"public static partial class {gitLabAliasesSymbol.Name}");
        EndLine(sourceBuilder);

        BeginBlock(sourceBuilder, ref indent);
        {
            foreach (var alias in aliases)
            {
                // TODO: Generate /// documentation comments based on documentation of original alias

                BeginLine(sourceBuilder, indent);
                sourceBuilder.Append("[global::Cake.Core.Annotations.CakeMethodAlias]");
                EndLine(sourceBuilder);

                BeginLine(sourceBuilder, indent);
                sourceBuilder.Append("public static ");


                // Return Type
                if (alias.IsAsync)
                {
                    sourceBuilder.Append("async ");
                }

                if (alias.ReturnsVoid)
                {
                    sourceBuilder.Append("void ");
                }
                else
                {
                    sourceBuilder.Append("global::");
                    sourceBuilder.Append(GetFullName(alias.ReturnType));
                    sourceBuilder.Append(" ");
                }

                // Method Name
                sourceBuilder.Append(alias.Name);

                // Parameters (first parameter of type IGitLabConnectionCakeContext replaces the first two parameters of the original declaration)
                sourceBuilder.Append("(this global::");
                sourceBuilder.Append(GetFullName(gitlabConnectionCakeContextSymbol!));
                sourceBuilder.Append(" context");

                // Add remaining parameters unchanged
                foreach (var parameter in alias.Parameters.Skip(2))
                {
                    sourceBuilder.Append(", ");
                    sourceBuilder.Append("global::");
                    sourceBuilder.Append(GetFullName(parameter.Type));
                    sourceBuilder.Append(" ");
                    sourceBuilder.Append("@");
                    sourceBuilder.Append(parameter.Name);
                }
                sourceBuilder.Append(")");
                EndLine(sourceBuilder);

                BeginBlock(sourceBuilder, ref indent);
                {
                    BeginLine(sourceBuilder, indent);

                    // add "return" if alias returns a value, otherwise omit it
                    if (!alias.ReturnsVoid && !(alias.IsAsync && SymbolEqualityComparer.Default.Equals(alias.ReturnType, systemThreadingTasksTaskSymbol)))
                    {
                        sourceBuilder.Append("return ");
                    }

                    // Add "await" is alias is asnc
                    if (alias.IsAsync)
                    {
                        sourceBuilder.Append("await ");
                    }

                    // Call original alias
                    sourceBuilder.Append("global::");
                    sourceBuilder.Append(GetFullName(gitLabAliasesSymbol));
                    sourceBuilder.Append(".");
                    sourceBuilder.Append(alias.Name);

                    // Pass first two parameters (context and the connection provided by the context)
                    sourceBuilder.Append("(context, context.Connection");

                    // Pass remaining parameter
                    foreach (var parameter in alias.Parameters.Skip(2))
                    {
                        sourceBuilder.Append(", ");
                        sourceBuilder.Append("@");
                        sourceBuilder.Append(parameter.Name);
                    }
                    sourceBuilder.Append(");");

                    EndLine(sourceBuilder);
                }
                EndBlock(sourceBuilder, ref indent);

                // For readability, add empty line between methods
                sourceBuilder.AppendLine();

            }

        }
        EndBlock(sourceBuilder, ref indent);



        context.AddSource($"{gitLabAliasesSymbol.Name}.g.cs", sourceBuilder.ToString());
    }


    private static string GetFullName(INamespaceOrTypeSymbol symbol)
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

    private bool TryGetSymbolByMetadataName(GeneratorExecutionContext context, string metadataName, out INamedTypeSymbol? symbol)
    {
        symbol = context.Compilation.GetTypeByMetadataName(metadataName);

        if (symbol is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RequiredTypeNotFound, location: null, metadataName));
            return false;
        }
        else
        {
            return true;
        }
    }

    private void BeginFile(StringBuilder output)
    {
        output.AppendLine("// <auto-generated/>");
        output.AppendLine();
    }

    private void Namespace(StringBuilder output, string @namespace)
    {
        output.Append($"namespace ");
        output.Append(@namespace);
        output.Append(";");
        output.AppendLine();

        output.AppendLine();

    }

    private void BeginBlock(StringBuilder output, ref int indent)
    {
        for (int i = 0; i < indent; i++)
        {
            output.Append(s_Indent);
        }
        output.AppendLine("{");

        indent += 1;
    }

    private void EndBlock(StringBuilder output, ref int indent)
    {
        indent -= 1;

        for (int i = 0; i < indent; i++)
        {
            output.Append(s_Indent);
        }
        output.AppendLine("}");

    }

    private void BeginLine(StringBuilder output, int indent = 0)
    {
        for (int i = 0; i < indent; i++)
        {
            output.Append(s_Indent);
        }
    }

    private void EndLine(StringBuilder output)
    {
        output.AppendLine();
    }
}
