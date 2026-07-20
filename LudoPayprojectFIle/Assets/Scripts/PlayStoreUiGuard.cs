using UnityEngine;

/// <summary>
/// Hides real-money UI on Play Store builds (wallet, withdraw, external shop pay, KYC cash).
/// Attach to MenuManager or any persistent menu root.
/// </summary>
public class PlayStoreUiGuard : MonoBehaviour
{
    [Header("Optional — auto-found if empty")]
    public GameObject walletHomeButton;
    public GameObject walletPanel;
    public GameObject kycPanel;

    void Awake()
    {
        if (!PlayStoreCompliance.RealMoneyFeaturesEnabled)
            HideRealMoneyUi();
    }

    void HideRealMoneyUi()
    {
        if (walletPanel == null)
        {
            var home = GameObject.Find("HomeManager");
            if (home != null)
            {
                var hm = home.GetComponent<HomeManager>();
                if (hm != null) walletPanel = hm.WalletPanel;
            }
        }

        if (walletPanel != null)
            walletPanel.SetActive(false);

        if (kycPanel == null)
        {
            var mm = FindObjectOfType<MenuManager>();
            if (mm != null) kycPanel = mm.KYCPanel;
        }

        if (kycPanel != null)
            kycPanel.SetActive(false);

        if (walletHomeButton == null)
        {
            var walletGo = GameObject.Find("Wallet");
            if (walletGo != null) walletHomeButton = walletGo;
        }

        if (walletHomeButton != null)
            walletHomeButton.SetActive(false);

        var walletMgr = FindObjectOfType<WalletManager>();
        if (walletMgr != null)
            walletMgr.gameObject.SetActive(false);
    }
}
