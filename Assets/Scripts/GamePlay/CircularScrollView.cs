using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircularScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform contentParent, patternScrollViewItem;
    [SerializeField] private CanvasGroup mainCg;
    private PatternData patternData;
    private int currentAmount = 0;
    private float startOffset = 180f;
    private List<PatternScrollViewItem> patternScrollViewItems = new();

    private void RefreshRotationAndSizeDelta(int amount)
    {
        float zRotPerItem = 360f / amount;
        float toAddInOffset = ((float)amount - patternData.minSliderValue) / (patternData.maxSliderValue - patternData.minSliderValue) * 10f;
        for (int i = 0; i < patternScrollViewItems.Count; i++)
        {
            patternScrollViewItems[i].SetRotation(startOffset + toAddInOffset + (360 - i * zRotPerItem));
            if (patternData.stretchWidth)
            {
                patternScrollViewItems[i].RT.DOKill();
                patternScrollViewItems[i].RT.DOSizeDelta(new Vector2(zRotPerItem * patternData.patternSize[i % patternData.patternSize.Count].x, patternScrollViewItems[i].RT.sizeDelta.y), 1f).SetEase(Ease.OutExpo);
            }
        }
    }

    internal void InitializeItems()
    {
        for (int i = 0; i < 30; i++)
        {
            RectTransform rt = Instantiate(patternScrollViewItem, contentParent);
            PatternScrollViewItem patternScrollItem = rt.GetComponent<PatternScrollViewItem>();
            patternScrollViewItems.Add(patternScrollItem);
            patternScrollItem.patternImage = rt.GetChild(0).GetComponent<Image>();
            patternScrollItem.RT = rt;
            rt.gameObject.SetActive(false);
        }
    }

    internal void SetSprites(PatternData patternData)
    {

        ClearList();
        this.patternData = patternData;
        for (int i = 0; i < patternScrollViewItems.Count; i++)
        {
            patternScrollViewItems[i].patternImage.sprite = patternData.patternSprites[i % patternData.patternSprites.Count];
            patternScrollViewItems[i].RT.DOKill();
            if (patternData.stretchWidth)
            {
                patternScrollViewItems[i].patternImage.preserveAspect = false;
            }
            else
            {
                patternScrollViewItems[i].patternImage.preserveAspect = true;
                patternScrollViewItems[i].RT.sizeDelta = new Vector2(patternData.patternSize[i % patternData.patternSize.Count].x, patternData.patternSize[i % patternData.patternSize.Count].y);
            }
            RectTransform patternImageRt = patternScrollViewItems[i].RT.GetChild(0).GetComponent<RectTransform>();
            patternImageRt.offsetMin = new Vector2(patternImageRt.offsetMin.x, patternData.patternBottom[i % patternData.patternBottom.Count]);
        }

    }

    internal void SetAmount(int amount)
    {
        if (Mathf.Abs(amount - currentAmount) > 2)
        {
            while (amount % patternData.patternSprites.Count != 0)
            {
                amount--;
            }
        }

        if (currentAmount == amount || amount % patternData.patternSprites.Count != 0)
            return;

        SoundManager.Inst.LightVibrate();
        if (currentAmount < amount)
        {
            int loops = amount - currentAmount;
            float toAddInOffset = ((float)amount - patternData.minSliderValue) / (patternData.maxSliderValue - patternData.minSliderValue) * 10f;
            for (int i = 0; i < loops; i++)
            {
                patternScrollViewItems[i + currentAmount].RT.localEulerAngles = new Vector3(0, 0, startOffset + toAddInOffset);
                patternScrollViewItems[i + currentAmount].rotZ = startOffset + toAddInOffset;
                patternScrollViewItems[i + currentAmount].gameObject.SetActive(true);
                patternScrollViewItems[i + currentAmount].patternImage.DOKill();
                if (i == loops - 1)
                {
                    patternScrollViewItems[i + currentAmount].DoFade(1, 0.5f, Ease.InSine);
                }
                else
                {
                    patternScrollViewItems[i + currentAmount].DoFade(1, 0.2f, Ease.InSine);
                }
            }
            RefreshRotationAndSizeDelta(amount);
        }
        else
        {
            for (int i = currentAmount - 1; i >= amount; i--)
            {
                patternScrollViewItems[i].patternImage.DOKill();
                if (i == amount)
                {
                    patternScrollViewItems[i].DoFade(0, 0.5f, Ease.OutExpo);
                }
                else
                {
                    patternScrollViewItems[i].DoFade(0, 0.3f, Ease.OutExpo);
                }
            }
            RefreshRotationAndSizeDelta(amount);
        }
        currentAmount = amount;
    }

    internal void ClearList()
    {
        for (int i = currentAmount - 1; i >= 0; i--)
        {
            patternScrollViewItems[i].patternImage.DOKill();
            patternScrollViewItems[i].DoFade(0, 0, Ease.Linear);
        }
        currentAmount = 0;
    }

    internal void SetImageColor(Color color)
    {
        foreach (PatternScrollViewItem patternItem in patternScrollViewItems)
        {
            patternItem.patternImage.color = new(color.r, color.g, color.b, patternItem.patternImage.color.a);
        }
    }

    internal void SetOpacity(float opacity)
    {
        mainCg.alpha = opacity;
    }
}