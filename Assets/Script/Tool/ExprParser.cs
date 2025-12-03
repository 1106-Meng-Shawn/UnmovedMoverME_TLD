using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class ExprParser
{

    public static bool ParseBool(string val) =>
        val.Equals("true", StringComparison.OrdinalIgnoreCase) || val == "1";

    public static bool? ParseBoolNullable(string val) =>
        val.Equals("true", StringComparison.OrdinalIgnoreCase) ? true
        : val.Equals("false", StringComparison.OrdinalIgnoreCase) ? (bool?)false
        : val == "1" ? (bool?)true
        : val == "0" ? (bool?)false
        : null;

    public static int[] ParseArgsToIntArray(string line)
    {
        int start = line.IndexOf('(');
        int end = line.IndexOf(')');
        if (start < 0 || end < 0 || end <= start) return Array.Empty<int>();

        string argsPart = line.Substring(start + 1, end - start - 1);
        var parts = argsPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var list = new List<int>();
        foreach (var p in parts)
        {
            if (int.TryParse(p.Trim(), out int val))
                list.Add(val);
        }

        return list.ToArray();
    }


    public static List<string> SplitTopLevel(string input, char delimiter)
    {
        var parts = new List<string>();
        int depthBrace = 0, depthBracket = 0, depthParen = 0;
        int start = 0;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '{') depthBrace++;
            else if (c == '}') depthBrace = Math.Max(0, depthBrace - 1);
            else if (c == '[') depthBracket++;
            else if (c == ']') depthBracket = Math.Max(0, depthBracket - 1);
            else if (c == '(') depthParen++;
            else if (c == ')') depthParen = Math.Max(0, depthParen - 1);
            else if (c == delimiter && depthBrace == 0 && depthBracket == 0 && depthParen == 0)
            {
                parts.Add(input[start..i]);
                start = i + 1;
            }
        }
        if (start < input.Length)
            parts.Add(input[start..]);

        return parts.Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
    }


    public static List<string> SplitTopLevelBlocks(string input, char open, char close)
    {
        var result = new List<string>();
        int depth = 0, start = -1;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == open)
            {
                if (depth == 0) start = i;
                depth++;
            }
            else if (c == close)
            {
                depth--;
                if (depth == 0 && start >= 0)
                {
                    result.Add(input[start..(i + 1)]);
                    start = -1;
                }
            }
        }

        return result;
    }



    public static List<string> SplitTopLevelArgs(string argStr)
    {
        var args = new List<string>();
        if (string.IsNullOrEmpty(argStr)) return args;

        int level = 0;
        int lastSplit = 0;
        for (int i = 0; i < argStr.Length; i++)
        {
            char c = argStr[i];
            if (c == '{') level++;
            else if (c == '}') level--;
            else if (c == ',' && level == 0)
            {
                args.Add(argStr.Substring(lastSplit, i - lastSplit).Trim());
                lastSplit = i + 1;
            }
        }
        args.Add(argStr.Substring(lastSplit).Trim());
        return args;
    }

    public static string TrimBrackets(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;
        s = s.Trim();
        if (s.StartsWith("[") && s.EndsWith("]")) return s.Substring(1, s.Length - 2);
        return s;
    }

    public static List<int> ParseListInt(string s)
    {
        var result = new List<int>();
        if (string.IsNullOrWhiteSpace(s)) return result;

        s = TrimBrackets(s);
        string[] parts = s.Split(',');
        foreach (var part in parts)
        {
            if (int.TryParse(part.Trim(), out int val))
                result.Add(val);
        }
        return result;
    }

    public static List<List<string>> ParseListOfLists(string s)
    {
        var result = new List<List<string>>();
        if (string.IsNullOrWhiteSpace(s)) return result;

        s = TrimBrackets(s);
        var inner = new StringBuilder();
        int depth = 0;

        foreach (char c in s)
        {
            if (c == ',' && depth == 0)
            {
                if (inner.Length > 0)
                {
                    result.Add(ParseListString(inner.ToString()));
                    inner.Clear();
                }
            }
            else
            {
                if (c == '[') depth++;
                if (c == ']') depth--;
                inner.Append(c);
            }
        }

        if (inner.Length > 0)
            result.Add(ParseListString(inner.ToString()));

        return result;
    }

    private static List<string> ParseListString(string s)
    {
        var result = new List<string>();
        s = TrimBrackets(s);
        string[] parts = s.Split(',');
        foreach (var part in parts)
        {
            string p = part.Trim();
            if (!string.IsNullOrEmpty(p))
                result.Add(p);
        }
        return result;
    }



    public static object[] ParseArgsToObjectArray(string expr)
    {
        int start = expr.IndexOf('(');
        int end = expr.LastIndexOf(')');

        if (start < 0 || end < 0 || end <= start)
            return Array.Empty<object>();

        string argStr = expr.Substring(start + 1, end - start - 1);
        var rawArgs = SplitArgs(argStr);
        var results = new List<object>();

        foreach (var arg in rawArgs)
        {
            string trimmed = arg.Trim();
            if ((trimmed.StartsWith("'") && trimmed.EndsWith("'")) ||
                (trimmed.StartsWith("\"") && trimmed.EndsWith("\"")))
            {
                results.Add(trimmed.Substring(1, trimmed.Length - 2));
            }
            else if (bool.TryParse(trimmed, out bool b))
            {
                results.Add(b);
            }
            else if (int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
            {
                results.Add(i);
            }
            else if (float.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
            {
                results.Add(f);
            }
            else
            {
                throw new Exception($"Cannot parse argument: {trimmed}");
            }
        }

        return results.ToArray();
    }
    private static List<string> SplitArgs(string argStr)
    {
        var args = new List<string>();
        if (string.IsNullOrEmpty(argStr)) return args;

        int start = 0;
        bool inQuotes = false;
        char quoteChar = '\0';

        for (int i = 0; i < argStr.Length; i++)
        {
            char c = argStr[i];

            if (c == '\'' || c == '"')
            {
                if (inQuotes)
                {
                    if (c == quoteChar) inQuotes = false;
                }
                else
                {
                    inQuotes = true;
                    quoteChar = c;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                args.Add(argStr.Substring(start, i - start));
                start = i + 1;
            }
        }

        args.Add(argStr.Substring(start));

        return args;
    }

    // ==================== Parameter Parsing ====================

    /// <summary>
    /// Parse a single command with format: command(argument)
    /// Returns (command, argument) or (null, null) if format is invalid.
    /// </summary>
    public static (string command, string argument) ParseCommandWithArguments(string rawEffect)
    {
        rawEffect = rawEffect.Trim();

        int openIndex = rawEffect.IndexOf('(');
        if (openIndex < 0 || !rawEffect.EndsWith(")"))
        {
            UnityEngine.Debug.LogWarning($"Invalid effect format: {rawEffect}");
            return (null, null);
        }

        string command = rawEffect.Substring(0, openIndex).Trim();
        string argument = rawEffect.Substring(openIndex + 1, rawEffect.Length - openIndex - 2).Trim();

        return (command, argument);
    }

    /// <summary>
    /// Parse a single integer parameter.
    /// Example: "5" -> out: 5
    /// </summary>
    public static bool TryParseInt(string arg, out int result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(arg))
            return false;

        return int.TryParse(arg.Trim(), out result);
    }

    /// <summary>
    /// Parse two comma-separated integers.
    /// Example: "5, 10" -> out: (5, 10)
    /// </summary>
    public static bool TryParseTwoInts(string arg, out int first, out int second)
    {
        first = second = 0;

        if (string.IsNullOrWhiteSpace(arg))
            return false;

        string[] parts = arg.Split(',');
        if (parts.Length != 2)
            return false;

        return int.TryParse(parts[0].Trim(), out first) &&
               int.TryParse(parts[1].Trim(), out second);
    }

    /// <summary>
    /// Parse two comma-separated strings.
    /// Example: "hello, world" -> out: ("hello", "world")
    /// </summary>
    public static bool TryParseTwoStrings(string arg, out string first, out string second)
    {
        first = second = null;

        if (string.IsNullOrWhiteSpace(arg))
            return false;

        string[] parts = arg.Split(',');
        if (parts.Length != 2)
            return false;

        first = parts[0].Trim();
        second = parts[1].Trim();
        return true;
    }

    /// <summary>
    /// Parse three parameters: (string, int, int).
    /// Example: "regionA, 1, 2" -> out: ("regionA", 1, 2)
    /// </summary>
    public static bool TryParseStringIntInt(string arg, out string str, out int int1, out int int2)
    {
        str = null;
        int1 = int2 = 0;

        if (string.IsNullOrWhiteSpace(arg))
            return false;

        string[] parts = arg.Split(',');
        if (parts.Length != 3)
            return false;

        str = parts[0].Trim();
        return int.TryParse(parts[1].Trim(), out int1) &&
               int.TryParse(parts[2].Trim(), out int2);
    }

    /// <summary>
    /// Parse format "string, int".
    /// Example: "fileName, 123" -> out: ("fileName", 123)
    /// </summary>
    public static bool TryParseStringInt(string arg, out string str, out int num)
    {
        str = null;
        num = 0;

        if (string.IsNullOrWhiteSpace(arg))
            return false;

        string[] parts = arg.Split(',');
        if (parts.Length != 2)
            return false;

        str = parts[0].Trim();
        return int.TryParse(parts[1].Trim(), out num);
    }

    // ==================== Regex Matching ====================

    /// <summary>
    /// Extract function name and argument from function call.
    /// Example: "HasSheet(5)" -> ("HasSheet", "5")
    /// </summary>
    public static bool TryExtractFunctionCall(string expr, out string funcName, out string argument)
    {
        funcName = argument = null;

        var match = Regex.Match(expr, @"(\w+)\s*\(\s*(.*?)\s*\)");
        if (!match.Success)
            return false;

        funcName = match.Groups[1].Value;
        argument = match.Groups[2].Value;
        return true;
    }

    /// <summary>
    /// Parse comparison expression.
    /// Example: "gameValue.currentYear == 2024" 
    /// -> out: ("gameValue", "currentYear", "==", 2024)
    /// </summary>
    public static bool TryParseComparison(string expr, out string objName, out string attrName, out string op, out int value)
    {
        objName = attrName = op = null;
        value = 0;

        string pattern = @"(\w+)\.(\w+)\s*(==|!=|<=|>=|<|>)\s*(-?\d+)";
        var match = Regex.Match(expr, pattern);

        if (!match.Success)
            return false;

        objName = match.Groups[1].Value;
        attrName = match.Groups[2].Value;
        op = match.Groups[3].Value;
        value = int.Parse(match.Groups[4].Value);

        return true;
    }

    /// <summary>
    /// Parse "character_name, number" format.
    /// Example: "Tom, 50" -> out: ("Tom", 50)
    /// Used for commands like AddFavorability, ChangeFavorabilityLevel.
    /// </summary>
    public static bool TryParseCharacterIntEffect(string arg, out string charName, out int value)
    {
        charName = null;
        value = 0;

        Regex regex = new Regex(@"(\w+),\s*(-?\d+)");
        Match match = regex.Match(arg);

        if (!match.Success)
            return false;

        charName = match.Groups[1].Value;
        value = int.Parse(match.Groups[2].Value);
        return true;
    }

    /// <summary>
    /// Parse "character_name, string_value" format.
    /// Example: "Tom, general" -> out: ("Tom", "general")
    /// Used for commands like CharacterTypeChange.
    /// </summary>
    public static bool TryParseCharacterStringEffect(string arg, out string charName, out string stringVal)
    {
        charName = stringVal = null;

        Regex regex = new Regex(@"^\s*(\w+)\s*,\s*(\w+)\s*$");
        Match match = regex.Match(arg);

        if (!match.Success)
            return false;

        charName = match.Groups[1].Value;
        stringVal = match.Groups[2].Value;
        return true;
    }

    /// <summary>
    /// Parse "region, country" format.
    /// Example: "region_A, country_B" -> out: ("region_A", "country_B")
    /// </summary>
    public static bool TryParseRegionCountry(string arg, out string region, out string country)
    {
        region = country = null;

        if (string.IsNullOrWhiteSpace(arg))
            return false;

        string[] parts = arg.Split(',');
        if (parts.Length != 2)
            return false;

        region = parts[0].Trim();
        country = parts[1].Trim();
        return true;
    }

    /// <summary>
    /// Parse "region, city_index, country" format.
    /// Example: "region_A, 1, country_B" -> out: ("region_A", 1, "country_B")
    /// </summary>
    public static bool TryParseCityCountry(string arg, out string region, out int cityIndex, out string country)
    {
        region = country = null;
        cityIndex = 0;

        if (string.IsNullOrWhiteSpace(arg))
            return false;

        string[] parts = arg.Split(',');
        if (parts.Length != 3)
            return false;

        region = parts[0].Trim();
        country = parts[2].Trim();

        return int.TryParse(parts[1].Trim(), out cityIndex);
    }

    // ==================== Logical Operations ====================

    /// <summary>
    /// Check if condition contains OR operator (||) and split by it.
    /// Returns true if split is successful, out array contains parts.
    /// </summary>
    public static bool TrySplitByOr(string condition, out string[] parts)
    {
        parts = null;

        if (!condition.Contains("||"))
            return false;

        parts = condition.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0;
    }

    /// <summary>
    /// Check if condition contains AND operator (&&) and split by it.
    /// Returns true if split is successful, out array contains parts.
    /// </summary>
    public static bool TrySplitByAnd(string condition, out string[] parts)
    {
        parts = null;

        if (!condition.Contains("&&"))
            return false;

        parts = condition.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0;
    }

    // ==================== Comparison Operations ====================

    /// <summary>
    /// Perform comparison operation between two integers.
    /// Operators: ==, !=, <, <=, >, >=
    /// </summary>
    public static bool Compare(int a, string op, int b)
    {
        return op switch
        {
            "==" => a == b,
            "!=" => a != b,
            "<" => a < b,
            "<=" => a <= b,
            ">" => a > b,
            ">=" => a >= b,
            _ => false
        };
    }

    // ==================== String Utilities ====================

    /// <summary>
    /// Remove quotes from string if present.
    /// Example: "hello" -> hello, 'world' -> world
    /// </summary>
    public static string UnquoteString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if ((str.StartsWith("\"") && str.EndsWith("\"")) ||
            (str.StartsWith("'") && str.EndsWith("'")))
        {
            return str.Substring(1, str.Length - 2);
        }

        return str;
    }

    /// <summary>
    /// Safely extract function argument from text.
    /// Example: Extract "5, 10" from "GoToSheet(5, 10)"
    /// </summary>
    public static bool TryExtractFunctionArgument(string text, string funcName, out string argument)
    {
        argument = null;

        string pattern = $@"{funcName}\s*\(\s*(.*?)\s*\)";
        var match = Regex.Match(text, pattern);

        if (!match.Success)
            return false;

        argument = match.Groups[1].Value;
        return true;
    }
}
