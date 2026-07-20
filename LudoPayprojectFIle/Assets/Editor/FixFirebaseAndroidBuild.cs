#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class FixFirebaseAndroidBuild : IPreprocessBuildWithReport
{
    const string AssetPath = "Assets/Plugins/Android/MessagingUnityPlayerActivity.java";
    const string FixedMarker = "//mUnityPlayer.quit(); // Replaced as it causes a build error";
    const string PreserveLabel = "FirebasePreserve";

    public int callbackOrder { get { return 1; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        var fileToFix = Path.Combine(Directory.GetCurrentDirectory(), AssetPath);
        if (!File.Exists(fileToFix))
            return;

        var content = File.ReadAllText(fileToFix);
        if (content.Contains(FixedMarker))
        {
            EnsurePreserveLabel();
            return;
        }

        if (!content.Contains("mUnityPlayer.quit();"))
            return;

        content = content.Replace("mUnityPlayer.quit();", FixedMarker);
        File.WriteAllText(fileToFix, content);
        EnsurePreserveLabel();
        Debug.Log($"FixFirebaseAndroidBuild : Fixed file \"{fileToFix}\"");
    }

    static void EnsurePreserveLabel()
    {
        var asset = AssetDatabase.LoadMainAssetAtPath(AssetPath);
        if (asset == null)
            return;

        var labels = AssetDatabase.GetLabels(asset);
        if (labels.Contains(PreserveLabel))
            return;

        AssetDatabase.SetLabels(asset, labels.Concat(new[] { PreserveLabel }).ToArray());
    }
}
#endif