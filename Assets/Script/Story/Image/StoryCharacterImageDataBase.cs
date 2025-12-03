using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StoryCharacterImageDataBase
/// 
/// Parses layered visual state:
/// CharacterType ? Pose ? Expression ? Accessories
/// 
/// - Accessories stored under Pose/Accessories/
/// - Automatically detects _Front / _Back variants
/// - Output sprite order respects input accessory order
/// 
/// Example input:
///     "AliceGood_Stand_Smile_Hat_Glasses"
/// Result order:
///     HatFront ? GlassesFront ? Smile ? Stand ? HatBack
/// </summary>
/// 

public static class PoseConstants
{
    public const string Stand = "Stand";
}

[CreateAssetMenu(fileName = "StoryCharacterImageDataBase", menuName = "Story/Character Image Database")]
public class StoryCharacterImageDataBase : ScriptableObject
{
    [Header("Global fallback")]
    public string CharacterKey;
    public Sprite DefaultSprite;
    public Sprite DefaultIcon;
    public List<CharacterImageSet> characterTypes = new List<CharacterImageSet>();

    [System.Serializable]
    public class CharacterImageSet
    {
        public string characterType;
        public Sprite DefaultSprite;
        public Sprite DefaultIcon;
        public List<PoseData> poses = new List<PoseData>();
    }

    [System.Serializable]
    public class PoseData
    {
        public string poseName;
        public Sprite poseIcon;
        public Sprite poseSprite;
        public List<ExpressionData> expressions = new List<ExpressionData>();
        public List<AccessoryData> accessories = new List<AccessoryData>();
    }

    [System.Serializable]
    public class ExpressionData
    {
        public string expressionName;
        public Sprite expressionSprite;
    }

    [System.Serializable]
    public class AccessoryData
    {
        public string accessoryName;
        public Sprite frontSprite;
        public Sprite backSprite;
        public Sprite mainSprite;
    }

    [System.Serializable]
    public class SpriteLayerInfo
    {
        public Sprite sprite;
        public string tag;
        public int order;
    }

    // ============================================================
    // === PUBLIC API ===
    // ============================================================

    /// <summary>
    /// ? “Alice_Stand_Sad_Glasses_Hat” ??????
    /// ?? "!Glasses" ????
    /// </summary>
    public List<SpriteLayerInfo> GetSpritesByCode(string state)
    {
        if (string.IsNullOrEmpty(state))
            return DefaultLayer();

        // Example: "AliceGood_Stand_Smile_Hat_Glasses"
        string[] parts = state.Split('_');
        if (parts.Length < 3)
            return DefaultLayer();

        string characterType = parts[0];
        string poseName = parts[1];
        string expressionName = parts[2];

        var excludedAccessories = new HashSet<string>();
        var accessories = new List<string>();

        for (int i = 3; i < parts.Length; i++)
        {
            string acc = parts[i];
            if (acc.StartsWith("!"))
                excludedAccessories.Add(acc.Substring(1));
            else
                accessories.Add(acc);
        }

        // === 1. ?? ===
        var charSet = characterTypes.Find(c => c.characterType == characterType);
        if (charSet == null)
        {
            Debug.LogWarning($"Character type not found: {characterType}");
            return DefaultLayer();
        }

        // === 2. ?? ===
        var pose = charSet.poses.Find(p => p.poseName == poseName);
        if (pose == null)
        {
            Debug.LogWarning($"Pose not found: {poseName}");
            return CharacterDefaultLayer(charSet);
        }

        // === 3. ?? ===
        var expr = pose.expressions.Find(e => e.expressionName == expressionName);
        if (expr == null)
        {
            Debug.LogWarning($"Expression not found: {expressionName}");
        }

        // === 4. ???? ===
        var layers = new List<SpriteLayerInfo>();

        // ???
        foreach (var accName in accessories)
        {
            if (excludedAccessories.Contains(accName))
                continue;

            var acc = pose.accessories.Find(a => a.accessoryName == accName);
            if (acc == null) continue;

            if (acc.frontSprite != null)
                layers.Add(new SpriteLayerInfo { sprite = acc.frontSprite, tag = accName + "Front", order = layers.Count });
            else if (acc.mainSprite != null && acc.backSprite == null)
                layers.Add(new SpriteLayerInfo { sprite = acc.mainSprite, tag = accName + "Main", order = layers.Count });
        }

        // ??
        if (expr?.expressionSprite != null)
            layers.Add(new SpriteLayerInfo { sprite = expr.expressionSprite, tag = "Expression", order = layers.Count });

        // ????
        if (pose.poseSprite != null)
            layers.Add(new SpriteLayerInfo { sprite = pose.poseSprite, tag = "Pose", order = layers.Count });

        // ????
        for (int i = accessories.Count - 1; i >= 0; i--)
        {
            var accName = accessories[i];
            if (excludedAccessories.Contains(accName))
                continue;

            var acc = pose.accessories.Find(a => a.accessoryName == accName);
            if (acc?.backSprite != null)
                layers.Add(new SpriteLayerInfo { sprite = acc.backSprite, tag = accName + "Back", order = layers.Count });
        }

        return layers;
    }
    // ==========================================
    // ? ???????? Generator ???
    // ==========================================
    public CharacterImageSet CreateCharacterType(string typeName, Sprite defaultSprite, Sprite defaultIcon)
    {
        var set = new CharacterImageSet
        {
            characterType = typeName,
            DefaultSprite = defaultSprite,
            DefaultIcon = defaultIcon,
            poses = new List<PoseData>()
        };
        characterTypes.Add(set);
        return set;
    }

