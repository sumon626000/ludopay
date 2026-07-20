using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to a home/menu NGUI reward button. Wires UIButton click and cooldown label.
/// </summary>
public class RewardButtonUI : MonoBehaviour
{
    public UIButton rewardButton;
    public UILabel cooldownLabel;
    public UILabel coinsHintLabel;
    public GameObject lockedOverlay;
    public MenuManager menuManager;
    public string notReadyMessage = "Reward not available yet.";
    public string adsDisabledMessage = "Ads are disabled.";

    RewardAdManager manager;
    Coroutine tickRoutine;

    void Start()
    {
        manager = RewardAdManager.Instance;
        if (manager == null)
        {
            var go = new GameObject("RewardAdManager");
            manager = go.AddComponent<RewardAdManager>();
        }

        if (rewardButton == null)
            rewardButton = GetComponent<UIButton>();

        if (menuManager == null)
            menuManager = FindObjectOfType<MenuManager>();

        EventDelegate.Add(rewardButton.onClick, OnClickReward);

        manager.OnConfigUpdated += RefreshUi;
        manager.OnAvailabilityChanged += RefreshUi;

        if (!manager.ConfigLoaded)
            manager.RefreshConfigFromServer();
        else
            RefreshUi();

        tickRoutine = StartCoroutine(TickCooldown());
    }

    void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnConfigUpdated -= RefreshUi;
            manager.OnAvailabilityChanged -= RefreshUi;
        }
        if (tickRoutine != null)
            StopCoroutine(tickRoutine);
    }

    IEnumerator TickCooldown()
    {
        var wait = new WaitForSeconds(30f);
        while (true)
        {
            RefreshUi();
            yield return wait;
        }
    }

    void RefreshUi()
    {
        bool enabled = manager != null && manager.IsAdsEnabled();
        bool available = enabled && manager.IsRewardAvailable();

        if (rewardButton != null)
            rewardButton.isEnabled = available;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(enabled && !available);

        if (cooldownLabel != null)
        {
            var rem = manager != null ? manager.CooldownLabel() : "";
            cooldownLabel.text = string.IsNullOrEmpty(rem) ? "" : rem;
            cooldownLabel.gameObject.SetActive(!string.IsNullOrEmpty(rem));
        }

        if (coinsHintLabel != null && manager != null && manager.ConfigLoaded)
            coinsHintLabel.text = "+" + manager.Config.rewardCoinsAmount;
    }

    void OnClickReward()
    {
        if (manager == null)
            return;

        if (!manager.IsAdsEnabled())
        {
            ShowMessage(adsDisabledMessage);
            return;
        }
        if (!manager.IsRewardAvailable())
        {
            var rem = manager.CooldownRemaining();
            ShowMessage(notReadyMessage + (rem > System.TimeSpan.Zero ? "\r\n" + manager.CooldownLabel() : ""));
            return;
        }

        if (rewardButton != null)
            rewardButton.isEnabled = false;

        manager.ShowRewarded(success =>
        {
            RefreshUi();
            if (success)
                ShowMessage("+" + manager.Config.rewardCoinsAmount + " coins added!");
            else
                ShowMessage("Ad not completed. Try again later.");
        });
    }

    void ShowMessage(string msg)
    {
        if (menuManager != null)
            menuManager.On_MessagePanel(msg);
        else
            Debug.Log(msg);
    }
}
