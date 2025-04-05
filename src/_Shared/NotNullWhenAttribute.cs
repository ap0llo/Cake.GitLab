#if !NETCOREAPP3_0_OR_GREATER

using System;

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute
{
    public bool ReturnValue { get; }
    
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }
}
#endif
