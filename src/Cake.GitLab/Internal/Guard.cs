using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cake.Common.Tools.VSWhere.Latest;

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

    public static T NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string valueExpression = "") where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(valueExpression);
        }

        return value;
    }

    public static int Positive(int value, [CallerArgumentExpression(nameof(value))] string valueExpression = "")
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(valueExpression, value, "Value must be positive");
        }

        return value;
    }
}
