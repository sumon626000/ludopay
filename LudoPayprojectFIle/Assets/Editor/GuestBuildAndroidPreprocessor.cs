#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// When GUEST_BUILD is defined for Android, strip Firebase/Facebook Gradle deps and
/// disable the Firebase messaging Java activity so native libs are not bundled or loaded.
/// </summary>
public class GuestBuildAndroidPreprocessor : IPreprocessBuildWithReport
{
    const string GuestDefine = "GUEST_BUILD";
    const string PlayStoreDefine = "PLAY_STORE_BUILD";
    const string GradlePath = "Assets/Plugins/Android/mainTemplate.gradle";
    const string MessagingActivityPath = "Assets/Plugins/Android/MessagingUnityPlayerActivity.java";
    const string FirebaseAppLibPath = "Assets/Plugins/Android/FirebaseApp.androidlib";

    static readonly string[] GradleDepsToStrip = {
        "com.facebook.android:",
        "com.google.firebase:",
        "com.google.android.gms:play-services-base",
        "com.google.android.gms:play-services-ads",
        "com.google.android.gms:play-services-ads-lite",
        "com.google.flatbuffers:",
        "com.parse.bolts:",
    };

    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        if (!HasComplianceBuildDefine())
            return;

        StripFirebaseGradleDependencies();
        DisableMessagingActivityForAndroid();
        DisableFirebaseAppAndroidLib();
        Debug.Log("GuestBuildAndroidPreprocessor: GUEST_BUILD active — Firebase/Facebook native deps stripped for Android.");
    }

    static bool HasComplianceBuildDefine()
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        var parts = defines.Split(';').Select(d => d.Trim());
        return parts.Any(d => d == GuestDefine || d == PlayStoreDefine);
    }

    static bool HasGuestBuildDefine()
    {
        return HasComplianceBuildDefine();
    }

    static void StripFirebaseGradleDependencies()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), GradlePath);
        if (!File.Exists(path))
        {
            Debug.LogWarning("GuestBuildAndroidPreprocessor: mainTemplate.gradle not found.");
            return;
        }

        var lines = File.ReadAllLines(path);
        var changed = false;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (!line.TrimStart().StartsWith("implementation "))
                continue;

            if (GradleDepsToStrip.Any(dep => line.Contains(dep)))
            {
                if (!line.TrimStart().StartsWith("//"))
                {
                    lines[i] = "    // GUEST_BUILD: " + line.TrimStart();
                    changed = true;
                }
            }
        }

        if (changed)
            File.WriteAllLines(path, lines);
    }

    static void DisableMessagingActivityForAndroid()
    {
        if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), MessagingActivityPath)))
            return;

        var importer = AssetImporter.GetAtPath(MessagingActivityPath) as PluginImporter;
        if (importer == null)
            return;

        if (!importer.GetCompatibleWithPlatform(BuildTarget.Android))
            return;

        importer.SetCompatibleWithPlatform(BuildTarget.Android, false);
        importer.SaveAndReimport();
    }

    static void DisableFirebaseAppAndroidLib()
    {
        if (!AssetDatabase.IsValidFolder(FirebaseAppLibPath))
            return;

        var importer = AssetImporter.GetAtPath(FirebaseAppLibPath) as PluginImporter;
        if (importer == null)
            return;

        if (!importer.GetCompatibleWithPlatform(BuildTarget.Android))
            return;

        importer.SetCompatibleWithPlatform(BuildTarget.Android, false);
        importer.SaveAndReimport();
    }
}
#endif
