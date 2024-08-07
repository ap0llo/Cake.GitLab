using System;
using System.Collections.Generic;
using System.Linq;
using Grynwald.XmlDocs;
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
    private class Symbols
    {
        /// <summary>
        /// Gets the symbol for the <c>Cake.GitLab.GitLabAliases</c> type
        /// </summary>
        public INamedTypeSymbol GitLabAliases { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.GitLab.GitLabServerIdentity</c> type
        /// </summary>
        public INamedTypeSymbol GitLabServerIdentity { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.GitLab.GitLabServerConnection</c> type
        /// </summary>
        public INamedTypeSymbol GitLabServerConnection { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.GitLab.GitLabProjectIdentity</c> type
        /// </summary>
        public INamedTypeSymbol GitLabProjectIdentity { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.GitLab.GitLabProjectConnection</c> type
        /// </summary>
        public INamedTypeSymbol GitLabProjectConnection { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.Core.ICakeContext</c> type
        /// </summary>
        public INamedTypeSymbol CakeContext { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.Core.Annotations.CakeMethodAliasAttribute</c> type
        /// </summary>
        public INamedTypeSymbol CakeMethodAliasAttribute { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>System.Threading.Tasks.Task</c> type
        /// </summary>
        public INamedTypeSymbol SystemThreadingTasksTask { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>System.String</c> type
        /// </summary>
        public INamedTypeSymbol SystemString { get; set; } = null!;


        /// <summary>
        /// Gets the symbol for the <c>System.CodeDom.Compiler.GeneratedCodeAttribute</c> type
        /// </summary>
        public INamedTypeSymbol GeneratedCodeAttribute { get; set; } = null!;



        public static Symbols? TryGet(GeneratorExecutionContext generatorContext)
        {
            // Find required type symbols
            if (!TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabAliases", out var gitLabAliasesSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabServerIdentity", out var gitlabServerIdentitySymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabServerConnection", out var gitlabServerConnectionSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabProjectIdentity", out var gitlabProjectIdentitySymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabProjectConnection", out var gitlabProjectConnectionSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.Core.ICakeContext", out var cakeContextSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.Core.Annotations.CakeMethodAliasAttribute", out var cakeMethodAliasAttributeSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "System.Threading.Tasks.Task", out var systemThreadingTasksTaskSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "System.String", out var systemStringSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "System.CodeDom.Compiler.GeneratedCodeAttribute", out var generatedCodeAttributeSymbol)
            )
            {
                return null;
            }

            return new Symbols()
            {
                GitLabAliases = gitLabAliasesSymbol!,
                GitLabServerIdentity = gitlabServerIdentitySymbol!,
                GitLabServerConnection = gitlabServerConnectionSymbol!,
                GitLabProjectIdentity = gitlabProjectIdentitySymbol!,
                GitLabProjectConnection = gitlabProjectConnectionSymbol!,
                CakeContext = cakeContextSymbol!,
                CakeMethodAliasAttribute = cakeMethodAliasAttributeSymbol!,
                SystemThreadingTasksTask = systemThreadingTasksTaskSymbol!,
                SystemString = systemStringSymbol!,
                GeneratedCodeAttribute = generatedCodeAttributeSymbol!,
            };
        }

        private static bool TryGetSymbolByMetadataName(GeneratorExecutionContext generatorContext, string metadataName, out INamedTypeSymbol? symbol)
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

    private class Context : IDisposable
    {
        public GeneratorExecutionContext GeneratorContext { get; }

        public Symbols Symbols { get; }

        public SourceCodeBuilder Output { get; } = new();


        private Context(GeneratorExecutionContext generatorContext, Symbols symbols)
        {
            GeneratorContext = generatorContext;
            Symbols = symbols;
        }


        public void Dispose()
        {
            GeneratorContext.AddSource($"{Symbols.GitLabAliases.Name}.g.cs", Output.ToString());
        }


        public static Context? TryCreate(GeneratorExecutionContext generatorContext)
        {
            // Find required type symbols
            var symbols = Symbols.TryGet(generatorContext);
            if (symbols is null)
            {
                return null;
            }

            return new Context(generatorContext, symbols);
        }
    }

    private class ParameterReplacement
    {
        public string OriginalParameterName { get; set; } = null!;

        public ITypeSymbol NewParameterType { get; set; } = null!;

        public string NewParameterName { get; set; } = null!;

        /// <summary>
        /// Gets the expression to use as as argument for the replaced parameter when calling the original method
        /// </summary>
        public string ConversionExpression { get; set; } = null!;


        public bool MatchesOrigrinalParameter(IParameterSymbol parameter)
        {
            return parameter.Name == OriginalParameterName;
        }
    }


    public void Initialize(GeneratorInitializationContext context)
    { }

    public void Execute(GeneratorExecutionContext generatorContext)
    {
        using var context = Context.TryCreate(generatorContext);

        if (context is null)
        {
            return;
        }

        var aliases = context.Symbols.GitLabAliases
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(method =>
                method.DeclaredAccessibility == Accessibility.Public &&
                method.IsStatic &&
                method.Parameters.Length >= 3 &&
                SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, context.Symbols.CakeContext) &&
                SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, context.Symbols.SystemString) &&
                method.Parameters[1].Name == "serverUrl" &&
                SymbolEqualityComparer.Default.Equals(method.Parameters[2].Type, context.Symbols.SystemString) &&
                method.Parameters[2].Name == "accessToken"
            );

        if (!aliases.Any())
        {
            return;
        }


        context.Output.AppendFileScopedNamespace(context.Symbols.GitLabAliases.ContainingNamespace);

        context.Output.BeginLine();
        context.Output.Append($"public static partial class {context.Symbols.GitLabAliases.Name}");
        context.Output.EndLine();

        context.Output.BeginBlock();
        {
            foreach (var alias in aliases)
            {
                //
                // Generate an overload that uses a GitLabServerIdentity instead of the serverUrl string
                //
                GenerateOverload(context, alias,
                [
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "serverUrl",
                        NewParameterType = context.Symbols.GitLabServerIdentity,
                        NewParameterName = "serverIdentity",
                        ConversionExpression = "serverIdentity.Url"
                    }
                ]);

                //
                // Generate an overload that uses a GitLabServerConnection instead of the serverUrl and accessToken strings
                //
                GenerateOverload(context, alias,
                [
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "serverUrl",
                        NewParameterType = context.Symbols.GitLabServerConnection,
                        NewParameterName = "serverConnection",
                        ConversionExpression = "serverConnection.Url"
                    },
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "accessToken",
                        NewParameterType = context.Symbols.GitLabServerConnection,
                        NewParameterName = "serverConnection",
                        ConversionExpression = "serverConnection.AccessToken"
                    }
                ]);



                //
                // Generate an overload that uses a GitLabProjectIdentity instead of the serverUrl and project parameters
                //
                GenerateOverload(context, alias,
                [
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "serverUrl",
                        NewParameterType = context.Symbols.GitLabProjectIdentity,
                        NewParameterName = "projectIdentity",
                        ConversionExpression = "projectIdentity.Url"
                    },
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "project",
                        NewParameterType = context.Symbols.GitLabProjectIdentity,
                        NewParameterName = "projectIdentity",
                        ConversionExpression = "projectIdentity.ProjectPath"
                    }
                ]);



                //
                // Generate an overload that uses a GitLabProjectConnection instead of the serverUrl, project and accessToken parameters
                //
                GenerateOverload(context, alias,
                [
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "serverUrl",
                        NewParameterType = context.Symbols.GitLabProjectConnection,
                        NewParameterName = "projectConnection",
                        ConversionExpression = "projectConnection.Url"
                    },
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "accessToken",
                        NewParameterType = context.Symbols.GitLabProjectConnection,
                        NewParameterName = "projectConnection",
                        ConversionExpression = "projectConnection.AccessToken"
                    },
                    new ParameterReplacement()
                    {
                        OriginalParameterName = "project",
                        NewParameterType = context.Symbols.GitLabProjectConnection,
                        NewParameterName = "projectConnection",
                        ConversionExpression = "projectConnection.ProjectPath"
                    }
                ]);

            }

        }
        context.Output.EndBlock();
    }


    private static void GenerateOverload(Context context, IMethodSymbol alias, IReadOnlyList<ParameterReplacement> parameterReplacements)
    {
        var originalDocumentation = GetDocumentation(alias) as MethodMemberElement;
        var newDocumentation = default(MethodMemberElement);

        if (originalDocumentation is not null)
        {
            newDocumentation = new MethodMemberElement(originalDocumentation.Id)
            {
                Summary = originalDocumentation.Summary,
                Remarks = originalDocumentation.Remarks,
                Returns = originalDocumentation.Returns,
            };

            //TODO: Copy and adapt list members (Parameters, TypeParameters, Exceptions, SeeAlso)

            context.Output.Append(newDocumentation);
        }

        context.Output.BeginLine();
        context.Output.Append("[");
        context.Output.Append(context.Symbols.CakeMethodAliasAttribute);
        context.Output.Append("]");
        context.Output.EndLine();

        context.Output.BeginLine();
        context.Output.Append("[");
        context.Output.Append(context.Symbols.GeneratedCodeAttribute);
        context.Output.Append("(\"");
        context.Output.Append(nameof(ConvenienceAliasOverloardGenerator));
        context.Output.Append("\", \"\")");
        context.Output.Append("]");
        context.Output.EndLine();

        context.Output.BeginLine();
        context.Output.Append("public static ");


        // Return Type
        if (alias.IsAsync)
        {
            context.Output.Append("async ");
        }

        if (alias.ReturnsVoid)
        {
            context.Output.Append("void ");
        }
        else
        {
            context.Output.Append(alias.ReturnType);
            context.Output.Append(" ");
        }

        // Method Name
        context.Output.Append(alias.Name);

        //
        // Generate new parameter list: If a replacement is defined, omit parameter and add replacement instead, otherwise, add parameter unchanged
        //
        var addedParameters = new HashSet<string>();

        context.Output.Append("(this "); // assume, the overload if still an extension method
        foreach (var parameter in alias.Parameters)
        {
            if (parameterReplacements.SingleOrDefault(replacement => replacement.MatchesOrigrinalParameter(parameter)) is { } replacement)
            {
                // multiple parameter may have the same replacement => only add replacement once
                if (!addedParameters.Contains(replacement.NewParameterName))
                {
                    if (addedParameters.Count > 0)
                    {
                        context.Output.Append(", ");
                    }
                    context.Output.AppendParameter(replacement.NewParameterType, replacement.NewParameterName);
                    addedParameters.Add(replacement.NewParameterName);
                }
            }
            else
            {
                // add parameter unchanged
                if (addedParameters.Count > 0)
                {
                    context.Output.Append(", ");
                }
                context.Output.AppendParameter(parameter);
                addedParameters.Add(parameter.Name);
            }

        }
        context.Output.Append(")");
        context.Output.EndLine();


        context.Output.BeginBlock();
        {
            // Add null check for replacement parameters
            foreach (var parameterName in parameterReplacements.Where(x => x.NewParameterType.IsReferenceType).Select(x => x.NewParameterName).Distinct())
            {
                context.Output.BeginLine();
                context.Output.Append($"global::System.ArgumentNullException.ThrowIfNull(@{parameterName});");
                context.Output.EndLine();
            }

            context.Output.BeginLine();

            // add "return" if alias returns a value, otherwise omit it
            if (!alias.ReturnsVoid && !(alias.IsAsync && SymbolEqualityComparer.Default.Equals(alias.ReturnType, context.Symbols.SystemThreadingTasksTask)))
            {
                context.Output.Append("return ");
            }

            // Add "await" is alias is asnc
            if (alias.IsAsync)
            {
                context.Output.Append("await ");
            }

            // Call original alias
            context.Output.Append(context.Symbols.GitLabAliases);
            context.Output.Append(".");
            context.Output.Append(alias.Name);


            // Pass parameters or replacements as arguments
            context.Output.Append("(");
            context.Output.AppendArguments(
                alias.Parameters.Select(x =>
                {
                    if (parameterReplacements.SingleOrDefault(replacement => replacement.MatchesOrigrinalParameter(x)) is { } replacement)
                    {
                        return replacement.ConversionExpression;
                    }
                    else
                    {
                        return x.Name;
                    }
                })
            );
            context.Output.Append(");");

            context.Output.EndLine();
        }
        context.Output.EndBlock();

        // For readability, add empty line between methods
        context.Output.AppendLine();
    }

    private static MemberElement? GetDocumentation(ISymbol symbol)
    {
        var xmlString = symbol.GetDocumentationCommentXml();
        if (xmlString is null)
        {
            return null;
        }
        else
        {
            return MemberElement.FromXml(xmlString);
        }
    }
}
