using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatternController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRT, colorHandleRT, leftBottomRT;
    internal static PatternController Inst;
    internal int selectedPatternIndex = -1, selectedPatternIndexForUnlock;
    internal List<PatternItem> patternItems = new();
    [SerializeField] private Slider amountSlider, colorSlider;
    [SerializeField] private TextMeshProUGUI amountSliderText;
    [SerializeField] private Image colorSliderHandleImg, hueImage;
    internal List<int> unlockedPatterns;


    private void Awake()
    {
        Inst = this;
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

        rt.DOAnchorPosY(AdsManager.Inst.isNativeAdLoaded ? 707f : 537f, 0.3f);

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

    internal void RefreshForNativeAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        if (AdsManager.Inst.isNativeAdLoaded)
        {
            rt.DOAnchorPosY(707f, 0.2f);
        }
        else
        {
            rt.DOAnchorPosY(537f, 0.2f);
        }
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
            amountSlider.interactable = colorSlider.interactable = false;
            amountSlider.minValue = 0;
            amountSlider.maxValue = 2;
            amountSlider.value = 0;
            colorSlider.value = 0;
            colorSliderHandleImg.color = Color.white;
            amountSliderText.gameObject.SetActive(false);
        }
        else
        {
            PatternData patternData = Resources.Load<PatternData>("ScriptableObjects/PatternData/" + patternIndex);
            GameManager.Inst.gamePlayUi.patternGameScrollView.SetSprites(patternData);
            GameManager.Inst.gamePlayUi.patternGameScrollView.gameObject.SetActive(true);
            amountSlider.interactable = true;
            colorSlider.interactable = true;
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
            amountSliderText.gameObject.SetActive(true);
        }

        if (!GeneralDataManager.IsPatternTutorialShowed)
        {
            amountSlider.value = amountSlider.minValue;
            Transform handleRt = amountSlider.transform.GetChild(2).GetChild(0);
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
        amountSliderText.text = amountSlider.value.ToString();

    }

    public void On_ColorSlider_Value_Change()
    {
        leftBottomRT.position = colorHandleRT.position;
        float per = leftBottomRT.anchoredPosition.x / hueImage.GetComponent<RectTransform>().rect.width;
        Color color = hueImage.sprite.texture.GetPixel(Mathf.FloorToInt(hueImage.sprite.texture.width * per), 2);
        colorSliderHandleImg.color = color;
        GameManager.Inst.gamePlayUi.patternGameScrollView.SetImageColor(color);
        if ((int)(colorSlider.value * 100) % 10 == 0)
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
