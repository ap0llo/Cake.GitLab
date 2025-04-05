using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Cake.GitLab.Generator.FakeGitLabProvider;

internal class TypeName
{
    public string Namespace { get; } = "";

    public string Name { get; }

    public string FullName
    {
        get
        {
            var fullName = String.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";

            if (TypeParameters.Count > 0)
            {
                fullName = $"{fullName}<{String.Join(", ", TypeParameters.Select(x => x.FullName))}>";
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
