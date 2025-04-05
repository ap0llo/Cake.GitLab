using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator.FakeGitLabProvider;

internal class TypeName
{
    public string Namespace { get; } = "";

    public string Name { get; }

    public string CSharpName
    {
        get
        {
            if (IsVoid)
            {
                return "void";
            }

            var fullName = String.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";

            if (TypeParameters.Count > 0)
            {
                fullName = $"{fullName}<{String.Join(", ", TypeParameters.Select(x => x.CSharpName))}>";
            }

            if (IsNullable)
            {
                fullName += "?";
            }

            return fullName;
        }
    }

    public List<TypeName> TypeParameters { get; } = [];

    public bool IsNullable { get; set; }

    public bool IsVoid => Namespace == "System" && Name == "Void";

    public bool IsTaskWithoutValue => Namespace == "System.Threading.Tasks" && Name == "Task" && TypeParameters.Count == 0;


    public TypeName(string @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }


    public static TypeName FromSymbol(ITypeSymbol symbol)
    {
        var type = new TypeName(symbol.ContainingNamespace.GetFullName(), symbol.Name)
        {
            IsNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated
        };

        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            type.TypeParameters.AddRange(namedTypeSymbol.TypeArguments.Select(TypeName.FromSymbol));
        }



        return type;
    }
}
