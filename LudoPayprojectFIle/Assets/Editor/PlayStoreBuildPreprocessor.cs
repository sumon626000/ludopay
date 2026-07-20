#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PlayStoreBuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        var isPlayStore = defines.Contains("PLAY_STORE_BUILD");
        var hasAdMob = defines.Contains("ENABLE_ADMOB");

        PlayStoreBuildTools.PatchAndroidManifest(
            includeAdMob: hasAdMob && isPlayStore,
            admobAppId: ReadAdMobAppIdFromResources());

        if (isPlayStore)
            Debug.Log("PlayStoreBuildPreprocessor: Play Store manifest + compliance active.");
    }

    static string ReadAdMobAppIdFromResources()
    {
        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/PlayStore/admob_app_id.txt");
        if (asset == null)
            return "";

        foreach (var line in asset.text.Split('\n'))
        {
            var t = line.Trim();
            if (t.Length == 0 || t.StartsWith("#"))
                continue;
            if (t.StartsWith("ca-app-pub-"))
                return t;
        }

        return "";
    }
}
#endif
