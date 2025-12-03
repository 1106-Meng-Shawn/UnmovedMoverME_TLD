using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class StoryCharacterImageDatabaseGenerator : EditorWindow
{
    private string sourcePath = "Assets/Resources/MyDraw/Characters";

    [MenuItem("Tools/Generate Story Character Database")]
    private static void ShowWindow()
    {
        GetWindow<StoryCharacterImageDatabaseGenerator>("Story DB Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Story Character Database", EditorStyles.boldLabel);
        sourcePath = EditorGUILayout.TextField("Source Path", sourcePath);

        if (GUILayout.Button("Generate All Databases"))
            GenerateAllDatabases();
    }

    private void GenerateAllDatabases()
    {
        if (!Directory.Exists(sourcePath))
        {
            Debug.LogError($"Source path not found: {sourcePath}");
            return;
        }

        foreach (var charFolder in Directory.GetDirectories(sourcePath))
            GenerateSingleCharacter(charFolder);

        AssetDatabase.Refresh();
        Debug.Log("All Character Databases Generated!");
    }

    private void GenerateSingleCharacter(string charFolder)
    {
        string charKey = Path.GetFileName(charFolder);
        var db = ScriptableObject.CreateInstance<StoryCharacterImageDataBase>();
        db.CharacterKey = charKey;

        // ??????
        string globalDefaultPath = Path.Combine(charFolder, $"{charKey}_Default.png");
        if (File.Exists(globalDefaultPath))db.DefaultSprite = LoadSprite(globalDefaultPath);
        string globalDefaultIconPath = Path.Combine(charFolder, $"{charKey}_DefaultIcon.png");
        if (File.Exists(globalDefaultIconPath)) db.DefaultIcon = LoadSprite(globalDefaultIconPath);


        foreach (var typeFolder in Directory.GetDirectories(charFolder))
        {
            string typeName = Path.GetFileName(typeFolder);

            string typeDefaultPath = Path.Combine(typeFolder, $"{typeName}_Default.png");
            Sprite defaultSprite = File.Exists(typeDefaultPath) ? LoadSprite(typeDefaultPath) : null;
            string typeDefaultIconPath = Path.Combine(typeFolder, $"{typeName}_DefaultIcon.png");
            Sprite defaultIconSprite = File.Exists(typeDefaultIconPath) ? LoadSprite(typeDefaultIconPath) : null;


            var charType = db.CreateCharacterType(typeName, defaultSprite, defaultIconSprite);

            foreach (var poseFolder in Directory.GetDirectories(typeFolder))
            {
                var pose = LoadPose(db, charType, poseFolder);
            }
        }

        SaveDatabaseAsset(db, charFolder, charKey);
    }

    private StoryCharacterImageDataBase.PoseData LoadPose(StoryCharacterImageDataBase db, StoryCharacterImageDataBase.CharacterImageSet charSet, string poseFolder)
    {
        string poseName = Path.GetFileName(poseFolder);
        string posePath = Path.Combine(poseFolder, $"{poseName}.png");
        Sprite poseSprite = File.Exists(posePath) ? LoadSprite(posePath) : null;

        string iconPath = Path.Combine(poseFolder, $"{poseName}Icon.png");
        Sprite poseIcon = File.Exists(iconPath) ? LoadSprite(iconPath) : null;

        var pose = db.CreatePose(charSet, poseName, poseSprite, poseIcon);

        // Expressions
        string expressionsPath = Path.Combine(poseFolder, "Expressions");
        if (Directory.Exists(expressionsPath))
        {
            foreach (var exprFile in Directory.GetFiles(expressionsPath, "*.png"))
            {
                string exprName = Path.GetFileNameWithoutExtension(exprFile);
                db.CreateExpression(pose, exprName, LoadSprite(exprFile));
            }
        }

        // Accessories
        string accPath = Path.Combine(poseFolder, "Accessories");
        if (Directory.Exists(accPath))
        {
            foreach (var accFolder in Directory.GetDirectories(accPath))
            {
                string accName = Path.GetFileName(accFolder);
                string front = Path.Combine(accFolder, $"{accName}Front.png");
                string back = Path.Combine(accFolder, $"{accName}Back.png");
                string main = Path.Combine(accFolder, $"{accName}.png");

                db.CreateAccessory(pose,
                    accName,
                    File.Exists(front) ? LoadSprite(front) : null,
                    File.Exists(back) ? LoadSprite(back) : null,
                    File.Exists(main) ? LoadSprite(main) : null
                );
            }
        }

        return pose;
    }

    private Sprite LoadSprite(string path)
    {
        string assetPath = ToAssetPath(path);
        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private void SaveDatabaseAsset(StoryCharacterImageDataBase db, string charFolder, string charKey)
    {
        string basePath = "Assets/ScriptableObject/CharacterImage";
        if (!Directory.Exists(basePath))
            Directory.CreateDirectory(basePath);

        string globalAssetPath = Path.Combine(basePath, $"{charKey}_DB.asset");
        globalAssetPath = ToAssetPath(globalAssetPath);

        if (File.Exists(globalAssetPath))
            AssetDatabase.DeleteAsset(globalAssetPath);

        AssetDatabase.CreateAsset(db, globalAssetPath);
        AssetDatabase.SaveAssets();
        string localAssetPath = Path.Combine(charFolder, $"{charKey}_DB.asset");
        localAssetPath = ToAssetPath(localAssetPath);
        var localCopy = ScriptableObject.Instantiate(db);
        AssetDatabase.CreateAsset(localCopy, localAssetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"? Created Character DB for {charKey}:\n  ? Global: {globalAssetPath}\n  ? Local: {localAssetPath}");
    }
    private string ToAssetPath(string path)
    {
        string cleanPath = path.Replace("\\", "/");
        if (cleanPath.StartsWith(Application.dataPath))
            cleanPath = "Assets" + cleanPath.Substring(Application.dataPath.Length);
        return cleanPath;
    }
}
