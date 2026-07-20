using UnityEngine;
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoogleIAPManager : MonoBehaviour
{
    // static IStoreController storeController = null;
    static string[] sProductIds;
    static int[] sProductCoins;
    public RoomManager Roommanager;
    public ProfileManager profileManage;


    void Awake()
    {
        sProductCoins = new int[] { 11000, 27500, 55000, 110000, 550000, 1100000, 5500000, 11000000, 110000000 };
        // if (storeController == null)
        // {
        //     sProductIds = new string[] { "coin_69", "coin_99", "coin_149", "coin_199", "coin_749", "coin_1249", "coin_3999", "coin_6499", "coin_13499" };
        //     InitStore();
        // }
    }

    void InitStore()
    {
        // ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        // for (int i = 0; i < sProductIds.Length; i++)
        // {
        //     builder.AddProduct(sProductIds[i], ProductType.Consumable, new IDs { { sProductIds[i], GooglePlay.Name } });
        // }

        // UnityPurchasing.Initialize(this, builder);
    }

    // void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    // {
    //     storeController = controller;
    //     // txtLog.text = "Billing function initialization completed";
    // }

    // void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    // {
    //     //txtLog.text = "OnInitializeFailed" + error;
    // }

    public void OnBtnPurchaseClicked(GameObject id)
    {
        // int index = int.Parse(id.name);
        // if (storeController == null)
        // {
        //     //txtLog.text = "Purchase failed: Billing function initialization failed";
        // }
        // else
        //     storeController.InitiatePurchase(sProductIds[index]);
    }

    public void OnBtnPurchaseClicked_again(int index)
    {
        // if (storeController == null)
        // {
        //     //txtLog.text = "Purchase failed: Billing function initialization failed";
        // }
        // else
        //     storeController.InitiatePurchase(sProductIds[index]);
    }

//     PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
//     {
//         bool isSuccess = true;
// #if UNITY_ANDROID && !UNITY_EDITOR
// 		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
// 		try
// 		{
// 			IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);
// 			for(int i = 0; i < result.Length; i++)
// 				Analytics.Transaction(result[i].productID, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode, result[i].transactionID, null);
// 		}
// 		catch (IAPSecurityException)
// 		{
// 			isSuccess = false;
// 		}
// #endif
//         if (isSuccess)
//         {
//             Debug.Log("Purchase complete");
//             for (int i = 0; i < sProductIds.Length; i++)
//             {
//                 if (e.purchasedProduct.definition.id.Equals(sProductIds[i]))
//                 {
//                     AddCoin(sProductCoins[i]);
//                     break;
//                 }
//             }
         
//         }
//         else
//         {
//             // txtLog.text = "Purchase Failure: Abnormal Payment";

//         }

//         return PurchaseProcessingResult.Complete;
//     }

    // void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
    // {
    //     if (!error.Equals(PurchaseFailureReason.UserCancelled))
    //     {
    //         //txtLog.text = "Purchase Failure : " + error;
    //     }
    // }

    void AddCoin(int value)
    {
        GameManager.Instance.Points += value;
        Roommanager.UpdateUserInfo();
        profileManage.Counting(GameManager.Instance.Points, GameManager.Instance.Points - value);
    }
}

