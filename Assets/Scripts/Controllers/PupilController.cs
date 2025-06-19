using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PupilController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRT;
    [SerializeField] private Slider slider;
    [SerializeField] private Material fadeOutlineMaterial;
    internal static PupilController Inst;
    internal int selectedPupilIndex = -1, selectedPupilIndexForUnlock;
    internal List<PupilItem> pupilItems = new();
    internal List<int> unlockedPupils;

    private void Awake()
    {
        Inst = this;
        fadeOutlineMaterial.SetFloat("_BlurAmount", 0f);
        RectTransform sliderParentRT = slider.transform.parent.GetComponent<RectTransform>();
        if (sliderParentRT.rect.width > 900)
        {
            float diff = sliderParentRT.rect.width - 900;
            sliderParentRT.offsetMax = new Vector2(-diff / 2f, sliderParentRT.offsetMax.y);
            sliderParentRT.offsetMin = new Vector2(diff / 2f, sliderParentRT.offsetMin.y);
        }
    }

    internal void OnEnable()
    {
        unlockedPupils = GeneralDataManager.UnlockedPupils;
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -20);
        if (pupilItems.Count == 0)
            return;

        if (GeneralDataManager.IsChoiceSave)
            selectedPupilIndex = GeneralDataManager.SelectedPupilIndex;
        else
            selectedPupilIndex = GeneralDataManager.Inst.orderToShowPupils[0];

        float y = AdsManager.Inst.isBannerLoaded ? 537f + GameManager.Inst.bannerHeight : 537f;
        rt.DOAnchorPosY(y, 0.3f).OnComplete(() =>
        {
            if (UserTutorialController.Inst != null)
            {
                UserTutorialController.Inst.SetActiveHand(true);
                UserTutorialController.Inst.handRT.position = contentRT.GetChild(1).position;
                UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
            }
        });
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);
        if (UserTutorialController.Inst == null)
        {
            GameManager.Inst.gamePlayUi.RefreshPupil(selectedPupilIndex);
        }
        else
        {
            selectedPupilIndex = -1;
            slider.value = 0;
            return;
        }


        float viewPortSize = contentRT.parent.GetComponent<RectTransform>().rect.width;
        float width = pupilItems[0].GetComponent<RectTransform>().rect.width;
        contentRT.anchoredPosition = new Vector2(-Mathf.Min(contentRT.rect.width - viewPortSize, GeneralDataManager.Inst.orderToShowPupils.IndexOf(selectedPupilIndex) * width), contentRT.anchoredPosition.y);

        contentRT.parent.parent.GetComponent<DynamicScrollView>().refresh();
    }

    internal void RefreshAtIndex(int index)
    {
        foreach (PupilItem pupilItem in pupilItems)
        {
            if (pupilItem.pupilIndex == index)
            {
                pupilItem.SetThis();
                break;
            }
        }
    }

    internal void UnlockPupilCallBack()
    {
        unlockedPupils.Add(selectedPupilIndexForUnlock);
        GeneralDataManager.UnlockedPupils = unlockedPupils;
        GameManager.Inst.Show_Toast("Pupil unlocked!");
        foreach (PupilItem pupilItem in pupilItems)
        {
            if (pupilItem.pupilIndex == selectedPupilIndexForUnlock)
            {
                pupilItem.On_Pupil_Btn_Click();
                break;
            }
        }
    }

    internal void ResetSlider() => slider.value = 0;

    internal void RefreshForAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        float y = AdsManager.Inst.isBannerLoaded ? 537f + GameManager.Inst.bannerHeight : 537f;
        rt.DOAnchorPosY(y, 0.2f).OnComplete(() =>
        {
            if (UserTutorialController.Inst != null)
            {
                UserTutorialController.Inst.handRT.position = contentRT.GetChild(1).position;
                UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
            }
        });
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);
    }

    public void On_SliderValue_Change()
    {
        fadeOutlineMaterial.SetFloat("_BlurAmount", slider.value);
    }
}
