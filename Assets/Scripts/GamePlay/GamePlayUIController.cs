using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    [SerializeField] private RectTransform eyeRT, levelProgressParentRT, currentTaskBulletRT, nextBtnRT, itemAppliedParticleRT;
    [SerializeField] private GameObject pupilsOb, effectsOb, patternOb, highlightOb, headerOb, removeAdsBtnOb;
    [SerializeField] private Image pupilImage, highlightImage, upBackground, downBackground, itemAppliedRingImage;
    [SerializeField] private Button nextBtn, undoBtn;
    //[SerializeField] private Text autoStretchCountOrAdText;
    public ColorSelector colorSelector;
    public RectTransform eyeParent;
    public CircularScrollView patternGameScrollView;
    public DrawScript drawController;
    public LensController lensController;
    public PencilController pencilController;
    public Image combinedCirlcesImg;
    internal List<Sprite> pupilIcons = new(), highlightIcons = new();
    internal int currentStepCount = 1, itemSpacing = 20;
    internal float smallScale = -1, bigScale;
    private Color32 defaultUpColor = new(240, 244, 244, 255), defaultDownColor = new(228, 228, 228, 255);
    private bool isRateUsShowed;
    private Texture2D capturedEyeTex;

    private void OnEnable()
    {
        GameManager.Inst.SetActiveFillCoin(true);
        if (smallScale > 0)
            ResetLevel();
    }

    private void Start()
    {
        highlightIcons = Resources.LoadAll<Sprite>("Sprites/Highlights/").OrderBy(item => Convert.ToInt32(item.name)).ToList();
        pencilController.SetDefaultPoses();
        RectTransform drawableCircleAreaRT = drawController.transform.parent.GetComponent<RectTransform>();
        smallScale = Mathf.Min(drawableCircleAreaRT.rect.width, drawableCircleAreaRT.rect.height) / 864f;
        bigScale = Mathf.Min(drawableCircleAreaRT.rect.width + 80, drawableCircleAreaRT.rect.height) / 864f;
        OnEnable();
        RefreshRemoveAdsBtn();
        if (GeneralDataManager.TutorialStep < 5)
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
        }
        //SetAutoStretchText();
    }

    private void SetCurrentTaskBulletPos(Vector3 worldPos)
    {
        currentTaskBulletRT.position = worldPos;
        currentTaskBulletRT.anchoredPosition = new Vector2(currentTaskBulletRT.anchoredPosition.x, 4);
    }

    private IEnumerator CaptureEyeScreenshot(RectTransform targetRT)
    {
        yield return new WaitForEndOfFrame();
        var cam = Camera.main;
        var corners = new Vector3[4];
        targetRT.GetWorldCorners(corners);
        var bl = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        var tl = RectTransformUtility.WorldToScreenPoint(cam, corners[1]);
        var tr = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        var height = tl.y - bl.y;
        var width = tr.x - bl.x;

        var tex = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        var rex = new Rect(bl.x, bl.y, width, height);
        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        RenderTexture renderTexture = RenderTexture.GetTemporary(200, 200);
        Graphics.Blit(tex, renderTexture);
        capturedEyeTex = GameManager.Inst.RenderTextureToTexture2D(renderTexture);
        RenderTexture.ReleaseTemporary(renderTexture);
    }

    /*private void SetAutoStretchText()
    {
        autoStretchCountOrAdText.text = GeneralDataManager.AutoStretchPowerCount > 0 ? GeneralDataManager.AutoStretchPowerCount.ToString() : "AD";
    }*/

    private void EnableNextBtn()
    {
        NextBtnActiveSelf(true);
    }

    internal void UndoBtnSetActive(bool show)
    {
        undoBtn.gameObject.SetActive(show);
    }

    internal void NextBtnActiveSelf(bool show)
    {
        nextBtn.gameObject.SetActive(show);
        if (show)
        {
            nextBtn.transform.DOKill();
            nextBtn.transform.localScale = Vector3.zero;
            nextBtn.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    /*internal void AutoStretchBtnSetActive(bool show)
    {
        Transform btnTR = autoStretchCountOrAdText.transform.parent.parent;
        btnTR.gameObject.SetActive(show);
        if (show)
        {
            btnTR.DOKill();
            btnTR.localScale = Vector3.zero;
            btnTR.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }*/

    internal void RefreshRemoveAdsBtn()
    {
        removeAdsBtnOb.SetActive(!GeneralDataManager.IsPurchaseAdsRemoved);
    }

    internal Vector3 GetNextButtonPosition() => nextBtnRT.position;

    internal void EffectsSetActive(bool show)
    {
        effectsOb.SetActive(show);
        pupilsOb.SetActive(false);
        if (show)
        {
            if (GeneralDataManager.TutorialStep == 11)
            {
                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            }
            else
            {
                Invoke(nameof(EnableNextBtn), 0.5f);
            }
        }
    }

    internal void PatternSetActive(bool show)
    {
        patternOb.SetActive(show);
        pupilsOb.SetActive(false);
        if (show)
        {
            if (!GeneralDataManager.IsPatternTutorialShowed)
            {
                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            }
            else
            {
                Invoke(nameof(EnableNextBtn), 0.5f);
            }
        }
    }

    internal void RefreshPupil(int currentSelectedPupilIndex)
    {        
        if (GeneralDataManager.IsChoiceSave)
            GeneralDataManager.SelectedPupilIndex = currentSelectedPupilIndex;
        else
            GeneralDataManager.SelectedPupilIndex = 0;

        PlayItemAppliedEffect();
        pupilImage.sprite = pupilIcons[currentSelectedPupilIndex];
        pupilImage.transform.DOKill();
        pupilImage.transform.localScale = Vector3.zero;
        pupilImage.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    internal void RefreshHighlight(int currentSelectedHighlightIndex)
    {
        if (GeneralDataManager.IsChoiceSave)
            GeneralDataManager.SelectedHighlightIndex = currentSelectedHighlightIndex;
        else
            GeneralDataManager.SelectedHighlightIndex = 0;

        PlayItemAppliedEffect();
        if (currentSelectedHighlightIndex == -1)
        {
            highlightImage.DOFade(0, 0.2f);
        }
        else
        {
            highlightImage.sprite = highlightIcons[currentSelectedHighlightIndex];
            highlightImage.DOKill();
            highlightImage.color = new Color(1, 1, 1, 0);
            highlightImage.DOFade(1, 0.5f);
        }
    }

    internal void SaveCurrentLevelToGallery()
    {
        List<List<Color32>> colorsUsedInLevels = GeneralDataManager.ListOfColorsUsedInLevel;
        colorsUsedInLevels.Add(GameManager.Inst.gamePlayUi.drawController.colorsToShowInGallery);
        GeneralDataManager.ListOfColorsUsedInLevel = colorsUsedInLevels;

        List<int> characterPlayedInLevel = GeneralDataManager.ListOfCharacterPlayedInLevel;
        characterPlayedInLevel.Add(GameManager.Inst.homeScreen.characterIndex);
        GeneralDataManager.ListOfCharacterPlayedInLevel = characterPlayedInLevel;
        string path = Application.persistentDataPath + "/Eyes";
        try
        {
            File.SetAttributes(path, FileAttributes.Normal);
            File.WriteAllBytes(path + "/" + colorsUsedInLevels.Count + ".png", capturedEyeTex.EncodeToPNG());
        }
        catch (Exception ex) { }
    }

    internal void PlayItemAppliedEffect()
    {
        itemAppliedParticleRT.gameObject.SetActive(false);
        itemAppliedParticleRT.gameObject.SetActive(true);
        itemAppliedRingImage.DOKill();
        itemAppliedRingImage.transform.DOKill();
        itemAppliedRingImage.transform.localScale = Vector3.zero;
        itemAppliedRingImage.color = new(itemAppliedRingImage.color.r, itemAppliedRingImage.color.g, itemAppliedRingImage.color.b, 1);
        itemAppliedRingImage.transform.DOScale(1, 0.7f).SetEase(Ease.OutCubic);
        itemAppliedRingImage.DOFade(0, 0.2f).SetDelay(0.6f);
    }

    internal void ResetLevel()
    {
        highlightImage.color = new Color(1, 1, 1, 0);
        pupilImage.transform.localScale = Vector3.zero;
        pupilImage.gameObject.SetActive(true);
        highlightImage.gameObject.SetActive(true);
        SetCurrentTaskBulletPos(levelProgressParentRT.GetChild(0).position);
        upBackground.color = defaultUpColor;
        downBackground.color = defaultDownColor;
        pupilsOb.SetActive(false);
        PatternSetActive(false);
        EffectsSetActive(false);
        pencilController.gameObject.SetActive(true);
        colorSelector.gameObject.SetActive(true);
        eyeParent.gameObject.SetActive(false);
        drawController.transform.parent.gameObject.SetActive(true);
        lensController.gameObject.SetActive(false);
        nextBtnRT.anchoredPosition = new(0, 437);
        headerOb.SetActive(true);
        NextBtnActiveSelf(false);
        //AutoStretchBtnSetActive(false);
        levelProgressParentRT.gameObject.SetActive(true);
        currentStepCount = 1;
        patternGameScrollView.ClearList();
        GameManager.Inst.gamePlayUi.combinedCirlcesImg.material = null;
        colorSelector.enabled = true;
        itemAppliedParticleRT.anchoredPosition = new Vector2(0, 2000);
        itemAppliedParticleRT.gameObject.SetActive(false);
        pencilController.targetPos = pencilController.smoothTargetPos = colorSelector.transform.position;
        pencilController.MovePencilToTargetPos();
    }

    /*internal void AutoStretchPowerCallBack()
    {
        GeneralDataManager.AutoStretchPowerCount += 3;
        SetAutoStretchText();
    }*/

    public void On_Undo_Btn_Click()
    {
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameController.Inst.currentCircleIndex--;
        GameManager.Inst.gamePlayUi.drawController.On_Circle_Change(true);
        NextBtnActiveSelf(false);
    }

    public void On_Remove_Ads_Btn_Click()
    {
        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.Show_Toast("Please complete tutorial first!");
            return;
        }
        GameManager.Inst.Show_Popup(GameManager.Popups.RemoveAdsPopUp, false);
    }

    public void On_Pencil_Btn_Click()
    {
        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.Show_Toast("Please complete tutorial first!");
            return;
        }
        AdsManager.Inst.ShowInterstitialAd("ExtraLoaded");
        GameManager.Inst.Show_Popup(GameManager.Popups.ShopPopUp, false);
    }

   /* public void On_AutoStretchBtn_Click()
    {
        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.Show_Toast("Please complete tutorial first!");
            return;
        }
        if (GeneralDataManager.AutoStretchPowerCount > 0)
        {
            GeneralDataManager.AutoStretchPowerCount--;
            SetAutoStretchText();
            GameController.Inst.stretchController.StartCoroutine(GameController.Inst.stretchController.StretchRemaining());
        }
        else
        {
            AdsManager.Inst.RequestAndLoadRewardedAd("AutoStretchPower");
        }
    }*/

    public void On_Next_Btn_Click()
    {
        SetCurrentTaskBulletPos(levelProgressParentRT.GetChild(currentStepCount).position);

        if (currentStepCount == 3 && GeneralDataManager.Level == 1)
        {
            currentStepCount = 5;
            pupilsOb.SetActive(false);
            if (UserTutorialController.Inst != null)
            {
                GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                GeneralDataManager.TutorialStep++;// to 9
            }
        }
        else if (currentStepCount == 4 && GeneralDataManager.Level == 2)
        {
            currentStepCount = 5;
            EffectsSetActive(false);
        }

        if (currentStepCount == 1)
        {
            GameController.Inst.stretchController.gameObject.SetActive(true);
            GameController.Inst.beforeSRs[0].transform.parent.parent.DOScale(bigScale * new Vector3(2.25f, 2.25f, 2.25f), 0.5f);
            GameController.Inst.stretchController.transform.DOScale(bigScale * new Vector3(2.25f, 2.25f, 2.25f), 0.5f);
            GameController.Inst.stretchController.AssignRenderTextureUvsToMeshRenderer(drawController.renderTextures[GameController.Inst.currentCircleIndex], GameController.Inst.currentCircleIndex);
            GameController.Inst.stretchController.CombineAllCircles();
            drawController.transform.parent.gameObject.SetActive(false);
            NextBtnActiveSelf(false);
            GameController.Inst.currentCircleIndex = 0;
            colorSelector.enabled = false;
            pencilController.MovePencilToDefaultPos();
            GameManager.Inst.gamePlayUi.colorSelector.isColorChoosed = false;
            UndoBtnSetActive(false);
            GameController.Inst.BeforeSpriteSetActive(true);
            if (UserTutorialController.Inst != null)
            {
                GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                GeneralDataManager.TutorialStep++;// to 5
            }
            if (GeneralDataManager.TutorialStep == 5)
            {
                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            }
            //AutoStretchBtnSetActive(UserTutorialController.Inst == null);
        }
        else if (currentStepCount == 2)
        {
            GameController.Inst.BeforeSpriteSetActive(false);
            GameController.Inst.stretchController.gameObject.SetActive(false);
            eyeParent.gameObject.SetActive(true);
            eyeRT.position = drawController.transform.position;
            itemAppliedParticleRT.anchoredPosition = Vector2.zero;
            float size = Mathf.Min(eyeParent.rect.width, eyeParent.rect.height) - 16;
            eyeRT.DOKill();
            eyeRT.localScale = bigScale * Vector3.one;
            eyeRT.DOScale(size / 864 * Vector3.one, 0.5f);
            eyeRT.DOAnchorPos(Vector2.zero, 0.5f).OnComplete(() =>
            {
                if (GeneralDataManager.TutorialStep > 7)
                    NextBtnActiveSelf(true);
            });
            upBackground.DOColor(Color.white, 0.5f);
            downBackground.DOColor(Color.white, 0.5f);
            pupilsOb.SetActive(true);
            NextBtnActiveSelf(false);
            pencilController.gameObject.SetActive(false);
            colorSelector.gameObject.SetActive(false);
            nextBtnRT.anchoredPosition = new(0, 928);
            if (UserTutorialController.Inst != null)
            {
                GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
                GeneralDataManager.TutorialStep++;// to 7
            }

            if (GeneralDataManager.TutorialStep == 7)
            {
                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            }
            else if (!isRateUsShowed)
            {
                GameManager.Inst.Show_Rate_Popup();
                isRateUsShowed = true;
            }

        }
        else if (currentStepCount == 3)
        {
            pupilsOb.SetActive(false);
            NextBtnActiveSelf(false);
            GameManager.Inst.Show_Popup(GameManager.Popups.ChooseEffectPopUp);
        }
        else if (currentStepCount == 4)
        {
            EffectsSetActive(false);
            PatternSetActive(false);
            highlightOb.SetActive(true);
            NextBtnActiveSelf(false);
            if (GeneralDataManager.TutorialStep == 12)
            {
                GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            }
            else
            {
                Invoke(nameof(EnableNextBtn), 0.5f);
            }
        }
        else if (currentStepCount == 5)
        {
            StartCoroutine(CaptureEyeScreenshot(eyeRT));
            headerOb.SetActive(false);
            GameManager.Inst.SetActiveCoins(false);
            NextBtnActiveSelf(false);
            highlightOb.SetActive(false);
            lensController.gameObject.SetActive(true);
            Transform rightLens = Instantiate(eyeRT, lensController.rightLensParent);
            Transform leftLens = Instantiate(eyeRT, lensController.leftLensParent);
            leftLens.localScale = Vector3.zero;
            Destroy(rightLens.GetChild(1).GetComponent<CircularScrollView>());
            Destroy(leftLens.GetChild(1).GetComponent<CircularScrollView>());
            rightLens.GetChild(rightLens.childCount - 1).gameObject.SetActive(false);
            leftLens.GetChild(leftLens.childCount - 1).gameObject.SetActive(false);
            leftLens.localPosition = Vector3.zero;
            rightLens.position = eyeRT.position;
            lensController.RotateRightCap(() =>
            {
                rightLens.DOScale(rightLens.localScale * 1.05f, 0.2f);
                rightLens.DOLocalMove(rightLens.localPosition * 1.05f, 0.2f).OnComplete(() =>
                {
                    rightLens.DOScale(0.17f, 0.7f);
                    rightLens.DOLocalMove(Vector3.zero, 0.7f).OnComplete(() =>
                    {
                        lensController.currentLens = rightLens.parent;
                        lensController.SetActiveTarget(true);
                        leftLens.DOScale(0.17f, 0.3f);
                        if (GeneralDataManager.TutorialStep == 9)
                        {
                            GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
                            UserTutorialController.Inst.StartCoroutine(UserTutorialController.Inst.SetHandAnimForLens(lensController.rightLensParent.parent.position, lensController.GetTargetPos()));
                        }
                        if (!Input.GetMouseButton(0))
                        {
                            lensController.rightLensParent.GetComponent<Animation>().Play();
                            GameManager.Inst.homeScreen.PlayBlinkAnim(true);
                        }
                    });
                });
            });
            eyeParent.gameObject.SetActive(false);
        }
        currentStepCount++;
    }
}