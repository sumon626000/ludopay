#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PlayStoreBuildTools
{
    const string GuestDefine = "GUEST_BUILD";
    const string PlayStoreDefine = "PLAY_STORE_BUILD";
    const string AdMobDefine = "ENABLE_ADMOB";
    const string ManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
    const string AdMobIdResourcePath = "Assets/Resources/PlayStore/admob_app_id.txt";

    [MenuItem("LudoPay/Apply Play Store Compliance (Fix All)", false, 0)]
    public static void ApplyPlayStoreCompliance()
    {
        SetAndroidDefines(GuestDefine, PlayStoreDefine);
        RemoveAndroidDefine(AdMobDefine);

        PlayerSettings.companyName = "Monster Game";
        PlayerSettings.productName = "Monster Game";
        PlayerSettings.Android.useCustomKeystore = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        EnsureAdMobIdFile();
        PatchAndroidManifest(includeAdMob: false, admobAppId: ReadAdMobAppId());

        DeleteLegacyManifest();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Play Store Compliance",
            "Applied Play Store fixes:\n\n" +
            "- GUEST_BUILD + PLAY_STORE_BUILD enabled\n" +
            "- Real-money wallet/withdraw/shop pay disabled in app\n" +
            "- Cleartext HTTP disabled in manifest\n" +
            "- Test AdMob ID removed (add real ID in Resources/PlayStore/admob_app_id.txt if needed)\n" +
            "- App name set to Monster Game\n" +
            "- Legacy camera manifest removed\n\n" +
            "Next: Build → Build App Bundle (Android)\n" +
            "Play Console: add Privacy Policy URL + Data safety form.",
            "OK");
    }

    [MenuItem("LudoPay/Play Store Help", false, 21)]
    public static void ShowHelp()
    {
        EditorUtility.DisplayDialog(
            "Play Store Upload Checklist",
            "Before upload:\n" +
            "1. Run: LudoPay → Apply Play Store Compliance\n" +
            "2. Build AAB (Release)\n" +
            "3. Play Console → Privacy policy URL\n" +
            "4. Play Console → Data safety (phone, email, photos)\n" +
            "5. Store listing name: Monster Game\n\n" +
            "AdMob (optional):\n" +
            "- Put real App ID in Resources/PlayStore/admob_app_id.txt\n" +
            "- Add ENABLE_ADMOB to Android Scripting Defines\n" +
            "- Re-run compliance menu",
            "OK");
    }

    static void SetAndroidDefines(params string[] required)
    {
        var group = BuildTargetGroup.Android;
        var set = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
            .Split(';')
            .Select(d => d.Trim())
            .Where(d => d.Length > 0)
            .ToList();

        foreach (var d in required)
        {
            if (!set.Contains(d))
                set.Add(d);
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", set));
    }

    static void RemoveAndroidDefine(string define)
    {
        var group = BuildTargetGroup.Android;
        var set = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
            .Split(';')
            .Select(d => d.Trim())
            .Where(d => d.Length > 0 && d != define)
            .ToList();
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", set));
    }

    static void EnsureAdMobIdFile()
    {
        var dir = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), AdMobIdResourcePath));
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var full = Path.Combine(Directory.GetCurrentDirectory(), AdMobIdResourcePath);
        if (!File.Exists(full))
        {
            File.WriteAllText(full,
                "# Replace with your real AdMob App ID from Google AdMob console\n" +
                "# Example: ca-app-pub-1234567890123456~0987654321\n");
        }
    }

    static string ReadAdMobAppId()
    {
        var full = Path.Combine(Directory.GetCurrentDirectory(), AdMobIdResourcePath);
        if (!File.Exists(full))
            return "";

        foreach (var line in File.ReadAllLines(full))
        {
            var t = line.Trim();
            if (t.Length == 0 || t.StartsWith("#"))
                continue;
            if (t.StartsWith("ca-app-pub-"))
                return t;
        }

        return "";
    }

    public static void PatchAndroidManifest(bool includeAdMob, string admobAppId)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), ManifestPath);
        if (!File.Exists(path))
        {
            Debug.LogWarning("PlayStoreBuildTools: AndroidManifest not found.");
            return;
        }

        var text = File.ReadAllText(path);
        text = text.Replace("android:usesCleartextTraffic=\"true\"", "android:usesCleartextTraffic=\"false\"");

        const string admobMeta =
            "<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"ca-app-pub-3940256099942544~3347511713\" />";

        if (text.Contains(admobMeta))
            text = text.Replace(admobMeta, "");

        if (includeAdMob && !string.IsNullOrWhiteSpace(admobAppId))
        {
            var insert =
                "    <meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"" +
                admobAppId.Trim() + "\" />\n";
            text = text.Replace("<application ", insert + "  <application ");
        }

        File.WriteAllText(path, text);
    }

    static void DeleteLegacyManifest()
    {
        var legacy = "Assets/Plugins/AndroidManifest.xml";
        if (AssetDatabase.LoadAssetAtPath<Object>(legacy) != null)
        {
            AssetDatabase.DeleteAsset(legacy);
            Debug.Log("PlayStoreBuildTools: removed legacy Assets/Plugins/AndroidManifest.xml");
        }
    }
}
#endif
