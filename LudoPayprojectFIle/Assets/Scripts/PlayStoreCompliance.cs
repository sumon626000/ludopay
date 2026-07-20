/// <summary>
/// Monster Game uses virtual coins only — no real-money withdraw, deposit, or cash payouts.
/// </summary>
public static class PlayStoreCompliance
{
    public const string PrivacyPolicyUrl = "https://admin.monstergame.app/privacy-policy";
    public const string TermsUrl = "https://admin.monstergame.app/terms-and-conditions";
    public const string DefaultAdminApiBaseUrl = "https://admin.monstergame.app/api";

    /// <summary>Virtual coins only. Wallet/withdraw/cash shop are off in every build.</summary>
    public static bool RealMoneyFeaturesEnabled => false;

    /// <summary>No external Razorpay/browser payments — use in-game virtual coins only.</summary>
    public static bool ExternalPaymentsEnabled => false;

#if PLAY_STORE_BUILD
    public static bool AllowCleartextTraffic => false;
#else
    public static bool AllowCleartextTraffic => true;
#endif

    public static string BlockedFeatureMessage =>
        "This game uses virtual coins only. No real money.";
}
