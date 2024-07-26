using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
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
        /// Gets the symbol for the <c>Cake.GitLab.GitLabConnection</c> type
        /// </summary>
        public INamedTypeSymbol GitLabServerConnection { get; set; } = null!;

        /// <summary>
        /// Gets the symbol for the <c>Cake.GitLab.IGitLabConnectionCakeContext</c> type
        /// </summary>
        public INamedTypeSymbol GitLabServerConnectionCakeContext { get; set; } = null!;

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

        public static Symbols? TryGet(GeneratorExecutionContext generatorContext)
        {
            // Find required type symbols
            if (!TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabAliases", out var gitLabAliasesSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.GitLabServerConnection", out var gitlabServerConnectionSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.GitLab.IGitLabServerConnectionCakeContext", out var gitlabServerConnectionCakeContextSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.Core.ICakeContext", out var cakeContextSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "Cake.Core.Annotations.CakeMethodAliasAttribute", out var cakeMethodAliasAttributeSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "System.Threading.Tasks.Task", out var systemThreadingTasksTaskSymbol) ||
               !TryGetSymbolByMetadataName(generatorContext, "System.String", out var systemStringSymbol)
            )
            {
                return null;
            }

            return new Symbols()
            {
                GitLabAliases = gitLabAliasesSymbol!,
                GitLabServerConnection = gitlabServerConnectionSymbol!,
                GitLabServerConnectionCakeContext = gitlabServerConnectionCakeContextSymbol!,
                CakeContext = cakeContextSymbol!,
                CakeMethodAliasAttribute = cakeMethodAliasAttributeSymbol!,
                SystemThreadingTasksTask = systemThreadingTasksTaskSymbol!,
                SystemString = systemStringSymbol!
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
                GenerateOverloadForGitLabConnectionCakeContext(context, alias);
                GenerateOverloadForGitLabConnection(context, alias);
            }

        }
        context.Output.EndBlock();
    }


    private static void GenerateOverloadForGitLabConnectionCakeContext(Context context, IMethodSymbol alias)
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

        // Parameters (first parameter of type IGitLabConnectionCakeContext replaces the first two parameters of the original declaration)
        context.Output.Append("(this ");
        context.Output.Append(context.Symbols.GitLabServerConnectionCakeContext);
        context.Output.Append(" context");

        // Add remaining parameters unchanged
        foreach (var parameter in alias.Parameters.Skip(3))
        {
            context.Output.Append(", ");
            context.Output.AppendParameter(parameter);
        }
        context.Output.Append(")");
        context.Output.EndLine();

        context.Output.BeginBlock();
        {
            context.Output.BeginLine();

            // add "return" if alias returns a value, otherwise omit it
            if (!alias.ReturnsVoid && !(alias.IsAsync && SymbolEqualityComparer.Default.Equals(alias.ReturnType, context.Symbols.SystemThreadingTasksTask)))
            {
                context.Output.Append("return ");
            }

            // Add "await" is alias is async
            if (alias.IsAsync)
            {
                context.Output.Append("await ");
            }

            // Call original alias
            context.Output.Append(context.Symbols.GitLabAliases);
            context.Output.Append(".");
            context.Output.Append(alias.Name);

            // Pass first two parameters (context and the connection details provided by the context)
            context.Output.Append("(");
            var arguments = Enumerable.Concat(
                ["context", "context.Connection.Url", "context.Connection.AccessToken"],
                alias.Parameters.Skip(3).Select(x => x.Name)
            );
            context.Output.AppendArguments(arguments);
            context.Output.Append(");");

            context.Output.EndLine();
        }
        context.Output.EndBlock();

        // For readability, add empty line between methods
        context.Output.AppendLine();
    }

    private static void GenerateOverloadForGitLabConnection(Context context, IMethodSymbol alias)
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

        // First parameter is unchanged (ICakeContext)        
        context.Output.Append("(this ");
        context.Output.AppendParameter(alias.Parameters[0]);

        // Second and third parameters (serverUrl and accessToken) are replaced with a single parameter of type GitLabConnection
        context.Output.Append(", ");
        context.Output.Append(context.Symbols.GitLabServerConnection);
        context.Output.Append(" @connection");

        // Add remaining parameters unchanged
        foreach (var parameter in alias.Parameters.Skip(3))
        {
            context.Output.Append(", ");
            context.Output.AppendParameter(parameter);
        }
        context.Output.Append(")");
        context.Output.EndLine();

        context.Output.BeginBlock();
        {
            // Add null check for "connection" parameter
            context.Output.BeginLine();
            context.Output.Append("global::System.ArgumentNullException.ThrowIfNull(connection);");
            context.Output.EndLine();

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


            // Pass first three parameters (context and the connection details provided by the GitLabConnection)
            context.Output.Append("(");

            // Pass remaining parameter
            var arguments = Enumerable.Concat(
              ["context", "connection.Url", "connection.AccessToken"],
              alias.Parameters.Skip(3).Select(x => x.Name)
            );
            context.Output.AppendArguments(arguments);

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
