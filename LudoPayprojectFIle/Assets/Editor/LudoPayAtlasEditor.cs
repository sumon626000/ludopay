#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Shortcuts for editing the LudoPay NGUI atlas (ScriptableObject format, not legacy prefab).
/// </summary>
public static class LudoPayAtlasEditor
{
    const string AtlasAssetPath = "Assets/Atlases/LudoPay.asset";
    const string AtlasTexturePath = "Assets/Atlases/LudoPay.png";
    const string AtlasesFolder = "Assets/Atlases";

    [MenuItem("LudoPay/Open LudoPay Atlas for Editing", false, 0)]
    public static void OpenLudoPayAtlasForEditing()
    {
        var atlas = AssetDatabase.LoadAssetAtPath<NGUIAtlas>(AtlasAssetPath);
        if (atlas == null)
        {
            EditorUtility.DisplayDialog(
                "LudoPay Atlas",
                "Could not load:\n" + AtlasAssetPath + "\n\n" +
                "Expected NGUI atlas asset (NGUIAtlas .asset), not a .prefab.",
                "OK");
            return;
        }

        NGUISettings.atlas = atlas;
        Selection.activeObject = atlas;
        EditorGUIUtility.PingObject(atlas);
        EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true).Show();
    }

    [MenuItem("LudoPay/Fix Atlases Folder Read-Only", false, 1)]
    public static void ClearAtlasesFolderReadOnly()
    {
        var fullPath = Path.GetFullPath(AtlasesFolder);
        if (!Directory.Exists(fullPath))
        {
            EditorUtility.DisplayDialog("LudoPay Atlas", "Folder not found:\n" + fullPath, "OK");
            return;
        }

        int cleared = ClearReadOnlyRecursive(fullPath);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "LudoPay Atlas",
            "Cleared read-only flag on " + cleared + " file(s) under:\n" + AtlasesFolder,
            "OK");
    }

    [MenuItem("LudoPay/Atlas Editing Help (English + বাংলা)", false, 20)]
    public static void ShowAtlasHelp()
    {
        EditorUtility.DisplayDialog(
            "LudoPay NGUI Atlas — How to change images",
            BuildHelpMessage(),
            "OK");
    }

    [MenuItem("LudoPay/Check LudoPay Atlas Files", false, 2)]
    public static void CheckAtlasFiles()
    {
        var atlas = AssetDatabase.LoadAssetAtPath<NGUIAtlas>(AtlasAssetPath);
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AtlasTexturePath);
        var matPath = "Assets/Atlases/LudoPay.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

        string msg = "LudoPay.asset: " + (atlas != null ? "OK (" + atlas.spriteList.Count + " sprites)" : "MISSING") + "\n";
        msg += "LudoPay.png: " + (tex != null ? "OK (" + tex.width + "x" + tex.height + ")" : "MISSING — rebuild atlas or restore PNG") + "\n";
        msg += "LudoPay.mat: " + (mat != null ? "OK" : "MISSING") + "\n\n";
        msg += "UISprite on Facebook only picks sprite NAMES (e.g. facebookLogin).\n";
        msg += "To change the picture: use Atlas Maker (menu above).";

        EditorUtility.DisplayDialog("LudoPay Atlas Check", msg, "OK");
    }

    static string BuildHelpMessage()
    {
        return
            "WHY you cannot drag a new image onto UISprite:\n" +
            "NGUI uses one packed texture (LudoPay.png). Scene widgets only choose a slice name.\n\n" +
            "── Replace facebookLogin or logo (English) ──\n" +
            "1. Menu: LudoPay → Open LudoPay Atlas for Editing\n" +
            "2. Prepare PNG with transparency; name it exactly like the sprite (e.g. facebookLogin.png or ludo pay logo-02.png)\n" +
            "3. In Project, select that PNG (and others if needed)\n" +
            "4. In Atlas Maker: Atlas = LudoPay, check Keep existing sprites, click Add/Update\n" +
            "5. Save scene; enter Play mode to verify\n\n" +
            "Monster Game logo: Menu → LudoPay → Apply Monster Game Logo (All Pages)\n" +
            "Or use sprite name \"ludo pay logo-02\" in Atlas Maker.\n\n" +
            "── বাংলা (সহজ ধাপ) ──\n" +
            "১. LudoPay → Open LudoPay Atlas for Editing\n" +
            "২. নতুন PNG তৈরি করুন (পেছনে স্বচ্ছ), নাম হুবহু sprite নাম (যেমন facebookLogin.png)\n" +
            "৩. Project-এ PNG সিলেক্ট করুন\n" +
            "৪. Atlas Maker-এ Add/Update চাপুন — LudoPay.png আপডেট হবে\n" +
            "৫. Scene সেভ করুন\n\n" +
            "Inspector-এ Facebook-এ শুধু sprite নাম বদলায়; ছবি বদলাতে Atlas Maker লাগবে।\n" +
            "NGUI মেনু না থাকলে: Assets/Atlases/LudoPay.asset সিলেক্ট → Inspector → Atlas Maker";
    }

    static int ClearReadOnlyRecursive(string directory)
    {
        int count = 0;
        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            var attrs = File.GetAttributes(file);
            if ((attrs & FileAttributes.ReadOnly) != 0)
            {
                File.SetAttributes(file, attrs & ~FileAttributes.ReadOnly);
                count++;
            }
        }
        return count;
    }
}
#endif
