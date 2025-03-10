using DG.Tweening;
using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopUpController : MonoBehaviour
{
    [SerializeField] private RectTransform headerButtonsParentRT, penScrollViewRT, pupilScrollViewRT, patternScrollViewRT, lensesScrollViewRT, bottomButtonsRT;
    [SerializeField] private Text priceText;
    internal static ShopPopUpController Inst;
    internal List<PenItem> penItems = new();
    internal List<LensItem> lensItems = new();
    internal List<PupilShopItem> pupilItems = new();
    internal List<PatternShopItem> patternItems = new();
    internal List<int> unlockedIndexes = new();
    internal int spacing = 20, totalPenItems = 31, totalLensItems = 25, totalPupilItems = 46, totalPatternItems = 25,
        bigItemsPerRow = 2, smallItemsPerRow, selectedIndex = 1, selectedIndexForUnlock;
    private Color32 greyBtnColor = new(238, 238, 238, 255), blueBtnColor = new(91, 168, 255, 255);

    private void Awake()
    {
        Inst = this;
        unlockedIndexes = GeneralDataManager.UnlockedPenIndexes;
        smallItemsPerRow = Math.Max(Mathf.FloorToInt(penScrollViewRT.rect.width / 240), 3);
        pupilScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalPupilItems / (float)smallItemsPerRow);
        patternScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalPatternItems / (float)smallItemsPerRow);
        if (penScrollViewRT.rect.width > 1000)
        {
            bigItemsPerRow = 3;
            penScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalPenItems / 3f);
            lensesScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalLensItems / 3f);
        }
        if (GameManager.activeScreen == GameManager.Screens.GamePlay)
        {
            headerButtonsParentRT.GetChild(1).gameObject.SetActive(false);
            headerButtonsParentRT.GetChild(2).gameObject.SetActive(false);
            headerButtonsParentRT.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            GameManager.Inst.SetActiveFillCoin(true);
        }
        RefreshForNativeAd();
    }

    private void Start()
    {
        On_Header_Btn_Click(0);
        GameManager.Inst.SetCoinParentAbovePopUp(true);
    }

    private void OnDestroy()
    {
        GameManager.Inst.SetCoinParentAbovePopUp(false);
        if (GameManager.activeScreen == GameManager.Screens.Home)
            GameManager.Inst.SetActiveFillCoin(false);
    }

    private List<int> GetLockedItemsListOfCurrentScrollView()
    {
        List<int> lockedIndexes = new();
        if (selectedIndex == 0)
        {
            for (int i = 0; i < totalPenItems; i++)
            {
                if (!unlockedIndexes.Contains(i))
                    lockedIndexes.Add(i);
            }
        }
        else if (selectedIndex == 1)
        {
            for (int i = 0; i < totalLensItems; i++)
            {
                if (!unlockedIndexes.Contains(i))
                    lockedIndexes.Add(i);
            }
        }
        else if (selectedIndex == 2)
        {
            for (int i = 0; i < totalPupilItems; i++)
            {
                if (!unlockedIndexes.Contains(i) && GeneralDataManager.Inst.levelNeededToUnlockPupil[i] == -1)
                    lockedIndexes.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < totalPatternItems; i++)
            {
                if (!unlockedIndexes.Contains(i) && GeneralDataManager.Inst.levelNeededToUnlockPattern[i] == -1)
                    lockedIndexes.Add(i);
            }
        }
        return lockedIndexes;
    }

    private IEnumerator ScrollToSelectedItem(int waitForEndFrameCount, int selectedItemIndex, RectTransform contentRT)
    {
        GeneralDataManager.Inst.No_Click_Panel_On_Off(true);
        for (int i = 0; i < waitForEndFrameCount; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        int viewPortHeight = (int)contentRT.parent.GetComponent<RectTransform>().rect.height;
        int row;
        if (selectedIndex == 0 || selectedIndex == 1)
            row = Mathf.FloorToInt(selectedItemIndex / (float)bigItemsPerRow);
        else
            row = Mathf.FloorToInt(selectedItemIndex / (float)smallItemsPerRow);

        float target = 0;
        if (contentRT.rect.height > viewPortHeight)
            target = Mathf.Min(contentRT.rect.height - viewPortHeight, row * contentRT.GetChild(0).GetComponent<RectTransform>().rect.height);
        contentRT.GetComponentInParent<ScrollRect>().velocity = Vector2.zero;
        contentRT.DOAnchorPosY(target, 1800f).SetSpeedBased(true).OnComplete(() =>
        {
            GeneralDataManager.Inst.Invoke(nameof(GeneralDataManager.Inst.No_Click_Panel_Off), 1f);// wait for anim to complete
            unlockedIndexes.Add(selectedIndexForUnlock);
            if (selectedIndex == 0)
            {
                GeneralDataManager.UnlockedPenIndexes = unlockedIndexes;
                foreach (PenItem penItem in penItems)
                {
                    if (penItem.index == selectedIndexForUnlock)
                    {
                        penItem.GetComponent<Animation>().Play();
                        penItem.Invoke(nameof(penItem.On_Pen_Btn_Click), 1f);
                        break;
                    }
                }
            }
            else if (selectedIndex == 1)
            {
                GeneralDataManager.UnlockedLensIndexes = unlockedIndexes;
                foreach (LensItem lensItem in lensItems)
                {
                    if (lensItem.index == selectedIndexForUnlock)
                    {
                        lensItem.GetComponent<Animation>().Play();
                        lensItem.Invoke(nameof(lensItem.On_Lens_Btn_Click), 1f);
                        break;
                    }
                }
            }
            else if (selectedIndex == 2)
            {
                GeneralDataManager.UnlockedPupils = unlockedIndexes;
                foreach (PupilShopItem pupilShopItem in pupilItems)
                {
                    if (pupilShopItem.pupilIndex == selectedIndexForUnlock)
                    {
                        pupilShopItem.GetComponent<Animation>().Play();
                        pupilShopItem.Invoke(nameof(pupilShopItem.SetThis), 1);
                        break;
                    }
                }
            }
            else
            {
                GeneralDataManager.UnlockedPatterns = unlockedIndexes;
                foreach (PatternShopItem patternShopItem in patternItems)
                {
                    if (patternShopItem.patternIndex == selectedIndexForUnlock)
                    {
                        patternShopItem.GetComponent<Animation>().Play();
                        patternShopItem.Invoke(nameof(patternShopItem.SetThis), 1);
                        break;
                    }
                }
            }
        });
    }

    private void UpdatePriceText()
    {
        priceText.text = GeneralDataManager.ShopPrices[selectedIndex].ToString();
    }

    internal void DeselectPen(int index)
    {
        foreach (PenItem penItem in penItems)
        {
            if (penItem.index == index)
            {
                penItem.SetThis();
                break;
            }
        }
    }

    internal void DeselectLens(int index)
    {
        foreach (LensItem lensItem in lensItems)
        {
            if (lensItem.index == index)
            {
                lensItem.SetThis();
                break;
            }
        }
    }

    internal void UnlockRandomCallBack()
    {
        if (selectedIndex == 0)
        {
            StartCoroutine(ScrollToSelectedItem(0, selectedIndexForUnlock, penScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>()));
        }
        else if (selectedIndex == 1)
        {
            StartCoroutine(ScrollToSelectedItem(0, selectedIndexForUnlock, lensesScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>()));
        }
        else if (selectedIndex == 2)
        {
            StartCoroutine(ScrollToSelectedItem(0, GeneralDataManager.Inst.orderToShowPupils.IndexOf(selectedIndexForUnlock), pupilScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>()));
        }
        else
        {
            StartCoroutine(ScrollToSelectedItem(0, GeneralDataManager.Inst.orderToShowPattern.IndexOf(selectedIndexForUnlock), patternScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>()));
        }
    }

    internal void RefreshForNativeAd()
    {
        if (AdsManager.Inst.isNativeAdLoaded)
        {
            penScrollViewRT.offsetMin = lensesScrollViewRT.offsetMin = pupilScrollViewRT.offsetMin = patternScrollViewRT.offsetMin = new(penScrollViewRT.offsetMin.x, 525);
            bottomButtonsRT.anchoredPosition = new Vector2(0, 368f);
        }
        else
        {
            penScrollViewRT.offsetMin = lensesScrollViewRT.offsetMin = pupilScrollViewRT.offsetMin = patternScrollViewRT.offsetMin = new(penScrollViewRT.offsetMin.x, 355);
            bottomButtonsRT.anchoredPosition = new Vector2(0, 198f);
        }
    }

    public void On_Watch_Btn_Click()
    {
        List<int> lockedIndexes = GetLockedItemsListOfCurrentScrollView();
        if (lockedIndexes.Count == 0)
        {
            if (selectedIndex == 0)
                GameManager.Inst.Show_Toast("All pens unlocked!");
            else if (selectedIndex == 1)
                GameManager.Inst.Show_Toast("All cases unlocked!");
            else if (selectedIndex == 2)
                GameManager.Inst.Show_Toast("All pupils unlocked!");
            else
                GameManager.Inst.Show_Toast("All patterns unlocked!");
            return;
        }
        selectedIndexForUnlock = lockedIndexes[UnityEngine.Random.Range(0, lockedIndexes.Count)];
        AdsManager.Inst.RequestAndLoadRewardedAd("UnlockShopItem");
    }

    public void On_Coins_Btn_Click()
    {
        List<int> lockedIndexes = GetLockedItemsListOfCurrentScrollView();
        if (lockedIndexes.Count == 0)
        {
            if (selectedIndex == 0)
                GameManager.Inst.Show_Toast("All pens unlocked!");
            else if (selectedIndex == 1)
                GameManager.Inst.Show_Toast("All cases unlocked!");
            else if (selectedIndex == 2)
                GameManager.Inst.Show_Toast("All pupils unlocked!");
            else
                GameManager.Inst.Show_Toast("All patterns unlocked!");
            return;
        }
        List<int> shopPrices = GeneralDataManager.ShopPrices;
        if (GeneralDataManager.Coins < shopPrices[selectedIndex])
        {
            GameManager.Inst.Show_Toast("Not enough coins!");
            return;
        }

        GeneralDataManager.Coins -= shopPrices[selectedIndex];
        GameManager.Inst.SetCoinsText(GeneralDataManager.Coins.ToString());
        shopPrices[selectedIndex] += 100;
        GeneralDataManager.ShopPrices = shopPrices;
        UpdatePriceText();

        selectedIndexForUnlock = lockedIndexes[UnityEngine.Random.Range(0, lockedIndexes.Count)];
        UnlockRandomCallBack();
    }

    public void On_Header_Btn_Click(int index)
    {
        if (selectedIndex != -1)
        {
            headerButtonsParentRT.GetChild(selectedIndex).GetComponent<Image>().color = greyBtnColor;
            headerButtonsParentRT.GetChild(selectedIndex).GetChild(0).gameObject.SetActive(false);
            headerButtonsParentRT.GetChild(selectedIndex).GetChild(1).gameObject.SetActive(true);
        }
        headerButtonsParentRT.GetChild(index).GetComponent<Image>().color = blueBtnColor;
        headerButtonsParentRT.GetChild(index).GetChild(0).gameObject.SetActive(true);
        headerButtonsParentRT.GetChild(index).GetChild(1).gameObject.SetActive(false);

        switch (selectedIndex)
        {
            case 0:
                penScrollViewRT.gameObject.SetActive(false);
                break;
            case 1:
                lensesScrollViewRT.gameObject.SetActive(false);
                break;
            case 2:
                pupilScrollViewRT.gameObject.SetActive(false);
                break;
            case 3:
                patternScrollViewRT.gameObject.SetActive(false);
                break;
        }

        selectedIndex = index;

        switch (index)
        {
            case 0:
                unlockedIndexes = GeneralDataManager.UnlockedPenIndexes;
                penScrollViewRT.gameObject.SetActive(true);
                break;
            case 1:
                unlockedIndexes = GeneralDataManager.UnlockedLensIndexes;
                lensesScrollViewRT.gameObject.SetActive(true);
                break;
            case 2:
                pupilScrollViewRT.gameObject.SetActive(true);
                unlockedIndexes = GeneralDataManager.UnlockedPupils;
                break;
            case 3:
                unlockedIndexes = GeneralDataManager.UnlockedPatterns;
                patternScrollViewRT.gameObject.SetActive(true);
                break;
        }
        UpdatePriceText();
    }

    public void On_Back_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }
}