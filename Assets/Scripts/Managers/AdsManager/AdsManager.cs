using GoogleMobileAds.Api;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsManager : SingletonComponent<AdsManager>
{
    public bool useTestId = false;
    internal static bool IsQuittingGame;
    internal bool isBannerLoaded = false;
    //Android Ids.
    // androidAppID = ca-app-pub-2981087280704608~2965667585
    string androidBannerID = "ca-app-pub-2981087280704608/7209517934",
           androidRewardVideoID = "ca-app-pub-2981087280704608/1288789687",
           androidInterstitialID = "ca-app-pub-2981087280704608/2518110873",
           splashAndroidInterstitialID = "ca-app-pub-2981087280704608/2518110873",
           _androidAppOpenID = "ca-app-pub-2981087280704608/9448684379",
           _androidRewardInterstitialID = "ca-app-pub-2981087280704608/9495951685";

    //IPhone Ids.
    //iosAppID = ca-app-pub-3583215259954966~9694727467  
    string iosBannerID = "ca-app-pub-3583215259954966/8784106445",
           iosRewardVideoID = "ca-app-pub-3583215259954966/2792339397",
           iosInterstitialID = "ca-app-pub-3583215259954966/7387294175",
           splashIOSInterstitialID = "ca-app-pub-3583215259954966/7387294175",
           _iosAppOpenID = "ca-app-pub-3583215259954966/8592534751",
           _iosRewardInterstitialID = "ca-app-pub-3583215259954966/6655000072";

    private string bannerID = "", rewardVideoID = "", interstitialID = "", splashInterstitialID = "", _appOpenId, _rewardInterstitialId;

    internal BannerView bannerView;
    private BannerView loadBanner;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private AppOpenAd _appOpenAd;
    private RewardedInterstitialAd _rewardInterstitialAd;

    private string rewardType, interstitialType;
    private bool isMainThreadBlocked;


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
            _androidAppOpenID = "ca-app-pub-3940256099942544/9257395921";
            _androidRewardInterstitialID = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
            //iosAppID = "ca-app-pub-3940256099942544~1458002511";
            iosBannerID = "ca-app-pub-3940256099942544/2934735716";
            iosRewardVideoID = "ca-app-pub-3940256099942544/1712485313";
            iosInterstitialID = "ca-app-pub-3940256099942544/4411468910";
            splashIOSInterstitialID = iosInterstitialID;
            _iosAppOpenID = "ca-app-pub-3940256099942544/5575463023";
            _iosRewardInterstitialID = "ca-app-pub-3940256099942544/6978759866";
#endif
        }

#if UNITY_ANDROID
        bannerID = androidBannerID;
        interstitialID = androidInterstitialID;
        rewardVideoID = androidRewardVideoID;
        splashInterstitialID = splashAndroidInterstitialID;
        _appOpenId = _androidAppOpenID;
        _rewardInterstitialId = _androidRewardInterstitialID;

#elif UNITY_IPHONE
        bannerID = iosBannerID;
        interstitialID = iosInterstitialID;
        rewardVideoID = iosRewardVideoID;
        splashInterstitialID = splashIOSInterstitialID;
        _appOpenId = _iosAppOpenID;
        _rewardInterstitialId = _iosRewardInterstitialID;
#else
        bannerID = "unexpected_platform";
        rewardVideoID = "unexpected_platform";
        interstitialID = "unexpected_platform";
        _appOpenId = "unexpected_platform";
        _rewardInterstitialId = "unexpected_platform";
