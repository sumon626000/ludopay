#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class RewardButtonPrefabCreator
{
    const string PrefabPath = "Assets/Prefabs/UI/RewardButton.prefab";

    [MenuItem("LudoPay/Create Reward Ad Button Prefab")]
    public static void CreatePrefab()
    {
        var dir = Path.GetDirectoryName(PrefabPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var root = new GameObject("RewardButton", typeof(BoxCollider), typeof(UIButton), typeof(UISprite), typeof(RewardButtonUI));
        var sprite = root.GetComponent<UISprite>();
        sprite.width = 96;
        sprite.height = 96;
        sprite.type = UIBasicSprite.Type.Simple;
        sprite.color = new Color(1f, 0.82f, 0.2f, 1f);

        var labelGo = new GameObject("Label", typeof(UILabel));
        labelGo.transform.SetParent(root.transform, false);
        var label = labelGo.GetComponent<UILabel>();
        label.text = "AD";
        label.fontSize = 22;
        label.width = 80;
        label.height = 40;
        label.alignment = NGUIText.Alignment.Center;
        labelGo.transform.localPosition = Vector3.zero;

        var cooldownGo = new GameObject("Cooldown", typeof(UILabel));
        cooldownGo.transform.SetParent(root.transform, false);
        var cd = cooldownGo.GetComponent<UILabel>();
        cd.text = "";
        cd.fontSize = 14;
        cd.width = 80;
        cd.height = 24;
        cd.alignment = NGUIText.Alignment.Center;
        cooldownGo.transform.localPosition = new Vector3(0, -36f, 0);

        var ui = root.GetComponent<RewardButtonUI>();
        ui.rewardButton = root.GetComponent<UIButton>();
        ui.cooldownLabel = cd;
        ui.coinsHintLabel = label;

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);
        Selection.activeObject = prefab;
        Debug.Log("Created " + PrefabPath + " — drag onto Home panel in MenuScene.");
    }
}
#endif
