using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsManager : SingletonComponent<AdsManager>
{
    public bool useTestId = false;
    private bool isBannerLoadedForAdmob = false;
    //Android Ids.
    // androidAppID = ca-app-pub-2981087280704608~2965667585
    string androidBannerID = "",
           androidRewardVideoID = "ca-app-pub-2981087280704608/1288789687",
           androidInterstitialID = "ca-app-pub-2981087280704608/2518110873",
           splashAndroidInterstitialID = "ca-app-pub-2981087280704608/2518110873";

    //IPhone Ids.
    //iosAppID = ca-app-pub-3583215259954966~9694727467  
    string iosBannerID = "",
           iosRewardVideoID = "ca-app-pub-3583215259954966/2792339397",
           iosInterstitialID = "ca-app-pub-3583215259954966/7387294175",
           splashIOSInterstitialID = "ca-app-pub-3583215259954966/7387294175";

    private string bannerID = "", rewardVideoID = "", interstitialID = "", splashInterstitialID = "";

    internal BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private string rewardType, interstitialType;


    #region UNITY MONOBEHAVIOR METHODS

    protected override void Awake()
    {
        if (useTestId)
        {
#if UNITY_ANDROID
            //androidAppID = "ca-app-pub-3940256099942544~3347511713";
            androidBannerID = "ca-app-pub-3940256099942544/6300978111";
            androidRewardVideoID = "ca-app-pub-3940256099942544/5224354917";
            androidInterstitialID = "ca-app-pub-3940256099942544/1033173712";
            splashAndroidInterstitialID = androidInterstitialID;
#elif UNITY_IPHONE
            //iosAppID = "ca-app-pub-3940256099942544~1458002511";
            iosBannerID = "ca-app-pub-3940256099942544/2934735716";
            iosRewardVideoID = "ca-app-pub-3940256099942544/1712485313";
            iosInterstitialID = "ca-app-pub-3940256099942544/4411468910";
            splashIOSInterstitialID = iosInterstitialID;
#endif
        }

#if UNITY_ANDROID
        bannerID = androidBannerID;
        interstitialID = androidInterstitialID;
        rewardVideoID = androidRewardVideoID;
        splashInterstitialID = splashAndroidInterstitialID;

#elif UNITY_IPHONE
        bannerID = iosBannerID;
        interstitialID = iosInterstitialID;
        rewardVideoID = iosRewardVideoID;
        splashInterstitialID = splashIOSInterstitialID;
#else
        bannerID = "unexpected_platform";
        rewardVideoID = "unexpected_platform";
        interstitialID = "unexpected_platform";
#endif


        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(initStatus => { });
    }


    public void Start()
    {
        Invoke(nameof(LoadSplashInterstitial), 0.8f);
    }

    private void LoadSplashInterstitial()
    {
        RequestAndLoadInterstitialAd(true);
    }

    private static void HandleInitCompleteAction(InitializationStatus initStatus)
    {
        Debug.Log("Initialization complete.");
    }

    #endregion

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }

    public static bool isSplash_InterstitialShowTime()
    {
        var count = PlayerPrefs.GetInt("SplashInterstitialShowCount", 1);
        PlayerPrefs.SetInt("SplashInterstitialShowCount", 1);
        return count == 1;
    }

    private static void Reset_SplashShowCount()
    {
        PlayerPrefs.SetInt("SplashInterstitialShowCount", 1);
    }

    internal void RemoveAdsApply()
    {
        DestroyInterstitialAd();
        Destroy_NativeBanner_Ad();
        UnityAdsManager.isUnityInterstitialLoded = false;
        isRequestSended = false;
        //DestroyBannerAd();
    }

    public static bool IsAdShow()
    {
        return !GeneralDataManager.IsPurchaseAdsRemoved;
    }

    private void OnApplicationQuit()
    {
        rewardedAd?.Destroy();
        RemoveAdsApply();
    }

    #endregion

    /*#region BANNER ADS

    public void RequestBannerAd()
    {
        if (!IsAdShow())
            return;

        bannerView?.Destroy();
        bannerView = new BannerView(bannerID, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
        bannerView.LoadAd(CreateAdRequest());
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.LogError("Banner view loaded an ad with response : " + bannerView.GetResponseInfo());
            isBannerLoadedForAdmob = true;
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            UnityAdsManager.Inst.ShowBannerAd();
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        };
    }

    public void ShowBannerAd() => Invoke(nameof(ShowBannerInvoke), 0.1f);

    private void ShowBannerInvoke()
    {
        if (isBannerLoadedForAdmob == false && UnityAdsManager.Inst.isBannerAdShow == false)
        {
            RequestBannerAd();
            return;
        }
        if (isBannerLoadedForAdmob)
        {
            bannerView?.Show();
        }
        else if (UnityAdsManager.Inst.isBannerAdShow)
        {
            UnityAdsManager.Inst.ShowBannerAd();
        }
    }

    public void HideBannerAd()
    {
        CancelInvoke(nameof(ShowBannerInvoke));
        if (isBannerLoadedForAdmob)
        {
            bannerView?.Hide();
        }
        else if (UnityAdsManager.Inst.isBannerAdShow)
        {
            Advertisement.Banner.Hide();
        }
    }

    public void DestroyBannerAd()
    {
        UnityAdsManager.Inst.HideBannerAd();
        isBannerLoadedForAdmob = false;
        bannerView?.Destroy();
    }

    #endregion*/

    #region INTERSTITIAL ADS

    internal bool isRequestSended = false;
    internal void RequestAndLoadInterstitialAd(bool isSplash = false)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
            return;

        if (!IsAdShow())
            return;

        isRequestSended = true;
        InterstitialAd.Load(isSplash ? splashInterstitialID : interstitialID, CreateAdRequest(),
            (ad, loadError) =>
            {
                if (loadError != null || ad == null)
                {
                    //Here for showing unity in splash screen.
                    Invoke(nameof(Interstitial_OnAdLoaded), 0.08f);
                    return;
                }

                interstitialAd = ad;

                //On Ad loaded call
                Invoke(nameof(Interstitial_OnAdLoaded), 0.08f);
                ad.OnAdFullScreenContentClosed += Interstitial_OnAdClosed;
            });
    }

    public void ShowInterstitialAd(string type, UnityAction callBack = null)
    {
        if (!IsAdShow())
        {
            callBack?.Invoke();
            return;
        }

        interstitialType = type;
        if (interstitialAd == null || !interstitialAd.CanShowAd())
        {
            if (!UnityAdsManager.isUnityInterstitialLoded)
                UnityAdsManager.Inst.Load_Interstitial_Ad();
            if (UnityAdsManager.isUnityInterstitialLoded && isRequestSended)
            {
                isRequestSended = false;
                interstitialCallBack = null;
                interstitialCallBack = callBack;
                UnityAdsManager.Inst.Show_Interstitial_Ad();
            }
            else
                callBack?.Invoke();
            return;
        }
        isRequestSended = false;
        interstitialCallBack = null;
        interstitialCallBack = callBack;
        interstitialAd?.Show();
    }

    public void DestroyInterstitialAd()
    {
        interstitialAd?.Destroy();
    }

    private void Interstitial_OnAdClosed()
    {
        GeneralDataManager.Inst.No_Click_Panel_On_Off(true);
        Invoke(nameof(Interstitial_Close), 0.04f);
    }

    private void Interstitial_OnAdLoaded()
    {
        if (GameManager.Inst.launchScreen.activeSelf && isSplash_InterstitialShowTime())
        {
            ShowInterstitialAd("Splash");
        }
    }

    private UnityAction interstitialCallBack;

    public void Interstitial_Close()
    {
        GeneralDataManager.Inst.No_Click_Panel_On_Off(false);
        interstitialCallBack?.Invoke();
        switch (interstitialType)
        {
            case "Splash":
                SplashTasks();
                break;
        }
    }

    private void SplashTasks()
    {
        Reset_SplashShowCount();
        FindObjectOfType<LaunchScreenController>()?.Complete();
    }

    #endregion

    #region REWARDED ADS
    internal bool isRewardRequestAlreadySended = false;
    public void RequestAndLoadRewardedAd(string type)
    {
        rewardType = type;

        if (GameManager.Is_Internet_Available() || (rewardedAd != null && rewardedAd.CanShowAd()))
        {
            GameManager.Inst.Show_AdLoader_Panel(true);

            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                ShowRewardedAd();
            }
            else
            {
                if (isRewardRequestAlreadySended) return;
                isRewardRequestAlreadySended = true;

                RewardedAd.Load(rewardVideoID, CreateAdRequest(),
                    (ad, loadError) =>
                    {
                        if (loadError != null || ad == null)
                        {
                            Invoke(nameof(HandleRewardAdFailedToLoad), 0.04f);
                            Debug.LogError("Rewarded ad failed to load with error: " + loadError?.GetMessage());
                            return;
                        }

                        rewardedAd = ad;
                        HandleRewardedAdLoaded();
                        ad.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
                    });
            }
        }
        else
        {
            GameManager.Inst.Show_Toast("Check Network Connection!");
        }
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) => { HandleUserEarnedReward(); });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    private void HandleRewardedAdLoaded()
    {
        if (GameManager.Inst.adLoaderPanel.activeSelf)
            ShowRewardedAd();
    }

    private void HandleUserEarnedReward()
    {
        Invoke(nameof(Give_Reward), 0.08f);
    }


    private void HandleRewardAdFailedToLoad()
    {
        isRewardRequestAlreadySended = false;
        if (GameManager.Inst.adLoaderPanel.activeSelf)
        {
            if (UnityAdsManager.isUnityRewardVideoLoded)
            {
                UnityAdsManager.Inst.Show_Reward_Ad();
                return;
            }
            UnityAdsManager.Inst.Load_Reward_Ad();
            if (GameManager.Inst.adLoaderPanel.activeSelf)
                GameManager.Inst.Show_Toast("Ad Not Available!");
        }
        GameManager.Inst.Show_AdLoader_Panel(false);
    }


    private void HandleRewardedAdClosed()
    {
        isRewardRequestAlreadySended = false;
        GameManager.Inst.Show_AdLoader_Panel(false);
    }

    public void Give_Reward()
    {
        switch (rewardType)
        {
            case "UnlockImage":
                ChooseImagePopUpController.Inst.UnlockImageCallBack();
                break;
            case "UnlockPupil":
                PupilController.Inst.UnlockPupilCallBack();
                break;
            case "UnlockPattern":
                PatternController.Inst.UnlockPatternCallBack();
                break;
            case "UnlockShopItem":
                ShopPopUpController.Inst.UnlockRandomCallBack();
                break;
            case "FreeCoins":
                FindObjectOfType<FreeCoinsPopUp>().FreeCoinsCallBack();
                break;
            case "LevelCompleteDoubleReward":
                FindObjectOfType<LevelComplete>().DoubleRewardCallback();
                break;
            case "DailyDoubleReward":
                FindObjectOfType<DailyRewardPopUp>().DoubleRewardCallBack();
                break;
                /*case "AutoStretchPower":
                    GameManager.Inst.gamePlayUi.AutoStretchPowerCallBack();
                    break;*/
        }
    }
    #endregion

    #region Native Banner Ad

    internal bool isNativeAdLoaded = false;
    private NativeAd nativeAd;
    NativeBannerController nativeBanner;
    public GameObject[] nativeBannerPrefabs;

    string[] android_NativeID = {
            "ca-app-pub-2981087280704608/4497169273"},
            iPhone_NativeID = {
            "ca-app-pub-3583215259954966/7929147829"};
    string _nativeAdID = string.Empty;
    internal int bannerType = 0;

    public void Refresh_NativeAd()
    {
        RequestNativeAd(true);
    }

    public void RequestNativeAd(bool refreshAd = false)
    {
        if (!IsAdShow())
            return;

        isNativeAdLoaded = false;
        Destroy_NativeBanner_Ad();


        bannerType = Random.Range(0, nativeBannerPrefabs.Length);

#if UNITY_ANDROID
        _nativeAdID = android_NativeID[bannerType];
#elif UNITY_IPHONE
        _nativeAdID = iPhone_NativeID[bannerType];
#else
        _nativeAdID = "unexpected_platform";
#endif

        if (useTestId)
        {
#if UNITY_ANDROID
            _nativeAdID = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
            _nativeAdID = "ca-app-pub-3940256099942544/3986624511";
#endif
        }

        AdLoader adLoader = new AdLoader.Builder(_nativeAdID).ForNativeAd().Build();

        if (!refreshAd)
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        else
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded_ForRefresh;

        adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;

        adLoader.LoadAd(new AdRequest());
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        isNativeAdLoaded = true;

        this.nativeAd = args.nativeAd;

        nativeBanner = Instantiate(nativeBannerPrefabs[bannerType], GameManager.Inst.nativeAdParent).GetComponent<NativeBannerController>();
        nativeBanner.Refresh_Details(nativeAd);
        GameManager.Inst.RefreshCurrentScreen();
    }

    private void HandleNativeAdLoaded_ForRefresh(object sender, NativeAdEventArgs args)
    {
        isNativeAdLoaded = true;

        this.nativeAd = args.nativeAd;

        nativeBanner = Instantiate(nativeBannerPrefabs[bannerType], GameManager.Inst.nativeAdParent).GetComponent<NativeBannerController>();
        nativeBanner.Refresh_Details(nativeAd);
        GameManager.Inst.RefreshCurrentScreen();
    }

    public void Apply_Bottom_Padding_ForNative_Banner(RectTransform rt, int bottomPadding)
    {
        //if (nativeBanner != null && isNativeAdLoaded)
        rt.offsetMin = new Vector2(rt.offsetMin.x, rt.offsetMin.y + bottomPadding);
    }

    internal void Destroy_NativeBanner_Ad()
    {
        if (nativeBanner?.gameObject != null)
            Destroy(nativeBanner.gameObject);
        isNativeAdLoaded = false;
        nativeAd?.Destroy();
        GameManager.Inst.RefreshCurrentScreen();
    }

    void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        isNativeAdLoaded = false;
        GameManager.Inst.RefreshCurrentScreen();
        Debug.Log("Native ad failed to load: " + args);
    }

    #endregion
}