#endif
        if (Application.platform == RuntimePlatform.WindowsEditor)
            isBannerLoaded = true;

        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(initStatus => { });
    }


    public void Start()
    {
        Invoke(nameof(LoadAds), 0.8f);
    }

    private void LoadAds()
    {
        RequestAndLoadInterstitialAd(true);
        LoadAppOpenAd();
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
        PlayerPrefs.SetInt("SplashInterstitialShowCount", 2);
        return count == 2;
    }

    private static void Reset_SplashShowCount()
    {
        PlayerPrefs.SetInt("SplashInterstitialShowCount", 1);
    }

    internal void RemoveAdsApply()
    {
        DestroyBannerAd();
        CancelBannerRequest();
        DestroyInterstitialAd();
        _rewardInterstitialAd?.Destroy();
        _appOpenAd?.Destroy();
        UnityAdsManager.isUnityInterstitialLoded = false;
        isRequestSended = false;
    }

    public static bool IsAdShow()
    {
        return !GeneralDataManager.IsPurchaseAdsRemoved;
    }

    private void OnApplicationQuit()
    {
        IsQuittingGame = true;
        rewardedAd?.Destroy();
        RemoveAdsApply();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && ChooseImagePopUpController.Inst != null && isMainThreadBlocked)
        {
            if (!IsInvoking(nameof(SetThreadBlockFalse)))
                Invoke(nameof(SetThreadBlockFalse), 0.5f);
            return;
        }
        if (!pause && !GameManager.Inst.launchScreen.activeSelf)
            ShowAppOpenAd();
    }

    private void SetThreadBlockFalse()
    {
        isMainThreadBlocked = false;
        CanShowAppOpen = true;
    }

    internal void SetThreadBlockTrue()
    {
        CancelInvoke(nameof(SetThreadBlockFalse));
        isMainThreadBlocked = true;
    }
    #endregion

    #region BANNER ADS
    private bool isBannerAdRequested;
    internal void RequestBannerAd(bool isCollapsible)
    {
        if (!IsAdShow())
        {
            DestroyBannerAd();
            return;
        }
        if (isBannerAdRequested)
            return;
        isBannerAdRequested = true;
        loadBanner = new BannerView(bannerID,
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);
        var req = CreateAdRequest();
        if (isCollapsible) req.Extras.Add("collapsible", "bottom");
        loadBanner.LoadAd(req);
        loadBanner.OnBannerAdLoaded += OnBannerLoadInvoke;
        loadBanner.OnBannerAdLoadFailed += (_) => { isBannerAdRequested = false; };
    }

    internal void DestroyBannerAd()
    {
        UnityAdsManager.Inst.HideBannerAd();
        bannerView?.Destroy();
        isBannerAdRequested = false;
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            isBannerLoaded = false;
            GameManager.Inst.RefreshCurrentPopUpForBannerAd();
        }
    }

    internal void CancelBannerRequest()
    {
        if (loadBanner != null && loadBanner != bannerView)
        {
            loadBanner.OnBannerAdLoaded -= OnBannerLoadInvoke;
            loadBanner.Destroy();
            loadBanner = null;
            isBannerAdRequested = false;
        }
    }

    private void OnBannerLoadInvoke()
    {
        Invoke(nameof(OnBannerLoad), 0.1f);
    }

    private void OnBannerLoad()
    {
        DestroyBannerAd();
        bannerView = loadBanner;
        loadBanner = null;
        bannerView.OnAdFullScreenContentOpened += () => { CanShowAppOpen = false; };
        bannerView.OnBannerAdLoaded -= OnBannerLoadInvoke;
        isBannerLoaded = true;
        isBannerAdRequested = false;
        GameManager.Inst.RefreshCurrentPopUpForBannerAd();
    }
    #endregion

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
                    Invoke(nameof(Interstitial_OnAdLoaded), 0.1f);
                    return;
                }

                interstitialAd = ad;

                //On Ad loaded call
                Invoke(nameof(Interstitial_OnAdLoaded), 0.1f);
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
        CanShowAppOpen = false;
        interstitialType = type;

        bool isLoaded = IsRewardInterstitialAvailable();
        if ((isLoaded || (!isLoaded && type.Contains("RewardInterstitial"))) && type != "Splash" && _isRewardInterstitialRequested)
        {
            SetInterstitialCallback(callBack);
            ShowRewardInterstitial();
            return;
        }

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
                return;
            }

            interstitialType = "";
            CanShowAppOpen = true;
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
        Invoke(nameof(Interstitial_Close), 0.08f);
    }

    private void Interstitial_OnAdLoaded()
    {
        if (GameManager.Inst.launchScreen.activeSelf && isSplash_InterstitialShowTime())
            ShowInterstitialAd("Splash");
    }

    private UnityAction interstitialCallBack;

    public void Interstitial_Close()
    {
        GeneralDataManager.Inst.No_Click_Panel_On_Off(false);
        interstitialCallBack?.Invoke();
        switch (interstitialType)
        {
            case "Splash":
                Reset_SplashShowCount();
                FindAnyObjectByType<LaunchScreenController>()?.Complete();
                break;
        }
        interstitialType = "";
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
                            Invoke(nameof(HandleRewardAdFailedToLoad), 0.1f);
                            Debug.LogError("Rewarded ad failed to load with error: " + loadError?.GetMessage());
                            return;
                        }

                        rewardedAd = ad;
                        Invoke(nameof(HandleRewardedAdLoaded), 0.1f);
                        ad.OnAdFullScreenContentClosed += () => { Invoke(nameof(HandleRewardedAdClosed), 0.1f); };
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
            CanShowAppOpen = false;
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
        Invoke(nameof(Give_Reward), 0.1f);
    }


    private void HandleRewardAdFailedToLoad()
    {
        isRewardRequestAlreadySended = false;
        if (GameManager.Inst.adLoaderPanel.activeSelf)
        {
            if (UnityAdsManager.isUnityRewardVideoLoded)
            {
                CanShowAppOpen = false;
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
            case "UnlockItem":
                FindAnyObjectByType<UnlockItemPopUp>().UnlockItemCallBack();
                break;
            case "UnlockPupil":
                PupilController.Inst.UnlockPupilCallBack();
                break;
            case "UnlockPattern":
                PatternController.Inst.UnlockPatternCallBack();
                break;
            case "FreeCoins":
                StorePopUp.Inst.FreeCoinsCallBack();
                break;
            case "LevelCompleteDoubleReward":
                FindAnyObjectByType<LevelComplete>().DoubleRewardCallback();
                break;
            case "DailyDoubleReward":
                FindAnyObjectByType<DailyRewardPopUp>().DoubleRewardCallBack();
                break;
                /*case "AutoStretchPower":
                    GameManager.Inst.gamePlayUi.AutoStretchPowerCallBack();
                    break;*/
        }
    }
    #endregion

    #region App Open

    private int _failCountAppOpen;
    internal bool CanShowAppOpen;
    private bool _isAppOpenAdRequested;

    private void LoadAppOpenAd()
    {
        if (!IsAdShow())
            return;
        CanShowAppOpen = true;
        if (_appOpenAd != null && _appOpenAd.CanShowAd()) return;

        if (_isAppOpenAdRequested) return;
        _isAppOpenAdRequested = true;

        AppOpenAd.Load(_appOpenId, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                _isAppOpenAdRequested = false;
                return;
            }

            _appOpenAd = ad;
            _appOpenAd.OnAdFullScreenContentClosed += () => { Invoke(nameof(OnAppOpenAddClose), 0.1f); };
            _appOpenAd.OnAdFullScreenContentFailed += _ => { Invoke(nameof(OnAppOpenAdFailToLoad), 0.1f); };
            Invoke(nameof(OnAppOpenAdLoad), 0.1f);
        });
    }

    private void OnAppOpenAddClose()
    {
        _isAppOpenAdRequested = false;
        GameManager.Inst.interstitialAdLoader.SetActive(false);
        _failCountAppOpen = 0;
        _appOpenAd?.Destroy();
        _appOpenAd = null;
        LoadAppOpenAd();
    }

    private void OnAppOpenAdLoad()
    {
        if (GameManager.Inst.interstitialAdLoader.activeSelf && _rewardInterstitialCallBackAction == null)
            _appOpenAd?.Show();
    }
    private void OnAppOpenAdFailToLoad()
    {
        _isAppOpenAdRequested = false;
        if (_failCountAppOpen < 2)
        {
            _failCountAppOpen++;
            LoadAppOpenAd();
            return;
        }

        _failCountAppOpen = 0;
    }

    internal void ShowAppOpenAd()
    {
        Debug.Log(CanShowAppOpen);
        if (!IsAdShow())
            return;

        if (!CanShowAppOpen)
        {
            CanShowAppOpen = true;
            return;
        }

        SetInterstitialCallback(null);
        GameManager.Inst.interstitialAdLoader.SetActive(true);

        if (_appOpenAd != null && _appOpenAd.CanShowAd())
        {
            _appOpenAd.Show();
            return;
        }

        _failCountAppOpen = 0;
        LoadAppOpenAd();
    }

    #endregion

    #region Reward Interstitial

    private UnityAction _rewardInterstitialCallBackAction;
    private bool _isRewardInterstitialRequested;

    internal void RequestRewardInterstitial()
    {
        if (!IsAdShow())
            return;
        if (_rewardInterstitialAd != null && _rewardInterstitialAd.CanShowAd()) return;

        if (_isRewardInterstitialRequested) return;
        _isRewardInterstitialRequested = true;

        RewardedInterstitialAd.Load(_rewardInterstitialId, CreateAdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                _isRewardInterstitialRequested = false;
                return;
            }

            _rewardInterstitialAd = ad;
            _rewardInterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Invoke(nameof(OnRewardInterstitialAdClose), 0.1f);
            };
            _rewardInterstitialAd.OnAdFullScreenContentFailed += _ => { _isRewardInterstitialRequested = false; };
            Invoke(nameof(OnRewardInterstitialLoad), 0.1f);
        });
    }

    private void OnRewardInterstitialLoad()
    {
        if (GameManager.Inst.interstitialAdLoader.activeSelf) ShowRewardInterstitial();
    }

    private void OnRewardInterstitialAdClose()
    {
        GameManager.Inst.interstitialAdLoader.SetActive(false);
        _isRewardInterstitialRequested = false;
    }

    internal void SetInterstitialCallback(UnityAction callback)
    {
        _rewardInterstitialCallBackAction = null;
        _rewardInterstitialCallBackAction = callback;
    }

    internal void OnLoaderDisable()
    {
        _rewardInterstitialCallBackAction?.Invoke();
        _rewardInterstitialCallBackAction = null;
    }

    internal bool IsRewardInterstitialAvailable() =>
        _rewardInterstitialAd != null && _rewardInterstitialAd.CanShowAd();

    internal void ShowRewardInterstitial()
    {
        if (!IsAdShow())
        {
            OnLoaderDisable();
            return;
        }
        GameManager.Inst.interstitialAdLoader.SetActive(true);
        if (_rewardInterstitialAd == null || !_rewardInterstitialAd.CanShowAd()) return;
        CanShowAppOpen = false;
        _rewardInterstitialAd.Show(_ => { });
    }

    #endregion
}