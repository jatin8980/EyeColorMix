using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRT;
    internal static HighlightController Inst;
    internal int selectedHighlightIndex = -1;
    internal List<HighlightItem> highlightItems = new();

    private void Awake()
    {
        Inst = this;
    }

    internal void OnEnable()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -20);

        if (highlightItems.Count == 0)
            return;

        if (GeneralDataManager.IsChoiceSave)
            selectedHighlightIndex = GeneralDataManager.SelectedHighlightIndex;
        else
            selectedHighlightIndex = GeneralDataManager.Inst.orderToShowHighlights[0];

        if (selectedHighlightIndex == -1)
            selectedHighlightIndex = GeneralDataManager.Inst.orderToShowHighlights[0];
        float y = AdsManager.Inst.isBannerLoaded ? 395f + GameManager.Inst.bannerHeight : 395f;
        rt.DOAnchorPosY(y, 0.3f).OnComplete(() =>
        {
            if (UserTutorialController.Inst != null)
            {
                UserTutorialController.Inst.SetActiveHand(true);
                UserTutorialController.Inst.handRT.position = contentRT.GetChild(2).position;
                UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
            }
        });
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);

        if (UserTutorialController.Inst == null)
            GameManager.Inst.gamePlayUi.RefreshHighlight(selectedHighlightIndex);
        else
        {
            selectedHighlightIndex = -1;
            return;
        }

        if (contentRT.childCount > 1)
        {
            float viewPortSize = contentRT.parent.GetComponent<RectTransform>().rect.width;
            float width = highlightItems[0].GetComponent<RectTransform>().rect.width;
            contentRT.anchoredPosition = new Vector2(-Mathf.Min(contentRT.rect.width - viewPortSize, (GeneralDataManager.Inst.orderToShowHighlights.IndexOf(selectedHighlightIndex) + 1) * width), contentRT.anchoredPosition.y);
        }
        contentRT.parent.parent.GetComponent<DynamicScrollView>().refresh();
    }

    internal void RefreshAtIndex(int index)
    {
        foreach (HighlightItem highlightItem in highlightItems)
        {
            if (highlightItem.highlightIndex == index)
            {
                highlightItem.SetThis();
                break;
            }
        }
    }

    internal void RefreshForAd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOKill();
        float y = AdsManager.Inst.isBannerLoaded ? 395f + GameManager.Inst.bannerHeight : 395f;
        rt.DOAnchorPosY(y, 0.2f).OnComplete(() =>
        {
            if (UserTutorialController.Inst != null)
            {
                UserTutorialController.Inst.handRT.position = contentRT.GetChild(2).position;
                UserTutorialController.Inst.handRT.anchoredPosition += new Vector2(110, 0);
            }
        });
        GameManager.Inst.gamePlayUi.nextBtnParentRT.anchoredPosition = new Vector2(0, y + 100);
    }
}