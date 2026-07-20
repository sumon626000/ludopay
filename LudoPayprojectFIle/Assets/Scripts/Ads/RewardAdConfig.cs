using System;

[Serializable]
public class RewardAdConfig
{
    public bool adsEnabled = true;
    public bool testMode = true;
    public string appId = "";
    public string rewardedAdUnitId = "";
    public string rewardedAdUnitIdAndroid = "";
    public int rewardCoinsAmount = 300;
    public float rewardCooldownHours = 24f;

    public string EffectiveRewardedUnitId =>
        !string.IsNullOrEmpty(rewardedAdUnitIdAndroid) ? rewardedAdUnitIdAndroid : rewardedAdUnitId;
}

[Serializable]
public class AppConfigApiResponse
{
    public bool success;
    public AppConfigData data;
}

[Serializable]
public class AppConfigData
{
    public AdMobConfigPayload admob;
}

[Serializable]
public class AdMobConfigPayload
{
    public bool adsEnabled;
    public bool testMode;
    public string appId;
    public string rewardedAdUnitId;
    public string rewardedAdUnitIdAndroid;
    public int rewardCoinsAmount;
    public float rewardCooldownHours;
}