    public PoseData CreatePose(CharacterImageSet charSet, string poseName, Sprite poseSprite, Sprite poseIcon)
    {
        var pose = new PoseData
        {
            poseName = poseName,
            poseSprite = poseSprite,
            poseIcon = poseIcon,
            expressions = new List<ExpressionData>(),
            accessories = new List<AccessoryData>()
        };
        charSet.poses.Add(pose);
        return pose;
    }

    public ExpressionData CreateExpression(PoseData pose, string exprName, Sprite exprSprite)
    {
        var expr = new ExpressionData
        {
            expressionName = exprName,
            expressionSprite = exprSprite
        };
        pose.expressions.Add(expr);
        return expr;
    }

    public AccessoryData CreateAccessory(PoseData pose, string accName, Sprite front = null, Sprite back = null, Sprite main = null)
    {
        var acc = new AccessoryData
        {
            accessoryName = accName,
            frontSprite = front,
            backSprite = back,
            mainSprite = main
        };
        pose.accessories.Add(acc);
        return acc;
    }



    // === Helper Layers ===
    private List<SpriteLayerInfo> DefaultLayer()
    {
        return new List<SpriteLayerInfo> {
            new SpriteLayerInfo { sprite = DefaultSprite, tag = "GlobalDefault", order = 0 }
        };
    }

    private List<SpriteLayerInfo> CharacterDefaultLayer(CharacterImageSet set)
    {
        return new List<SpriteLayerInfo> {
            new SpriteLayerInfo { sprite = set.DefaultSprite ?? DefaultSprite, tag = "CharacterDefault", order = 0 }
        };
    }

    // ==========================================
    // ? ???? Stand ?/??
    // ==========================================
    public Sprite GetTypeDefaultStandImage(string type)
    {
        var set = characterTypes.Find(s => s.characterType == type);
        if (set != null)
        {
            var standPose = set.poses.Find(p => string.Equals(p.poseName, PoseConstants.Stand, StringComparison.OrdinalIgnoreCase));
            if (standPose != null && standPose.poseSprite != null)
                return standPose.poseSprite;
            if (set.DefaultSprite != null)
                return set.DefaultSprite;
        }
        return DefaultSprite;
    }

    public Sprite GetTypeDefaultStandIcon(string type)
    {
        var set = characterTypes.Find(s => s.characterType == type);
        if (set != null)
        {
            var standPose = set.poses.Find(p => string.Equals(p.poseName, PoseConstants.Stand, StringComparison.OrdinalIgnoreCase));
            if (standPose != null && standPose.poseIcon != null)return standPose.poseIcon;
            if (set.DefaultIcon != null)return set.DefaultIcon;
        }
        return DefaultIcon ?? null;
    }

    public Sprite GetIcon(string type,string pose)
    {
        var set = characterTypes.Find(s => s.characterType == type);

        if (set != null)
        {
            var Pose = set.poses.Find(p => string.Equals(p.poseName, pose, StringComparison.OrdinalIgnoreCase));
            if (Pose != null && Pose.poseIcon != null) return Pose.poseIcon;
            if (set.DefaultIcon != null) return set.DefaultIcon;
        }
        return DefaultIcon ?? null;
    }

}
