using System;
using System.Runtime.CompilerServices;

namespace Cake.GitLab.Internal;

internal static class Guard
{
    public static string NotNullOrWhitespace(string? value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (String.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value must not be null or whitespace", valueExpression);
        }

        return value;
    }
}
