using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Fetches GET {baseUrl}/mobile/config from Monster Game / LudoPay admin API.
/// Set base URL via PlayerPrefs key AdminApiBaseUrl (e.g. http://host:4000/api).
/// </summary>
public static class AppConfigClient
{
    public const string PrefsAdminApiBaseUrl = "AdminApiBaseUrl";
    public const string DefaultBaseUrl = "https://admin.monstergame.app/api";

    public static string GetBaseUrl()
    {
        return PlayerPrefs.GetString(PrefsAdminApiBaseUrl, DefaultBaseUrl).TrimEnd('/');
    }

    public static void SetBaseUrl(string url)
    {
        PlayerPrefs.SetString(PrefsAdminApiBaseUrl, (url ?? DefaultBaseUrl).TrimEnd('/'));
        PlayerPrefs.Save();
    }

    public static IEnumerator FetchAdMobConfig(Action<RewardAdConfig> onSuccess, Action<string> onError)
    {
        var url = GetBaseUrl() + "/mobile/config";
        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = 12;
            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                onError?.Invoke(req.error ?? "Config request failed");
                yield break;
            }

            try
            {
                var wrapper = JsonUtility.FromJson<AppConfigApiResponse>(req.downloadHandler.text);
                if (wrapper == null || !wrapper.success || wrapper.data?.admob == null)
                {
                    onError?.Invoke("Invalid config response");
                    yield break;
                }

                var a = wrapper.data.admob;
                onSuccess?.Invoke(new RewardAdConfig
                {
                    adsEnabled = a.adsEnabled,
                    testMode = a.testMode,
                    appId = a.appId ?? "",
                    rewardedAdUnitId = a.rewardedAdUnitId ?? "",
                    rewardedAdUnitIdAndroid = a.rewardedAdUnitIdAndroid ?? "",
                    rewardCoinsAmount = a.rewardCoinsAmount > 0 ? a.rewardCoinsAmount : 300,
                    rewardCooldownHours = a.rewardCooldownHours >= 0 ? a.rewardCooldownHours : 24f,
                });
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }
    }
}
