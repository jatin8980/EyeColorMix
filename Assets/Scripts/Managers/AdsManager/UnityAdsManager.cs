using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : SingletonComponent<UnityAdsManager>, IUnityAdsInitializationListener
{
    //Android Ids.
    string androidAppID = "5811841",
        androidBannerID = "Banner_Android",
        androidInterstitialID = "Interstitial_Android",
        androidRewardVideoID = "Rewarded_Android";

    //IPhone Ids.
    string iosAppID = "5811840",
        iosBannerID = "Banner_iOS",
        iosInterstitialID = "Interstitial_iOS",
        iosRewardVideoID = "Rewarded_iOS";

    internal string appID = "", bannerID = "", interstitialID = "", rewardVideoID = "";

    Unity_InterstitialAd interstitialAd = new Unity_InterstitialAd();
    Unity_RewardVideoAd rewardVideoAd = new Unity_RewardVideoAd();

    internal bool isBannerAdShow = false;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            appID = iosAppID;
            bannerID = iosBannerID;
            interstitialID = iosInterstitialID;
            rewardVideoID = iosRewardVideoID;
        }
        else
        {
            appID = androidAppID;
            bannerID = androidBannerID;
            interstitialID = androidInterstitialID;
            rewardVideoID = androidRewardVideoID;
        }
        Initilize_Unity_Ads();
    }

    public void Initilize_Unity_Ads()
    {
        Advertisement.Initialize(appID, AdsManager.Inst.useTestId, this);
    }

    public void OnInitializationComplete()
    {
        Load_Banner_Ad();
        Load_Interstitial_Ad();
        Load_Reward_Ad();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }


    //Banner Ad.
    public void Load_Banner_Ad()
    {
        if (!AdsManager.IsAdShow()) return;

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(bannerID, options);
    }

    void OnBannerLoaded()
    {
    }

    void OnBannerError(string message)
    {
    }

    public void ShowBannerAd()
    {
        if (!Advertisement.Banner.isLoaded)
        {
            Load_Banner_Ad();
            return;
        }
        AdsManager.Inst.bannerView?.Destroy();
        Advertisement.Banner.Show(bannerID, new BannerOptions
        {
            showCallback = () =>
            {
                isBannerAdShow = true;
            }
        });
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
        isBannerAdShow = false;
    }


    public static bool isUnityInterstitialLoded = false, isUnityRewardVideoLoded = false;

    //Interstitial Ad.
    public void Load_Interstitial_Ad()
    {
        if (!AdsManager.IsAdShow()) return;
        interstitialAd.Load_Ad();
    }

    public void Show_Interstitial_Ad()
    {
        interstitialAd.Show_Ad();
        if (!isUnityInterstitialLoded)
            Load_Interstitial_Ad();
    }

    //Reward Video Ad.
    public void Load_Reward_Ad()
    {
        rewardVideoAd.Load_Ad();
    }

    public void Show_Reward_Ad()
    {
        rewardVideoAd.Show_Ad();
    }
}


class Unity_InterstitialAd : IUnityAdsLoadListener, IUnityAdsShowListener
{
    public void Load_Ad()
    {
        Advertisement.Load(UnityAdsManager.Inst.interstitialID, this);
    }

    public void Show_Ad()
    {
        Advertisement.Show(UnityAdsManager.Inst.interstitialID, this);
    }

    //Load callback.
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        UnityAdsManager.isUnityInterstitialLoded = true;
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        UnityAdsManager.isUnityInterstitialLoded = false;
    }

    //After or on ad show callback.
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        UnityAdsManager.isUnityInterstitialLoded = false;
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        AdsManager.Inst.Interstitial_Close();
        UnityAdsManager.Inst.Load_Interstitial_Ad();
        UnityAdsManager.isUnityInterstitialLoded = false;
    }
}


class Unity_RewardVideoAd : IUnityAdsLoadListener, IUnityAdsShowListener
{
    public void Load_Ad()
    {
        Advertisement.Load(UnityAdsManager.Inst.rewardVideoID, this);
    }

    public void Show_Ad()
    {
        Advertisement.Show(UnityAdsManager.Inst.rewardVideoID, this);
    }

    //Load callback.
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        UnityAdsManager.isUnityRewardVideoLoded = true;
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        UnityAdsManager.isUnityRewardVideoLoded = false;
    }

    //After or on ad show callback.
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        UnityAdsManager.isUnityRewardVideoLoded = false;
        if (GameManager.Inst.adLoaderPanel.activeSelf)
        {
            GameManager.Inst.Show_Toast("Ad Not Available!");
        }

        GameManager.Inst.Show_AdLoader_Panel(false);
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        UnityAdsManager.isUnityRewardVideoLoded = false;
        if (adUnitId == UnityAdsManager.Inst.rewardVideoID &&
            showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            GameManager.Inst.Show_AdLoader_Panel(false);
            AdsManager.Inst.Give_Reward();
            UnityAdsManager.Inst.Load_Reward_Ad();
        }
    }
}