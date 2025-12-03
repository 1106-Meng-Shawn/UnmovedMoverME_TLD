using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Contains sprite and its metadata
/// </summary>
public class SpriteInfo
{
    public Sprite Sprite;
    public string Category;   // Background or CG
    public string Character;  // Character name (empty for Background)
    public string Scene;      // Scene/pose (Background can have Scene)
    public int Diff;          // Variation index (0 for Background)
    public string FullPath;   // Full Addressables key
}


public static class SpriteLoadHelperConstant
{
    public const string Background = "MyDraw/Background";
    public const string CG = "MyDraw/CG";

}


/// <summary>
/// Helper for loading sprites from Addressables, with automatic parsing of path
/// Background and CG are handled differently
/// </summary>
public static class SpriteLoadHelper
{
    // Root folders to search
    private static readonly string[] Roots = { $"{SpriteLoadHelperConstant.Background}/", $"{SpriteLoadHelperConstant.CG}/" };

    // Cache to avoid repeated Addressables loads
    private static readonly Dictionary<string, SpriteInfo> Cache = new();

    /// <summary>
    /// Load sprite by name from Addressables
    /// Excel only needs to provide spriteName (file name)
    /// Returns SpriteInfo with parsed metadata
    /// </summary>
    public static async Task<SpriteInfo> LoadAsync(string spriteName)
    {
        if (string.IsNullOrWhiteSpace(spriteName)) return null;

        // Return cached result if available
        if (Cache.TryGetValue(spriteName, out var cached))
            return cached;

        foreach (var root in Roots)
        {
            string key;

            if (root.StartsWith(SpriteLoadHelperConstant.Background))
            {
                // 自动处理 Background：去掉最后一个 '_' 取子文件夹
                int lastUnderscore = spriteName.LastIndexOf('_');
                if (lastUnderscore > 0)
                {
                    string folderName = spriteName.Substring(0, lastUnderscore);
                    key = $"{root}{folderName}/{spriteName}";
                }
                else
                {
                    key = root + spriteName;
                }
            }
            else
            {
                // CG 或其他直接拼接
                key = root + spriteName;
            }

            var handle = Addressables.LoadAssetAsync<Sprite>(key);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var sprite = handle.Result;
                var info = ParsePath(key, sprite);
                Cache[spriteName] = info;
                return info;
            }
        }

        Debug.LogError($"Sprite not found in Background/ or CG/: {spriteName}");
        return null;
    }

    /// <summary>
    /// Parse Addressables key to generate SpriteInfo
    /// Background: Character = "", Diff = 0, Scene = folder name
    /// CG: parses Character / Scene / Diff from path
    /// </summary>
    private static SpriteInfo ParsePath(string key, Sprite sprite)
    {
        var parts = key.Split('/');

        string category = parts.Length > 0 ? parts[0] : "";

        var info = new SpriteInfo
        {
            Sprite = sprite,
            FullPath = key,
            Category = category
        };

        if (category.Equals($"{SpriteLoadHelperConstant.Background}", System.StringComparison.OrdinalIgnoreCase))
        {
            // Background has no character, Diff = 0
            info.Character = "";
            info.Diff = 0;

            // Scene = folder under Background
            if (parts.Length > 1)
                info.Scene = parts[1];
            else
                info.Scene = sprite.name; // fallback to file name
        }
        else if (category.Equals($"{SpriteLoadHelperConstant.CG}", System.StringComparison.OrdinalIgnoreCase))
        {
            // CG parses Character / Scene / Diff
            info.Character = parts.Length > 1 ? parts[1] : "";
            info.Scene = parts.Length > 2 ? parts[2] : "";
            info.Diff = parts.Length > 3 ? ParseDiff(parts[3]) : 0;
        }
        else
        {
            // Fallback for unknown categories
            info.Character = "";
            info.Scene = sprite.name;
            info.Diff = 0;
        }

        return info;
    }

    /// <summary>
    /// Parse the variation index from file name, e.g., Alice_Happy_0 -> 0
    /// </summary>
    private static int ParseDiff(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return 0;

        var parts = fileName.Split('_');
        if (parts.Length == 0) return 0;

        if (int.TryParse(parts[parts.Length - 1], out int diff))
            return diff;

        return 0;
    }
}
