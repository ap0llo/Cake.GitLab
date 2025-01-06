using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cake.GitLab.Internal;

/// <summary>
/// Helper class for mapping between enum and string values
/// </summary>
/// <remarks>
/// This class converts between enum values and their string equivalent.
/// In contrast to <see cref="System.Enum"/>, this class allows enums to defined string values that differ
/// from the enum names using the <see cref="EnumMemberAttribute">[EnumMember] attribute</see>.
/// For enums that do not use any <see cref="EnumMemberAttribute">[EnumMember] attributes</see>, this class behaves the same as the equivalent method from <see cref="System.Enum"/>.
/// </remarks>
internal static class EnumHelper
{
    private class EnumMap
    {
        private readonly Dictionary<Enum, string> m_Names = new();
        private readonly Dictionary<string, Enum> m_Values = new(StringComparer.OrdinalIgnoreCase);

        public string GetName<T>(T value) where T : struct, Enum => m_Names[value];

        public T GetValue<T>(string name) where T : struct, Enum
        {
            if (m_Values.TryGetValue(name, out var value))
            {
                return (T)value;
            }
            else
            {
                throw new ArgumentException($"Cannot convert value '{name}' to enum {typeof(T).Name}");
            }
        }


        public static EnumMap For<T>() where T : struct, Enum
        {
            var map = new EnumMap();
            foreach (var field in typeof(T).GetFields().Where(f => f.FieldType == typeof(T)))
            {
                var value = (T)field.GetValue(null)!;
                var name = field.Name;

                if (field.GetCustomAttribute<EnumMemberAttribute>() is { Value: { } customName } && !String.IsNullOrWhiteSpace(customName))
                {
                    name = customName;
                }

                map.m_Names[value] = name;
                map.m_Values[name] = value;
            }

            return map;
        }
    }

    private static readonly object s_SyncRoot = new();
    private static readonly Dictionary<Type, EnumMap> s_Cache = new();

    public static T Parse<T>(string value) where T : struct, Enum =>
        GetEnumMap<T>().GetValue<T>(value);

    public static string ConvertToString<T>(this T enumValue) where T : struct, Enum =>
        GetEnumMap<T>().GetName(enumValue);

    private static EnumMap GetEnumMap<T>() where T : struct, Enum
    {
        lock (s_SyncRoot)
        {
            if (s_Cache.TryGetValue(typeof(T), out var cachedMap))
            {
                return cachedMap;
            }
            else
            {
                var map = EnumMap.For<T>();
                s_Cache[typeof(T)] = map;
                return map;
            }
        }
    }
}
