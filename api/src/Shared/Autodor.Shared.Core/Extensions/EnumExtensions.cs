using System.ComponentModel;
using System.Reflection;

namespace Autodor.Shared.Core.Extensions;

/// <summary>
/// Extension methods for enum types
/// </summary>
public static class EnumExtensions
{
    /// <summary>Gets the Description attribute value of an enum member, if present</summary>
    /// <param name="value">The enum value to inspect</param>
    /// <param name="useNameAsFallback">If true, returns the enum name when no description attribute is found</param>
    /// <returns>The description attribute value, enum name if fallback is true, or null</returns>
    public static string? GetEnumDescription(this Enum? value, bool useNameAsFallback = false)
    {
        if (value == null)
            return null;

        // Retrieve the DescriptionAttribute from the enum member
        FieldInfo? fi = value.GetType().GetField(value.ToString());

        if (fi != null)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
        }

        return useNameAsFallback ? value.ToString() : null;
    }
}