#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Replaces Monster Game logo everywhere: all UI pages, atlases, splash, and app icons.
/// </summary>
public static class LudoPayLogoReplacer
{
    const string SourceLogoPath = "Assets/Textures/Logo/MonsterGameLogo_Source.png";
    const string TransparentLogoPath = "Assets/Textures/Logo/MonsterGameLogo_Transparent.png";
    const string LudoPayAtlasPath = "Assets/Atlases/LudoPay.asset";
    const string GameplayAtlasPath = "Assets/Atlases/LudoGamePlay1.asset";
    const string InGameLogoPath = "Assets/LudoPaySprites/ludo pay logo-02.png";
    const string InGameLogoAltPath = "Assets/LudoPaySprites/ludo pay logo-01.png";
    const string SplashLogoPath = "Assets/LudoPaySprites/CompanyLogo.png";
    const string Logo512Path = "Assets/Textures/Logo/MonsterGameLogo_512.png";
    const string Logo256Path = "Assets/Textures/Logo/MonsterGameLogo_256.png";
    const string EpicSplashPath = "Assets/EpicVictoryEffects/Textures/Logo_SplashScreen_01.png";
    const string InGameSpriteName = "ludo pay logo-02";
    const string GameplaySpriteName = "logo";

    [MenuItem("LudoPay/Apply Monster Game Logo (All Pages)", false, 5)]
    public static void ApplyMonsterGameLogoAllPages()
    {
        var source = AssetDatabase.LoadAssetAtPath<Texture2D>(TransparentLogoPath);
        if (source == null)
            source = AssetDatabase.LoadAssetAtPath<Texture2D>(SourceLogoPath);
        if (source == null)
        {
            EditorUtility.DisplayDialog(
                "Monster Game Logo",
                "Source logo not found:\n" + SourceLogoPath,
                "OK");
            return;
        }

        CopyLogoToAllTargets(source);
        AssetDatabase.Refresh();

        var inGameLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(InGameLogoPath);
        var ludoPayAtlas = AssetDatabase.LoadAssetAtPath<NGUIAtlas>(LudoPayAtlasPath);
        var gameplayAtlas = AssetDatabase.LoadAssetAtPath<NGUIAtlas>(GameplayAtlasPath);

        if (ludoPayAtlas != null && inGameLogo != null)
        {
            NGUISettings.atlas = ludoPayAtlas;
            UIAtlasMaker.AddOrUpdate(ludoPayAtlas, inGameLogo);
        }

        if (gameplayAtlas != null && inGameLogo != null)
        {
            NGUISettings.atlas = gameplayAtlas;
            UIAtlasMaker.AddOrUpdate(gameplayAtlas, inGameLogo);
        }

        ApplyPlayerIconsAndSplash();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Monster Game Logo",
            "Logo replaced everywhere:\n\n" +
            "- MenuScene (3 logos)\n" +
            "- GameScene (1 logo)\n" +
            "- LudoPay atlas + LudoGamePlay1 atlas\n" +
            "- Splash screen (CompanyLogo)\n" +
            "- Android splash + app icon\n" +
            "- All logo PNG source files\n\n" +
            "Open MenuScene and press Play to verify.",
            "OK");
    }

    [MenuItem("LudoPay/Set Selected Texture As Monster Game Logo", false, 6)]
    public static void SetSelectedTextureAsLogo()
    {
        var selected = Selection.activeObject as Texture2D;
        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Monster Game Logo",
                "Select a PNG texture in the Project window first.",
                "OK");
            return;
        }

        var sourcePath = AssetDatabase.GetAssetPath(selected);
        File.Copy(
            Path.GetFullPath(sourcePath),
            Path.GetFullPath(SourceLogoPath),
            true);
        AssetDatabase.ImportAsset(SourceLogoPath, ImportAssetOptions.ForceUpdate);
        ApplyMonsterGameLogoAllPages();
    }

    static void CopyLogoToAllTargets(Texture2D source)
    {
        var sourcePath = AssetDatabase.GetAssetPath(source);
        CopyFile(sourcePath, InGameLogoPath);
        CopyFile(sourcePath, InGameLogoAltPath);
        CopyFile(sourcePath, SplashLogoPath);
        CopyFile(sourcePath, Logo512Path);
        CopyFile(sourcePath, Logo256Path);
        CopyFile(sourcePath, EpicSplashPath);
    }

    static void CopyFile(string fromAssetPath, string toAssetPath)
    {
        var from = Path.GetFullPath(fromAssetPath);
        var to = Path.GetFullPath(toAssetPath);
        Directory.CreateDirectory(Path.GetDirectoryName(to));
        File.Copy(from, to, true);
        AssetDatabase.ImportAsset(toAssetPath, ImportAssetOptions.ForceUpdate);
    }

    static void ApplyPlayerIconsAndSplash()
    {
        var icon512 = AssetDatabase.LoadAssetAtPath<Texture2D>(Logo512Path);
        var splashLogo = LoadSprite(SplashLogoPath);
        if (icon512 == null)
            return;

        var androidSizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
        if (androidSizes != null && androidSizes.Length > 0)
        {
            var androidIcons = new Texture2D[androidSizes.Length];
            for (var i = 0; i < androidSizes.Length; i++)
                androidIcons[i] = icon512;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, androidIcons);
        }

        var standaloneSizes = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Standalone);
        if (standaloneSizes != null && standaloneSizes.Length > 0)
        {
            var standaloneIcons = new Texture2D[standaloneSizes.Length];
            for (var i = 0; i < standaloneSizes.Length; i++)
                standaloneIcons[i] = icon512;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, standaloneIcons);
        }

        if (splashLogo != null)
        {
            PlayerSettings.SplashScreen.logos = new[]
            {
                new PlayerSettings.SplashScreenLogo
                {
                    logo = splashLogo,
                    duration = 3f
                }
            };
        }
    }

    static Sprite LoadSprite(string assetPath)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (sprite != null)
            return sprite;

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (texture == null)
            return null;

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }
}
#endif
