using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreenController : MonoBehaviour
{
    [SerializeField] private List<Sprite> modelCloseEyes;
    [SerializeField] private GameObject removeAdsBtnOb, galleryBtnOb;
    [SerializeField] private Text leveText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup messageCG;
    [SerializeField] private AnimationClip eyeAnimWithBlinkAnimClip, eyeBlinkAnimClip;
    [SerializeField] private Animation characterAnim;
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
        characterIndex = UnityEngine.Random.Range(0, totalCharactersCount);
        SetCharacter();
        RefreshRemoveAdsBtn();
        characterTR.DOKill();
        characterTR.localScale = Vector3.one * GameManager.Inst.homeScreen.CharacterScale();
        characterTR.localPosition = new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0);
        Invoke(nameof(ShowMessage), 0.5f);
        if ((DateTime.Now - DateTime.ParseExact(GeneralDataManager.DailyRewardClaimedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).Days > 0)
            GameManager.Inst.Show_Popup(GameManager.Popups.DailyRewardPopUp);
    }

    private void OnEnable()
    {
        GameManager.Inst.SetActiveFillCoin(false);
        leveText.text = "Level " + GeneralDataManager.Level.ToString();
        galleryBtnOb.SetActive(GeneralDataManager.Level > 1);
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
        messageCG.DOKill();
        messageCG.alpha = 0;
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
            messageCG.DOFade(0, 0.5f).SetDelay(4);
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

    internal void ChangeCharacter()
    {
        PlayOrStopCharacterAnim(false);
        Transform oldCharacterTr = Instantiate(characterTR, characterTR.parent);
        characterTR.DOKill();
        characterTR.position = new Vector3(-8, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0);
        characterTR.localScale = GameManager.Inst.homeScreen.CharacterScale() * Vector3.one;
        oldCharacterTr.DOLocalMove(new Vector3(8, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f).OnComplete(() =>
        {
            Destroy(oldCharacterTr.gameObject);
        });
        characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.7f).SetDelay(0.6f).OnComplete(() =>
        {
            PlayOrStopCharacterAnim(true);
            ShowMessage();
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
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameManager.Inst.Show_Screen(GameManager.Screens.None);
        GameManager.Inst.Show_Popup(GameManager.Popups.ChooseImagePopUp);
        messageCG.DOKill();
        messageCG.DOFade(0, 0.3f);
    }

    public void On_Gallery_Btn_Click()
    {
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameManager.Inst.Show_Popup(GameManager.Popups.GalleryPopUp);
    }

    public void On_Shop_Btn_Click()
    {
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameManager.Inst.Show_Popup(GameManager.Popups.ShopPopUp);
    }

    public void On_Setting_Btn_Click()
    {
        GameManager.Inst.Show_Popup(GameManager.Popups.Setting);
    }

    public void On_Remove_Ads_Btn_Click()
    {
        GameManager.Inst.Show_Popup(GameManager.Popups.RemoveAdsPopUp);
    }
}