using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreenController : MonoBehaviour
{
    [SerializeField] private List<Sprite> modelCloseEyes;
    [SerializeField] private GameObject removeAdsBtnOb, galleryBtnOb, firstTimeUiOb, createBtnOb;
    [SerializeField] private Text leveText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup messageCG;
    [SerializeField] private AnimationClip eyeAnimWithBlinkAnimClip, eyeBlinkAnimClip;
    [SerializeField] private Animation characterAnim;
    [SerializeField] private RectTransform shutterLoadingRT;
    public Transform characterTR, leftEyeParent, rightEyeParent;
    public List<Sprite> modelBodies;
    internal int characterIndex, totalCharactersCount = 10;
    private List<string> colorNames = new() { "pink", "red", "green", "yellow", "blue", "purple", "orange", "rainbow" };
    private List<string> typeNames = new() { "amazing", "cool", "beautiful" };
    private List<string> messages = new()
    {
        "<color=#6055CE>Hey...</color>\nlet's create #R lens!",
        "<color=#6055CE>Helloww...</color>\nI would love to wear #R lens!",
        "<color=#6055CE>Ummm...</color>\ntwo #R lens please!",
        "Can you make #R lens for me?"
    };

    private void Start()
    {
        GameManager.Inst.gamePlayUi.pupilIcons = Resources.LoadAll<Sprite>("Sprites/Pupils/").OrderBy(item => Convert.ToInt32(item.name)).ToList();
        GameManager.Inst.gamePlayUi.pencilController.SetTheme(GeneralDataManager.SelectedPenIndex);
        GameManager.Inst.gamePlayUi.lensController.SetLens(GeneralDataManager.SelectedLensIndex);
        GameManager.Inst.SetActiveCoins(true);
        GameManager.Inst.SetCoinsText(GeneralDataManager.Coins.ToString());
        Vibration.Init();
        firstTimeUiOb.SetActive(true);
        createBtnOb.SetActive(false);
        shutterLoadingRT.gameObject.SetActive(false);
        characterIndex = UnityEngine.Random.Range(0, totalCharactersCount);
        characterTR.localScale = Vector3.zero;
        RefreshRemoveAdsBtn();
        if ((DateTime.Now - DateTime.ParseExact(GeneralDataManager.DailyRewardClaimedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).Days > 0)
            GameManager.Inst.Show_Popup(GameManager.Popups.DailyRewardPopUp);
    }

    private void OnEnable()
    {
        GameManager.Inst.SetActiveFillCoin(false);
        leveText.text = "LVL " + GeneralDataManager.Level;
        galleryBtnOb.SetActive(GeneralDataManager.Level > 1);
    }

    private IEnumerator ShutterLoading()
    {
        GeneralDataManager.Inst.No_Click_Panel_On_Off(true);
        shutterLoadingRT.localScale = new Vector3(1, -1, 1);
        shutterLoadingRT.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        firstTimeUiOb.SetActive(false);
        createBtnOb.SetActive(true);
        SetCharacter();
        characterTR.DOKill();
        characterTR.localScale = Vector3.one * GameManager.Inst.homeScreen.CharacterScale();
        characterTR.localPosition = new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0);
        GameManager.Inst.Show_Screen(GameManager.Screens.None);
        GameManager.Inst.Show_Popup(GameManager.Popups.ChooseImagePopUp);
        transform.parent.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Backgrounds/HomeBG");
        yield return new WaitForSeconds(1f);
        shutterLoadingRT.localScale = Vector3.one;
        Animator leftAnim = shutterLoadingRT.GetChild(0).GetComponent<Animator>();
        Animator rightAnim = shutterLoadingRT.GetChild(1).GetComponent<Animator>();
        leftAnim.SetBool("Open", true);
        rightAnim.SetBool("Open", true);
        yield return new WaitForSeconds(1f);
        Destroy(shutterLoadingRT.gameObject);
        GeneralDataManager.Inst.No_Click_Panel_On_Off(false);
        yield return new WaitForSeconds(0.3f);
        if (GameManager.activeScreen == GameManager.Screens.None)
        {
            float diff = GameManager.Inst.GetDifferenceFromBottom() / 2f;
            messageCG.GetComponent<RectTransform>().anchoredPosition = new Vector2(199, -275 + diff);
            ShowMessage();
        }
    }

    private void SetCharacter()
    {
        CharacterDetails cd = GeneralDataManager.Inst.characterDetails[characterIndex];
        leftEyeParent.localPosition = cd.leftEyeCanvasPos;
        rightEyeParent.localPosition = cd.rightEyeCanvasPos;
        characterTR.GetChild(3).localPosition = cd.bodyPos;
        characterTR.GetChild(3).localScale = new Vector3(cd.bodyScale, cd.bodyScale, cd.bodyScale);
        characterTR.GetChild(3).GetComponent<SpriteRenderer>().sprite = modelBodies[characterIndex];
        characterTR.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().sprite = modelCloseEyes[characterIndex];
    }

    private void ShowMessage()
    {
        CancelInvoke(nameof(HideMessage));
        messageCG.DOKill();
        messageCG.alpha = 0;
        messageCG.gameObject.SetActive(true);
        int messageIndex = 3;
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.25)
            messageIndex = 0;
        else if (randomValue < 0.5)
            messageIndex = 1;
        else if (randomValue < 0.75f)
            messageIndex = 2;

        if (messageIndex == 1)
            messageText.text = messages[messageIndex].Replace("#R", colorNames[UnityEngine.Random.Range(0, colorNames.Count)]);
        else
            messageText.text = messages[messageIndex].Replace("#R", typeNames[UnityEngine.Random.Range(0, typeNames.Count)]);
        messageCG.DOFade(1, 0.5f).OnComplete(() =>
        {
            Invoke(nameof(HideMessage), 4f);
        });
    }

    internal void DoFadeCharacter(float target, float duration)
    {
        characterTR.GetComponent<CanvasGroup>().DOFade(target, duration);
        characterTR.GetChild(0).GetComponent<SpriteRenderer>().DOFade(target, duration);
        characterTR.GetChild(3).GetComponent<SpriteRenderer>().DOFade(target, duration);
        characterTR.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().DOFade(target, duration);
    }

    internal void PlayOrStopCharacterAnim(bool play)
    {
        if (play)
        {
            characterAnim.clip = characterAnim.GetClip("EyeAnimWithBlink");
            characterAnim.Play();
        }
        else
        {
            characterAnim.Stop();
            characterTR.GetChild(3).GetChild(0).gameObject.SetActive(false);
            characterTR.GetChild(1).localPosition = characterTR.GetChild(2).localPosition = Vector3.zero;
        }
    }

    internal void PlayBlinkAnim(bool play)
    {
        if (play)
        {
            characterAnim.clip = characterAnim.GetClip("EyeBlinkAnim");
            characterAnim.Play();
        }
        else
        {
            characterAnim.Stop();
            characterTR.GetChild(3).GetChild(0).gameObject.SetActive(false);
            characterTR.GetChild(1).localPosition = characterTR.GetChild(2).localPosition = Vector3.zero;
        }
    }

    internal void HideMessage()
    {
        messageCG.DOKill();
        messageCG.DOFade(0, 0.3f).OnComplete(() =>
        {
            messageCG.gameObject.SetActive(false);
        });
    }

    internal void HideCreateBtn()
    {
        createBtnOb.transform.DOKill();
        createBtnOb.transform.localScale = Vector3.zero;
    }

    internal void ChangeCharacter()
    {
        PlayOrStopCharacterAnim(false);
        Transform oldCharacterTr = Instantiate(characterTR, characterTR.parent);
        characterTR.DOKill();
        characterTR.position = new Vector3(-8.5f, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0);
        characterTR.localScale = GameManager.Inst.homeScreen.CharacterScale() * Vector3.one;
        oldCharacterTr.DOLocalMove(new Vector3(8.5f, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f).OnComplete(() =>
        {
            Destroy(oldCharacterTr.gameObject);
        });
        characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f).SetDelay(0.6f).OnComplete(() =>
        {
            PlayOrStopCharacterAnim(true);
            float diff = GameManager.Inst.GetDifferenceFromBottom() / 2f;
            messageCG.GetComponent<RectTransform>().anchoredPosition = new Vector2(199, -77 + diff);
            ShowMessage();
            createBtnOb.transform.DOScale(1, 0.15f).SetEase(Ease.OutBack);
        });
        SetCharacter();
    }

    internal void SetActiveEye(bool activeSelf)
    {
        if (leftEyeParent.childCount > 1)
        {
            leftEyeParent.GetChild(1).gameObject.SetActive(activeSelf);
            rightEyeParent.GetChild(1).gameObject.SetActive(activeSelf);
        }
    }

    internal float CharacterScale()
    {
        float totalDiff = GameManager.Inst.GetDifferenceFromBottom() + GameManager.Inst.GetDifferenceFromTop();
        totalDiff /= 1920;
        return 1 - totalDiff;
    }

    internal void RefreshRemoveAdsBtn()
    {
        removeAdsBtnOb.SetActive(!GeneralDataManager.IsPurchaseAdsRemoved);
    }

    public void On_Create_Btn_Click()
    {
        if (!GameManager.Is_Internet_Available())
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.NoInternetPopUp, false);
            return;
        }
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded", () =>
        {
            if (firstTimeUiOb.activeSelf)
            {
                GameManager.Inst.StartCoroutine(ShutterLoading());
            }
            else
            {
                HideMessage();
                GameManager.Inst.Show_Screen(GameManager.Screens.None);
                GameManager.Inst.Show_Popup(GameManager.Popups.ChooseImagePopUp);
            }
        });
    }

    public void On_Gallery_Btn_Click()
    {
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameManager.Inst.Show_Popup(GameManager.Popups.GalleryPopUp);
    }

    public void On_Shop_Btn_Click()
    {
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameManager.Inst.Show_Popup(GameManager.Popups.ThemesPopUp);
    }

    public void On_Setting_Btn_Click()
    {
        GameManager.Inst.Show_Popup(GameManager.Popups.Setting);
    }

    public void On_Remove_Ads_Btn_Click()
    {
        GameManager.Inst.Show_Popup(GameManager.Popups.StorePopUp);
    }
}