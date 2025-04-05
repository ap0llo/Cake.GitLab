using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator.FakeGitLabProvider;

internal class Parameter
{
    public string Name { get; }

    public TypeName Type { get; }


    public Parameter(string name, TypeName type)
    {
        Name = name;
        Type = type;
    }


    public static Parameter FromSymbol(IParameterSymbol symbol)
    {
        return new Parameter(symbol.ToDisplayParts().Last().ToString(), TypeName.FromSymbol(symbol.Type));
    }
}
