using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Scriban;
using Scriban.Runtime;

namespace Cake.GitLab.Generator.FakeGitLabProvider;

/// <summary>
/// Source generator that generates the <c>FakeGitLabProvider</c> class in the <c>Cake.GitLab.Testing</c> package.
/// </summary>
[Generator]
public class FakeGitLabProviderGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    { }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.Compilation.AssemblyName != "Cake.GitLab.Testing")
        {
            return;
        }

        if (!TryGetSymbolByMetadataName(context, "Cake.GitLab.IGitLabProvider", out var gitLabProviderSymbol))
        {
            return;
        }

        var template = LoadTemplate(context);
        if (template is null)
        {
            return;
        }

        var model = LoadModel(gitLabProviderSymbol);

        var rootScriptObject = new ScriptObject
        {
            { "Model", model }
        };

        context.AddSource("FakeGitLabProvider.g.cs", template.Render(rootScriptObject, member => member.Name));
    }


    private static Model LoadModel(INamedTypeSymbol gitLabProviderSymbol)
    {
        var model = new Model();

        model.Methods.AddRange(gitLabProviderSymbol.GetMembers().OfType<IMethodSymbol>().Select(Method.FromSymbol));

        return model;
    }

    private static Template? LoadTemplate(GeneratorExecutionContext context)
    {
        var template = Template.Parse(EmbeddedResources.GetContent("FakeGitLabProvider/_Templates/FakeGitLabProvider.scriban-cs"));

        if (template.HasErrors)
        {
            foreach (var message in template.Messages.Where(x => x.Type == Scriban.Parsing.ParserMessageType.Error))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ScribanTemplateError, null, message.Message));
            }

            return null;
        }

        return template;
    }

    private static bool TryGetSymbolByMetadataName(GeneratorExecutionContext generatorContext, string metadataName, [NotNullWhen(true)] out INamedTypeSymbol? symbol)
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
