using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using UnityEngine;

public class PupilController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRT;
    internal static PupilController Inst;
    internal int selectedPupilIndex = -1, selectedPupilIndexForUnlock;
    internal List<PupilItem> pupilItems = new();
    internal List<int> unlockedPupils;

    private void Awake()
    {
        Inst = this;
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

        rt.DOAnchorPosY(AdsManager.Inst.isNativeAdLoaded ? 565f : 395, 0.3f).OnComplete(() =>
        {
            if (UserTutorialController.Inst != null)
            {
                UserTutorialController.Inst.SetActiveHand(true);
                UserTutorialController.Inst.handRT.position = contentRT.GetChild(1).position;
                UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
            }
        });

        if (UserTutorialController.Inst == null)
        {
            GameManager.Inst.gamePlayUi.RefreshPupil(selectedPupilIndex);
        }
        else
        {
            selectedPupilIndex = -1;
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

    internal void RefreshForNativeAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        if (AdsManager.Inst.isNativeAdLoaded)
        {
            rt.DOAnchorPosY(565f, 0.2f).OnComplete(() =>
            {
                if (UserTutorialController.Inst != null)
                {
                    UserTutorialController.Inst.handRT.position = contentRT.GetChild(1).position;
                    UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
                }
            });
        }
        else
        {
            rt.DOAnchorPosY(395f, 0.2f).OnComplete(() =>
            {
                if (UserTutorialController.Inst != null)
                {
                    UserTutorialController.Inst.handRT.position = contentRT.GetChild(1).position;
                    UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
                }
            });
        }
    }
}
