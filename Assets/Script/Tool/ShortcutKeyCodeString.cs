using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum for all supported shortcut keys.
/// Includes letters, numbers, arrows, function keys, modifiers, and common keys.
/// </summary>
public enum ShortcutKey
{
    None,
    // Letters
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    // Numbers
    Alpha0, Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9,
    // Numpad
    Num0, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9, NumEnter,
    // Function keys
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    // Arrows
    UpArrow, DownArrow, LeftArrow, RightArrow,
    // Modifiers
    Shift, Ctrl, Alt,
    // Other keys
    Tab, Space, Enter, Backspace
}

/// <summary>
/// Utility class to convert ShortcutKey ↔ KeyCode and display string
/// </summary>
public static class ShortcutKeyCodeString
{
    private static readonly HashSet<KeyCode> blackListKeys = new()
    {
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3,
        KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6,
        KeyCode.Space, KeyCode.Tab, KeyCode.Return, KeyCode.KeypadEnter
    };


    private static readonly Dictionary<ShortcutKey, KeyCode> keyMap = new()
    {
        // Letters
        { ShortcutKey.A, KeyCode.A }, { ShortcutKey.B, KeyCode.B }, { ShortcutKey.C, KeyCode.C },
        { ShortcutKey.D, KeyCode.D }, { ShortcutKey.E, KeyCode.E }, { ShortcutKey.F, KeyCode.F },
        { ShortcutKey.G, KeyCode.G }, { ShortcutKey.H, KeyCode.H }, { ShortcutKey.I, KeyCode.I },
        { ShortcutKey.J, KeyCode.J }, { ShortcutKey.K, KeyCode.K }, { ShortcutKey.L, KeyCode.L },
        { ShortcutKey.M, KeyCode.M }, { ShortcutKey.N, KeyCode.N }, { ShortcutKey.O, KeyCode.O },
        { ShortcutKey.P, KeyCode.P }, { ShortcutKey.Q, KeyCode.Q }, { ShortcutKey.R, KeyCode.R },
        { ShortcutKey.S, KeyCode.S }, { ShortcutKey.T, KeyCode.T }, { ShortcutKey.U, KeyCode.U },
        { ShortcutKey.V, KeyCode.V }, { ShortcutKey.W, KeyCode.W }, { ShortcutKey.X, KeyCode.X },
        { ShortcutKey.Y, KeyCode.Y }, { ShortcutKey.Z, KeyCode.Z },

        // Numbers
        { ShortcutKey.Alpha0, KeyCode.Alpha0 }, { ShortcutKey.Alpha1, KeyCode.Alpha1 },
        { ShortcutKey.Alpha2, KeyCode.Alpha2 }, { ShortcutKey.Alpha3, KeyCode.Alpha3 },
        { ShortcutKey.Alpha4, KeyCode.Alpha4 }, { ShortcutKey.Alpha5, KeyCode.Alpha5 },
        { ShortcutKey.Alpha6, KeyCode.Alpha6 }, { ShortcutKey.Alpha7, KeyCode.Alpha7 },
        { ShortcutKey.Alpha8, KeyCode.Alpha8 }, { ShortcutKey.Alpha9, KeyCode.Alpha9 },

        // Numpad
        { ShortcutKey.Num0, KeyCode.Keypad0 }, { ShortcutKey.Num1, KeyCode.Keypad1 },
        { ShortcutKey.Num2, KeyCode.Keypad2 }, { ShortcutKey.Num3, KeyCode.Keypad3 },
        { ShortcutKey.Num4, KeyCode.Keypad4 }, { ShortcutKey.Num5, KeyCode.Keypad5 },
        { ShortcutKey.Num6, KeyCode.Keypad6 }, { ShortcutKey.Num7, KeyCode.Keypad7 },
        { ShortcutKey.Num8, KeyCode.Keypad8 }, { ShortcutKey.Num9, KeyCode.Keypad9 },
        { ShortcutKey.NumEnter, KeyCode.KeypadEnter },

        // Function keys
        { ShortcutKey.F1, KeyCode.F1 }, { ShortcutKey.F2, KeyCode.F2 }, { ShortcutKey.F3, KeyCode.F3 },
        { ShortcutKey.F4, KeyCode.F4 }, { ShortcutKey.F5, KeyCode.F5 }, { ShortcutKey.F6, KeyCode.F6 },
        { ShortcutKey.F7, KeyCode.F7 }, { ShortcutKey.F8, KeyCode.F8 }, { ShortcutKey.F9, KeyCode.F9 },
        { ShortcutKey.F10, KeyCode.F10 }, { ShortcutKey.F11, KeyCode.F11 }, { ShortcutKey.F12, KeyCode.F12 },

        // Arrows
        { ShortcutKey.UpArrow, KeyCode.UpArrow }, { ShortcutKey.DownArrow, KeyCode.DownArrow },
        { ShortcutKey.LeftArrow, KeyCode.LeftArrow }, { ShortcutKey.RightArrow, KeyCode.RightArrow },

        // Modifiers
        { ShortcutKey.Shift, KeyCode.LeftShift }, { ShortcutKey.Ctrl, KeyCode.LeftControl }, { ShortcutKey.Alt, KeyCode.LeftAlt },

        // Other keys
        { ShortcutKey.Tab, KeyCode.Tab }, { ShortcutKey.Space, KeyCode.Space },
        { ShortcutKey.Enter, KeyCode.Return }, { ShortcutKey.Backspace, KeyCode.Backspace }
    };

    /// <summary>
    /// Convert ShortcutKey to KeyCode
    /// </summary>
    public static KeyCode ToKeyCode(ShortcutKey key)
    {
        if (keyMap.TryGetValue(key, out KeyCode code)) return code;
        return KeyCode.None;
    }

    /// <summary>
    /// Convert ShortcutKey to display string (for UI)
    /// </summary>
    public static string ToDisplayString(ShortcutKey key)
    {
        string s = key.ToString();
        if (s.StartsWith("Alpha")) return s.Replace("Alpha", "Alpha ");
        if (s.StartsWith("Num")) return s.Replace("Num", "Num "); // Num0 → Num 0
        return s;
    }

    /// <summary>
    /// Capture pressed key and return corresponding ShortcutKey
    /// </summary>
    public static ShortcutKey GetPressedShortcutKey()
    {
        foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(code))
            {
                foreach (var kv in keyMap)
                    if (kv.Value == code) return kv.Key;
            }
        }
        return ShortcutKey.None;
    }

    public static bool IsBlackListed(KeyCode key)
    {
        return blackListKeys.Contains(key);
    }

}
