using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator.FakeGitLabProvider;

internal class Method
{
    public TypeName ReturnType { get; }

    public string Name { get; }

    public List<Parameter> Parameters { get; } = [];

    public bool HasParameters => Parameters.Count > 0;


    public Method(TypeName returnType, string name)
    {
        ReturnType = returnType;
        Name = name;
    }


    public static Method FromSymbol(IMethodSymbol symbol)
    {
        var method = new Method(TypeName.FromSymbol(symbol.ReturnType), symbol.Name);
        method.Parameters.AddRange(symbol.Parameters.Select(Parameter.FromSymbol));
        return method;
    }
}
