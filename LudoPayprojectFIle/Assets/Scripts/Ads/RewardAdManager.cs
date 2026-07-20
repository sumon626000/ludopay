using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// AdMob rewarded ads + admin-driven cooldown. Real ads require ENABLE_ADMOB scripting define
/// and Google Mobile Ads Unity plugin (see Assets/Docs/ADMOB_REWARD_SETUP.md).
/// GUEST_BUILD compiles without native ad SDK calls.
/// </summary>
public class RewardAdManager : MonoBehaviour
{
    public static RewardAdManager Instance { get; private set; }

    public const string PrefsLastRewardUtc = "AdMobRewardLastClaimUtc";

    [Header("Optional references")]
    public RoomManager roomManager;
    public ProfileManager profileManager;

    public RewardAdConfig Config { get; private set; } = new RewardAdConfig();
    public bool ConfigLoaded { get; private set; }
    public event Action OnConfigUpdated;
    public event Action OnAvailabilityChanged;

#if ENABLE_ADMOB && !GUEST_BUILD
    private GoogleMobileAds.Api.RewardedAd rewardedAd;
    private bool adLoading;
#endif

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RefreshConfigFromServer()
    {
        StartCoroutine(AppConfigClient.FetchAdMobConfig(OnConfigFetched, OnConfigFailed));
    }

    void OnConfigFetched(RewardAdConfig cfg)
    {
        Config = cfg;
        ConfigLoaded = true;
        OnConfigUpdated?.Invoke();
        OnAvailabilityChanged?.Invoke();
#if ENABLE_ADMOB && !GUEST_BUILD
        if (cfg.adsEnabled)
            PreloadRewarded();
#endif
    }

    void OnConfigFailed(string err)
    {
        Debug.LogWarning("RewardAdManager: config fetch failed — " + err);
        ConfigLoaded = true;
        OnConfigUpdated?.Invoke();
    }

    public bool IsAdsEnabled()
    {
        return ConfigLoaded && Config.adsEnabled;
    }

    public bool IsRewardAvailable()
    {
        if (!IsAdsEnabled())
            return false;
        if (Config.rewardCooldownHours <= 0f)
            return true;

        if (!PlayerPrefs.HasKey(PrefsLastRewardUtc))
            return true;

        if (!long.TryParse(PlayerPrefs.GetString(PrefsLastRewardUtc), out var lastTicks))
            return true;

        var last = new DateTime(lastTicks, DateTimeKind.Utc);
        var elapsed = DateTime.UtcNow - last;
        return elapsed.TotalHours >= Config.rewardCooldownHours;
    }

    public TimeSpan CooldownRemaining()
    {
        if (Config.rewardCooldownHours <= 0f || !PlayerPrefs.HasKey(PrefsLastRewardUtc))
            return TimeSpan.Zero;
        if (!long.TryParse(PlayerPrefs.GetString(PrefsLastRewardUtc), out var lastTicks))
            return TimeSpan.Zero;

        var last = new DateTime(lastTicks, DateTimeKind.Utc);
        var readyAt = last.AddHours(Config.rewardCooldownHours);
        var rem = readyAt - DateTime.UtcNow;
        return rem < TimeSpan.Zero ? TimeSpan.Zero : rem;
    }

    public string CooldownLabel()
    {
        var rem = CooldownRemaining();
        if (rem <= TimeSpan.Zero)
            return "";
        if (rem.TotalHours >= 1)
            return Mathf.CeilToInt((float)rem.TotalHours) + "h";
        return Mathf.CeilToInt((float)rem.TotalMinutes) + "m";
    }

    public void ShowRewarded(Action<bool> onFinished)
    {
        if (!IsAdsEnabled())
        {
            onFinished?.Invoke(false);
            return;
        }
        if (!IsRewardAvailable())
        {
            onFinished?.Invoke(false);
            return;
        }

#if ENABLE_ADMOB && !GUEST_BUILD
        ShowRewardedNative(onFinished);
#else
        Debug.LogWarning("RewardAdManager: ENABLE_ADMOB not defined — grant reward in Editor only.");
#if UNITY_EDITOR
        GrantReward(onFinished);
#else
        onFinished?.Invoke(false);
#endif
#endif
    }

#if ENABLE_ADMOB && !GUEST_BUILD
    void PreloadRewarded()
    {
        if (adLoading || rewardedAd != null)
            return;
        var unitId = Config.EffectiveRewardedUnitId;
        if (string.IsNullOrEmpty(unitId))
            return;

        adLoading = true;
        var request = new GoogleMobileAds.Api.AdRequest();
        GoogleMobileAds.Api.RewardedAd.Load(unitId, request, (ad, error) =>
        {
            adLoading = false;
            if (error != null || ad == null)
            {
                Debug.LogWarning("RewardAdManager: preload failed — " + error);
                rewardedAd = null;
                return;
            }
            rewardedAd = ad;
        });
    }

    void ShowRewardedNative(Action<bool> onFinished)
    {
        if (rewardedAd == null)
        {
            PreloadRewarded();
            onFinished?.Invoke(false);
            return;
        }

        bool earned = false;
        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            rewardedAd = null;
            PreloadRewarded();
            if (earned)
                GrantReward(onFinished);
            else
                onFinished?.Invoke(false);
        };

        rewardedAd.Show(reward =>
        {
            earned = true;
        });
    }
#endif

    void GrantReward(Action<bool> onFinished)
    {
        PlayerPrefs.SetString(PrefsLastRewardUtc, DateTime.UtcNow.Ticks.ToString());
        PlayerPrefs.Save();

        int before = GameManager.Instance != null ? GameManager.Instance.Points : 0;
        int amount = Config.rewardCoinsAmount;

        if (GameManager.Instance != null)
            GameManager.Instance.Points += amount;

        if (roomManager == null)
            roomManager = FindObjectOfType<RoomManager>();
        if (roomManager != null)
            roomManager.UpdateUserInfo();

        if (profileManager == null)
            profileManager = FindObjectOfType<ProfileManager>();
        if (profileManager != null && GameManager.Instance != null)
            profileManager.Counting(GameManager.Instance.Points, before);

        OnAvailabilityChanged?.Invoke();
        onFinished?.Invoke(true);
    }
}
