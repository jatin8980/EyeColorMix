using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatternController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRT, colorHandleRT;
    internal static PatternController Inst;
    internal int selectedPatternIndex = -1, selectedPatternIndexForUnlock;
    internal List<PatternItem> patternItems = new();
    [SerializeField] private Slider amountSlider, colorSlider, opacitySlider;
    [SerializeField] private Texture2D hueTexture;
    [SerializeField] private Image opacityImage;
    internal List<int> unlockedPatterns;

    private void Awake()
    {
        Inst = this;
        RectTransform sliderParentRT = amountSlider.transform.parent.GetComponent<RectTransform>();
        if (sliderParentRT.rect.width > 900)
        {
            float diff = sliderParentRT.rect.width - 900;
            sliderParentRT.offsetMax = new Vector2(-diff / 2f, sliderParentRT.offsetMax.y);
            sliderParentRT.offsetMin = new Vector2(diff / 2f, sliderParentRT.offsetMin.y);
        }
    }

    internal void OnEnable()
    {
        unlockedPatterns = GeneralDataManager.UnlockedPatterns;
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -20);
        if (patternItems.Count == 0)
        {
            GameManager.Inst.gamePlayUi.patternGameScrollView.InitializeItems();
            return;
        }

        if (GeneralDataManager.IsChoiceSave)
            selectedPatternIndex = GeneralDataManager.SelectedPatternIndex;
        else
            selectedPatternIndex = GeneralDataManager.Inst.orderToShowPattern[0];

        if (selectedPatternIndex == -1)
            selectedPatternIndex = GeneralDataManager.Inst.orderToShowPattern[0];

        float y = AdsManager.Inst.isBannerLoaded ? 537f + GameManager.Inst.bannerHeight : 537f;
        rt.DOAnchorPosY(y, 0.3f);
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);

        ApplyPattern(selectedPatternIndex);

        float viewPortSize = contentRT.parent.GetComponent<RectTransform>().rect.width;
        float width = patternItems[0].GetComponent<RectTransform>().rect.width;
        contentRT.anchoredPosition = new Vector2(-Mathf.Min(contentRT.rect.width - viewPortSize, (GeneralDataManager.Inst.orderToShowPattern.IndexOf(selectedPatternIndex) + 1) * width), contentRT.anchoredPosition.y);

        contentRT.parent.parent.GetComponent<DynamicScrollView>().refresh();
    }

    internal void RefreshAtIndex(int index)
    {
        foreach (PatternItem patternItem in patternItems)
        {
            if (patternItem.patternIndex == index)
            {
                patternItem.SetThis();
                break;
            }
        }
    }

    internal void RefreshForAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        float y = AdsManager.Inst.isBannerLoaded ? 537f + GameManager.Inst.bannerHeight : 537f;
        rt.DOAnchorPosY(y, 0.2f);
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);
    }

    internal void ApplyPattern(int patternIndex)
    {
        if (GeneralDataManager.IsChoiceSave)
            GeneralDataManager.SelectedPatternIndex = patternIndex;
        else
            GeneralDataManager.SelectedPatternIndex = 0;


        GameManager.Inst.gamePlayUi.PlayItemAppliedEffect();
        if (patternIndex == -1)
        {
            GameManager.Inst.gamePlayUi.patternGameScrollView.gameObject.SetActive(false);
            amountSlider.interactable = colorSlider.interactable = opacitySlider.interactable = false;
            amountSlider.minValue = 0;
            amountSlider.maxValue = 2;
            amountSlider.value = 0;
            colorSlider.value = 0;
            opacitySlider.value = 0;
        }
        else
        {
            PatternData patternData = Resources.Load<PatternData>("ScriptableObjects/PatternData/" + patternIndex);
            GameManager.Inst.gamePlayUi.patternGameScrollView.SetSprites(patternData);
            GameManager.Inst.gamePlayUi.patternGameScrollView.gameObject.SetActive(true);
            amountSlider.interactable = colorSlider.interactable = opacitySlider.interactable = true;
            amountSlider.maxValue = patternData.maxSliderValue;
            amountSlider.minValue = patternData.minSliderValue;
            if (amountSlider.value == patternData.defaultValue)
            {
                On_AmountSlider_Value_Change();
            }
            else
            {
                amountSlider.value = patternData.defaultValue;
            }
            colorSlider.value = 0;
            opacitySlider.value = 0;
        }

        if (!GeneralDataManager.IsPatternTutorialShowed)
        {
            amountSlider.value = amountSlider.minValue;
            Transform handleRt = amountSlider.transform.GetChild(1).GetChild(0);
            UserTutorialController.Inst.handRT.position = handleRt.position;
            UserTutorialController.Inst.SetActiveHand(true);
            amountSlider.DOValue(amountSlider.maxValue, 3).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).OnUpdate(() =>
            {
                UserTutorialController.Inst.handRT.position = handleRt.position;
            });
        }
        else
        {
            amountSlider.DOKill();
        }
    }

    internal void UnlockPatternCallBack()
    {
        unlockedPatterns.Add(selectedPatternIndexForUnlock);
        GeneralDataManager.UnlockedPatterns = unlockedPatterns;
        GameManager.Inst.Show_Toast("Pattern unlocked!");
        foreach (PatternItem patternItem in patternItems)
        {
            if (patternItem.patternIndex == selectedPatternIndexForUnlock)
            {
                patternItem.On_Pattern_Btn_Click();
                break;
            }
        }
    }

    public void On_AmountSlider_Value_Change()
    {
        if (!amountSlider.interactable)
            return;

        GameManager.Inst.gamePlayUi.patternGameScrollView.SetAmount((int)amountSlider.value);

    }

    public void On_ColorSlider_Value_Change()
    {
        opacityImage.color = hueTexture.GetPixel(Mathf.FloorToInt(hueTexture.width * colorSlider.value), 2);
        GameManager.Inst.gamePlayUi.patternGameScrollView.SetImageColor(opacityImage.color);
        if ((int)(colorSlider.value * 100) % 10 == 0)
        {
            SoundManager.Inst.LightVibrate();
        }
    }

    public void On_OpacitySlider_Value_Change()
    {
        GameManager.Inst.gamePlayUi.patternGameScrollView.SetOpacity(1 - opacitySlider.value);
        if ((int)(opacitySlider.value * 100) % 10 == 0)
        {
            SoundManager.Inst.LightVibrate();
        }
    }

    public void OnMouseDownOnSlider()
    {
        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.IsPatternTutorialShowed = true;
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
            amountSlider.DOKill();
        }
    }
}
