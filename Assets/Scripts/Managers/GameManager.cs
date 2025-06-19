using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("General")]
    public GameObject adLoaderPanel;
    public GameObject interstitialAdLoader;

    [Header("Screen Referance")]
    public GamePlayUIController gamePlayUi;
    public HomeScreenController homeScreen;
    public GameObject launchScreen;
    [SerializeField] private Transform worldScreenParent;
    [SerializeField] private GameObject gamePlay;

    [Header("PopUp Refrence")]
    [SerializeField] private Transform popUpParent;
    [SerializeField]
    private GameObject settingPopUp, exitPopUp, toastPrefab, rateusPopUp, chooseEffectPopUp,
        chooseImagePopUp, galleryPopUp, galleryDetailPopUp, levelCompletePopUp, themePopUp, storePopUp, tutorialPopUp,
        permissionPopUp, dailyRewardPopUp, unlockItemPopUp, noInternetPopUp;

    [Header("Others")]
    public Camera mainCamera;
    public GameObject loaderOb;
    public Transform coinCollectParentTR;
    [SerializeField] private RectTransform coinsRT;
    [SerializeField] private Transform clickParticleParentTR;
    [SerializeField] private Text coinText;
    [SerializeField] private Sprite coinsUnfillSp, coinsFillSp;

    private GameObject toast, gamePlayWorldOb;
    internal bool canShowClickParticle = true;
    internal static GameManager Inst;
    internal static List<GameObject> activePopUpsObs = new();
    internal static List<Popups> activePopUps = new();
    private List<Popups> popUpsToShowCoinsAbove = new() { Popups.ChooseImagePopUp, Popups.DailyRewardPopUp, Popups.LevelCompletePopUp, Popups.StorePopUp, Popups.ThemesPopUp, Popups.UnlockItemPopUp},
                         popUpsToShowFillCoins = new() { Popups.LevelCompletePopUp, Popups.StorePopUp, Popups.ThemesPopUp };

    internal float bannerHeight = 160;

    private void Awake()
    {
        Inst = this;
        Application.targetFrameRate = 1000;
        Input.multiTouchEnabled = false;
    }

    private void Start()
    {
        activePopUps.Add(Popups.Null);
        activePopUpsObs.Add(null);
        SetActiveCoins(false);
        homeScreen.characterTR.gameObject.SetActive(false);
        Show_Screen(Screens.Launch);
        Invoke(nameof(SetSelectedClickParticle), 1f);
    }

    public enum Screens
    {
        None,
        Launch,
        Home,
        GamePlay
    }

    public static Screens activeScreen = Screens.None;
    public static GameObject activeScreenObj;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && activePopUps.Last() is Popups.TutorialPopUp or Popups.Null or Popups.StorePopUp
            or Popups.ChooseEffectPopUp or Popups.ChooseImagePopUp or Popups.GalleryPopUp or Popups.LevelCompletePopUp or Popups.ThemesPopUp)
        {
            Invoke(nameof(ShowClickParticle), 0.05f);
        }
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (activeScreen == Screens.Launch || GeneralDataManager.Inst.IsNoClickPanelOn() || adLoaderPanel.activeSelf
            || interstitialAdLoader.activeSelf || loaderOb.activeSelf) return;

        if (popUpParent.childCount > 0)
        {
            if (activePopUps[activePopUps.Count - 1] is Popups.ChooseEffectPopUp or Popups.LevelCompletePopUp or Popups.TutorialPopUp
                or Popups.Permission or Popups.NoInternetPopUp)
            {
                return;
            }
            switch (activePopUps[activePopUps.Count - 1])
            {
                case Popups.ChooseImagePopUp:
                    activePopUpsObs[activePopUpsObs.Count - 1].GetComponent<ChooseImagePopUpController>().On_Back_Btn_Click();
                    return;
                case Popups.DailyRewardPopUp:
                    activePopUpsObs[activePopUpsObs.Count - 1].GetComponent<DailyRewardPopUp>().On_Close_Btn_Click();
                    return;
            }
            HidePopUp(activePopUpsObs[activePopUpsObs.Count - 1]);
            return;
        }

        if (activeScreen == Screens.GamePlay)
        {
            gamePlayUi.On_Back_Btn_Click();
            return;
        }

        if (activeScreen == Screens.Home)
        {
            Show_Popup(Popups.Exit);
        }
    }

    public void Show_Screen(Screens screen)
    {
        if (activeScreen == screen) return;
        if (activeScreen == Screens.GamePlay)
            Destroy(gamePlayWorldOb);
        if (activeScreenObj)
        {
            activeScreenObj.SetActive(false);
        }

        activeScreen = screen;
        switch (screen)
        {
            case Screens.None:
                return;
            case Screens.Launch:
                activeScreenObj = launchScreen;
                activeScreenObj.SetActive(true);
                return;
            case Screens.Home:
                activeScreenObj = homeScreen.gameObject;
                activeScreenObj.SetActive(true);
                homeScreen.characterTR.gameObject.SetActive(true);
                homeScreen.DoFadeCharacter(1, 0);
                return;
            case Screens.GamePlay:
                activeScreenObj = gamePlayUi.gameObject;
                gamePlayWorldOb = Instantiate(gamePlay, worldScreenParent);
                activeScreenObj.SetActive(true);
                homeScreen.characterTR.gameObject.SetActive(false);
                homeScreen.DoFadeCharacter(0, 0);
                return;
        }
    }

    public enum Popups
    {
        Null,
        Setting,
        Exit,
        RateUs,
        ChooseEffectPopUp,
        ChooseImagePopUp,
        GalleryPopUp,
        GalleryDetailPopUp,
        LevelCompletePopUp,
        ThemesPopUp,
        StorePopUp,
        TutorialPopUp,
        Permission,
        DailyRewardPopUp,
        UnlockItemPopUp,
        NoInternetPopUp
    }

    public void Show_Popup(Popups popup, bool destroyOld = true)
    {
        if (activePopUps[activePopUps.Count - 1] == popup) return;

        if (destroyOld == false)
        {
            activePopUps.Add(Popups.Null);
            activePopUpsObs.Add(null);
        }

        if (activePopUpsObs[activePopUpsObs.Count - 1]) HidePopUp(activePopUpsObs[activePopUpsObs.Count - 1]);
        activePopUps[activePopUps.Count - 1] = popup;

        activePopUpsObs[activePopUpsObs.Count - 1] = popup switch
        {
            Popups.Setting => Instantiate(settingPopUp, popUpParent),
            Popups.Exit => Instantiate(exitPopUp, popUpParent),
            Popups.RateUs => Instantiate(rateusPopUp, popUpParent),
            Popups.ChooseEffectPopUp => Instantiate(chooseEffectPopUp, popUpParent),
            Popups.ChooseImagePopUp => Instantiate(chooseImagePopUp, popUpParent),
            Popups.GalleryPopUp => Instantiate(galleryPopUp, popUpParent),
            Popups.GalleryDetailPopUp => Instantiate(galleryDetailPopUp, popUpParent),
            Popups.LevelCompletePopUp => Instantiate(levelCompletePopUp, popUpParent),
            Popups.ThemesPopUp => Instantiate(themePopUp, popUpParent),
            Popups.StorePopUp => Instantiate(storePopUp, popUpParent),
            Popups.TutorialPopUp => Instantiate(tutorialPopUp, popUpParent),
            Popups.Permission => Instantiate(permissionPopUp, popUpParent),
            Popups.DailyRewardPopUp => Instantiate(dailyRewardPopUp, popUpParent),
            Popups.UnlockItemPopUp => Instantiate(unlockItemPopUp, popUpParent),
            Popups.NoInternetPopUp => Instantiate(noInternetPopUp, popUpParent),
            _ => null,
        };

        if (popUpsToShowCoinsAbove.Contains(popup))
        {
            SetCoinParentAbovePopUp(true);
            SetActiveFillCoin(popUpsToShowFillCoins.Contains(popup));
        }
        else
        {
            SetCoinParentAbovePopUp(false);
        }
    }

    public void HidePopUp(GameObject popup)
    {
        Destroy(popup);
        if (activePopUps.Count > 1)
        {
            activePopUps.RemoveAt(activePopUps.Count - 1);
            activePopUpsObs.RemoveAt(activePopUpsObs.Count - 1);
        }
        else
        {
            activePopUps[0] = Popups.Null;
            activePopUpsObs[0] = null;
        }

        if (popUpsToShowCoinsAbove.Contains(activePopUps.Last()))
        {
            SetCoinParentAbovePopUp(true);
            SetActiveFillCoin(popUpsToShowFillCoins.Contains(activePopUps.Last()));
        }
        else
        {
            SetCoinParentAbovePopUp(false);
            if (activePopUps.Last() is Popups.Null or Popups.TutorialPopUp)
            {
                SetActiveFillCoin(activeScreen == Screens.GamePlay);
            }
        }
    }

    internal static bool Is_Internet_Available()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    internal void RefreshCurrentPopUpForBannerAd()
    {
        if (activePopUps.Count > 1 || activePopUps[0] != Popups.Null)
        {
            if (activePopUps.Contains(Popups.ChooseImagePopUp))
            {
                FindAnyObjectByType<ChooseImagePopUpController>().RefreshForAd();
            }
            else if (activePopUps.Contains(Popups.ChooseEffectPopUp))
            {
                FindAnyObjectByType<ChooseEffectPopUpController>().RefreshForAd();
            }
            else if (activePopUps.Contains(Popups.ThemesPopUp))
            {
                FindAnyObjectByType<ThemesPopUpController>().RefreshForAd();
            }
            else if (activePopUps.Contains(Popups.GalleryPopUp))
            {
                FindAnyObjectByType<GalleryPopUp>().RefreshForAd();
            }
        }
        if (EffectsController.Inst != null && EffectsController.Inst.gameObject.activeSelf)
            EffectsController.Inst.RefreshForAd();
        else if (PupilController.Inst != null && PupilController.Inst.gameObject.activeSelf)
            PupilController.Inst.RefreshForAd();
        else if (PatternController.Inst != null && PatternController.Inst.gameObject.activeSelf)
            PatternController.Inst.RefreshForAd();
        else if (HighlightController.Inst != null && HighlightController.Inst.gameObject.activeSelf)
            HighlightController.Inst.RefreshForAd();
        else if (gamePlayUi.lensController != null && gamePlayUi.lensController.gameObject.activeSelf)
            gamePlayUi.lensController.RefreshForAd();
    }

    internal float GetDifferenceFromTop() => 1920 - 1920 * coinsRT.parent.GetComponent<RectTransform>().anchorMax.y;

    internal float GetDifferenceFromBottom() => 1920 * coinsRT.parent.GetComponent<RectTransform>().anchorMin.y;

    internal Transform GetCoinTarget() => coinsRT.GetChild(1);

    internal Texture2D RenderTextureToTexture2D(RenderTexture sourceTexture)
    {
        Texture2D tex = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = sourceTexture;
        tex.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }

    internal void SetCoinsText(string text)
    {
        coinText.text = text;
        coinsRT.sizeDelta = new Vector2(Mathf.Min(coinText.preferredWidth + 149, 250), coinsRT.rect.height);
    }

    internal void SetActiveFillCoin(bool show)
    {
        coinsRT.GetComponent<Image>().sprite = show ? coinsFillSp : coinsUnfillSp;
    }

    internal void SetActiveCoins(bool active)
    {
        coinsRT.gameObject.SetActive(active);
    }

    internal void SetCoinParentAbovePopUp(bool above)
    {
        if (above)
        {
            if (coinsRT.parent.GetSiblingIndex() < popUpParent.GetSiblingIndex())
                coinsRT.parent.SetSiblingIndex(popUpParent.GetSiblingIndex() + 1);
        }
        else
        {
            if (coinsRT.parent.GetSiblingIndex() > popUpParent.GetSiblingIndex())
                coinsRT.parent.SetSiblingIndex(popUpParent.GetSiblingIndex());
        }
    }

    internal bool IsMouseOverAnyUi()
    {
        PointerEventData pointerEventData = new(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerEventData, results);
        return results.Count > 0;
    }

    internal void ShowClickParticle()
    {
        if (canShowClickParticle)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            clickParticleParentTR.position = worldPos;
            clickParticleParentTR.gameObject.SetActive(false);
            clickParticleParentTR.gameObject.SetActive(true);
        }
        canShowClickParticle = true;
    }

    internal void SetSelectedClickParticle()
    {
        clickParticleParentTR.position = new Vector3(0, 8, 0);
        canShowClickParticle = false;
        foreach (Transform tr in clickParticleParentTR)
            Destroy(tr.gameObject);
        if (GeneralDataManager.SelectedClickParticleIndex != -1)
            Instantiate(Resources.Load<Transform>("Prefabs/ClickParticles/" + GeneralDataManager.SelectedClickParticleIndex), clickParticleParentTR);
    }

    internal void Show_Toast(string message)
    {
        if (toast) Destroy(toast);
        toast = Instantiate(toastPrefab, popUpParent.parent);
        toast.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
    }

    internal void Show_AdLoader_Panel(bool isShow)
    {
        adLoaderPanel.SetActive(isShow);
    }

    internal void On_Rate_Btn_Click()
    {
        AdsManager.Inst.CanShowAppOpen = false;
#if UNITY_IPHONE
        Application.OpenURL(GeneralDataManager.iPhoneShareLink);
#else
        Application.OpenURL(GeneralDataManager.androidShareLink);
#endif
        if (Is_Internet_Available()) GeneralDataManager.UserAlreadyGiveRating = true;
    }

    internal void Show_Rate_Popup()
    {
        const string saveKey = "LastRatePopupShowDate";
        if (GeneralDataManager.UserAlreadyGiveRating) return;
        if (!Is_Internet_Available()) return;
        var timeBetweenTwoDates = DateTime.Now - DateTime.ParseExact(PlayerPrefs.GetString(saveKey), "dd/MM/yyyy", CultureInfo.InvariantCulture);
        if (Mathf.Abs(timeBetweenTwoDates.Days) < 3) return;
        PlayerPrefs.SetString(saveKey, DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
        StartCoroutine(ShowRateUs());
        IEnumerator ShowRateUs()
        {
            yield return new WaitUntil(() => popUpParent.childCount == 0);
            Show_Popup(Popups.RateUs);
        }
    }

    public void On_Coins_Btn_Click()
    {
        if (Input.touchCount > 1)
            return;

        if (UserTutorialController.Inst != null)
        {
            Show_Toast("Please complete tutorial first!");
            return;
        }
        Show_Popup(Popups.StorePopUp, false);
    }

}
