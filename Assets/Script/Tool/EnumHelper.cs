using System;
using UnityEngine;

public class EnumHelper
{
    public static T ParseEnumOrDefault<T>(string value) where T : struct, Enum
    {
        if (!string.IsNullOrEmpty(value) && Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }

        Debug.LogWarning($"[Enum Parse] Could not parse '{value}' into {typeof(T).Name}, defaulting to {default(T)}");
        return default;
    }

    public static void ChangeEnumValue<T>(ref T current, int delta) where T : System.Enum
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));
        int index = System.Array.IndexOf(values, current);
        index = (index + delta + values.Length) % values.Length;
        current = values[index];
    }

    public static T GetEnumByOffset<T>(T current, int offset) where T : System.Enum
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));
        int index = System.Array.IndexOf(values, current);
        index = (index + offset + values.Length) % values.Length;
        return values[index];
    }

}
