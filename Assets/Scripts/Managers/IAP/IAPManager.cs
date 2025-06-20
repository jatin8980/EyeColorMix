using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.XR;
using static GeneralDataManager;


//its use for remove ads or non-consumable products.
//if  (m_StoreController.products.WithID("com.your.iap.id.1").hasReceipt)
//    {
//       // IAP success results. 
//    }


public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    public static IAPManager inst;

    public List<string> productIDs = new List<string>();

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    private HashSet<string> purchasedNonConsumables;

    /// <summary>
    /// Save data id
    /// </summary>
    public string SaveId { get { return "iap_manager"; } }

    /// <summary>
    /// Callback that is invoked when the IAPManager has successfully initialized and has retrieved the list of products/prices
    /// </summary>
    public System.Action OnInitializedSuccessfully { get; set; }

    /// <summary>
    /// Callback that is invoked when a product is purchased, passes the product id that was purchased
    /// </summary>
    public System.Action<Product> OnProductPurchased { get; set; }

    /// <summary>
    /// Returns true if IAP is initialized
    /// </summary>
    public bool IsInitialized
    {
        get { return storeController != null && extensionProvider != null; }
    }


    //variable for save IAP info
    public class IAPItem
    {
        public string price, packName;
        public Dictionary<string, int> rewards = new Dictionary<string, int>();
    }
    internal Dictionary<string, IAPItem> iAPSavedDataDictonary = new Dictionary<string, IAPItem>();


    void Awake()
    {
        inst = this;
        iAPSavedDataDictonary = JsonConvert.DeserializeObject<Dictionary<string, IAPItem>>(Resources.Load<TextAsset>("IAP_Details").text);
        purchasedNonConsumables = new HashSet<string>();
    }

    private void Start()
    {
        InitilizitionIAP();
        OnProductPurchased += OnProductPurchases;
    }

    internal void InitilizitionIAP()
    {
        if (IsInitialized) return;

        // Initialize IAP
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add all the product ids to teh builder
        for (int i = 0; i < productIDs.Count; i++)
        {
            Debug.Log("Adding product to builder, id: " + productIDs[i] + ", consumable: " + (productIDs[i].Contains("removeads") ? false : true));
            builder.AddProduct(productIDs[i], productIDs[i].Contains("removeads") ? ProductType.NonConsumable : ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        LoadIAPData(true);
    }

    public void OnInitializeFailed(InitializationFailureReason failureReason)
    {
        LoadIAPData(false);
        Debug.LogError("Initializion failed! Reason: " + failureReason);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        StorePopUp.Inst.LoaderSetActive(false);
        GameManager.Inst.Show_Toast("Purchase Failed!");
        Debug.LogError("Purchase failed for product id: " + product.definition.id + ", reason: " + failureReason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Product product = args.purchasedProduct;

        Debug.Log("Purchase successful for product id: " + product.definition.id);

        if (product.definition.type != ProductType.Consumable)
        {
            purchasedNonConsumables.Add(product.definition.id);
        }

        if (OnProductPurchased != null)
        {
            OnProductPurchased(product);
        }


        //for (int i = 0; i < productsInfo.Count; i++)
        //{
        //	if (productsInfo[i].productId == product.definition.id && productsInfo[i].OnProductPurchaseEvent != null)
        //	{
        //		productsInfo[i].OnProductPurchaseEvent.Invoke();
        //	}
        //}

        return PurchaseProcessingResult.Complete;
    }


    private void OnProductPurchases(Product product)
    {
        StorePopUp.Inst.LoaderSetActive(false);
        StartCoroutine(GivePurchasedRewardIAP(product));
    }

    IEnumerator GivePurchasedRewardIAP(Product product)
    {
        string productID = product.definition.id;
        IAPItem iAPPurchasedItem = iAPSavedDataDictonary[productID];

        if (iAPPurchasedItem.rewards.ContainsKey("removeads"))
        {
            IsPurchaseAdsRemoved = true;
            AdsManager.Inst.RemoveAdsApply();
            GameManager.Inst.homeScreen.RefreshRemoveAdsBtn();
            StorePopUp.Inst.RefreshRemoveAdsOb();
            yield return new WaitForSeconds(0.1f);
            GameManager.Inst.Show_Toast("Ads Removed Successfully!");
        }
        else if (iAPPurchasedItem.rewards.ContainsKey("coin"))
        {
            Coins += iAPPurchasedItem.rewards["coin"];
            GameManager.Inst.StartCoroutine(StorePopUp.Inst.CollectCoins(iAPSavedDataDictonary.Keys.ToList().IndexOf(productID) + 1));
            yield return new WaitForSeconds(0.1f);
            GameManager.Inst.Show_Toast(iAPPurchasedItem.packName + " Purchased!");
        }
    }

    /// <summary>
    /// Starts the buying process for the given product id
    /// </summary>
    public void BuyProduct(string productId)
    {
        Debug.Log("BuyProduct: Purchasing product with id: " + productId);

        if (IsInitialized)
        {
            Product product = storeController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product == null)
            {
                Debug.LogError("BuyProduct: product with id \"" + productId + "\" does not exist.");
            }
            else if (!product.availableToPurchase)
            {
                Debug.LogError("BuyProduct: product with id \"" + productId + "\" is not available to purchase.");
            }
            else
            {
                storeController.InitiatePurchase(product);
            }
        }
        else
        {
            Debug.LogWarning("BuyProduct: IAPManager not initialized.");
        }
    }

    /// <summary>
    /// Gets the products store information
    /// </summary>
    public Product GetProductInformation(string productId)
    {
        if (IsInitialized)
        {
            return storeController.products.WithID(productId);
        }

        return null;
    }

    /// <summary>
    /// Restores the purchases if platform is iOS or OSX
    /// </summary>
    public void RestorePurchases()
    {
        //Logger.Log(LogTag, "RestorePurchases: Restoring purchases");
        if (IsInitialized)
        {
            if ((Application.platform == RuntimePlatform.IPhonePlayer ||
                 Application.platform == RuntimePlatform.OSXPlayer))
            {
                AdsManager.Inst.CanShowAppOpen = false;
                extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result) => { });
            }
            else
            {
                Debug.LogWarning("RestorePurchases: Device is not iOS, no need to call this method.");
            }
        }
        else
        {
            Debug.LogWarning("RestorePurchases: IAPManager not initialized.");
        }
    }

    internal void LoadIAPData(bool loadNew)
    {
        iAPSavedDataDictonary.Clear();
        if (Shop_Saved_Data != "" && !loadNew)
            iAPSavedDataDictonary = JsonConvert.DeserializeObject<Dictionary<string, IAPItem>>(Shop_Saved_Data);
        else
        {
            iAPSavedDataDictonary = JsonConvert.DeserializeObject<Dictionary<string, IAPItem>>(Resources.Load<TextAsset>("IAP_Details").text);
            if (IsInitialized)
            {
                for (var i = 0; i < productIDs.Count; i++)
                {
                    iAPSavedDataDictonary.ElementAt(i).Value.price = GetProductInformation(productIDs[i]).metadata.localizedPrice != 0 ? GetProductInformation(productIDs[i]).metadata.localizedPriceString : iAPSavedDataDictonary.ElementAt(i).Value.price;
                }
            }
            Shop_Saved_Data = JsonConvert.SerializeObject(iAPSavedDataDictonary);
        }

        if (GameManager.activePopUps.Count > 0 && GameManager.activePopUps.Last() == GameManager.Popups.StorePopUp)
        {
            StorePopUp.Inst.SetupIAPList();
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        LoadIAPData(false);
        Debug.LogError("Initializion failed! Reason: " + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        StorePopUp.Inst.LoaderSetActive(false);
        GameManager.Inst.Show_Toast("Purchase Failed!");
        Debug.LogError("Purchase failed for product id: " + product.definition.id + ", reason: " + failureDescription.reason);
    }
}