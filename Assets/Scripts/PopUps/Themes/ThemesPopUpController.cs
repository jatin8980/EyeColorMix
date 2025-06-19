using Mosframe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemesPopUpController : MonoBehaviour
{
    [SerializeField] private RectTransform headerButtonsParentRT, penScrollViewRT, pupilScrollViewRT, patternScrollViewRT, lensesScrollViewRT, particleScrollViewRT;
    internal static ThemesPopUpController Inst;
    internal List<PenItem> penItems = new();
    internal List<LensItem> lensItems = new();
    internal List<PupilShopItem> pupilItems = new();
    internal List<PatternShopItem> patternItems = new();
    internal List<ClickParticleItem> clickParticleItems = new();
    internal List<int> unlockedIndexes = new();
    internal int spacing = 20, totalPenItems = 31, totalLensItems = 25, totalPupilItems = 46, totalPatternItems = 25, totalParticleItems = 10,
        bigItemsPerRow = 2, smallItemsPerRow, selectedIndex = 1, selectedIndexForUnlock;
    private Color32 greyBtnColor = new(238, 238, 238, 255), blueBtnColor = new(91, 168, 255, 255);
    internal Color32 itemGreyColor = new(219, 219, 219, 255);

    private void Awake()
    {
        Inst = this;
        unlockedIndexes = GeneralDataManager.UnlockedPenIndexes;
        smallItemsPerRow = Mathf.Max(Mathf.FloorToInt(penScrollViewRT.rect.width / 240), 3);
        pupilScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalPupilItems / (float)smallItemsPerRow);
        patternScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalPatternItems / (float)smallItemsPerRow);
        if (penScrollViewRT.rect.width > 1000)
        {
            bigItemsPerRow = 3;
            penScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalPenItems / 3f);
            lensesScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalLensItems / 3f);
            particleScrollViewRT.GetComponent<DynamicScrollView>().totalItemCount = Mathf.CeilToInt(totalParticleItems / 3f);
        }
        if (GameManager.activeScreen == GameManager.Screens.GamePlay)
        {
            headerButtonsParentRT.gameObject.SetActive(false);
            penScrollViewRT.offsetMax = new Vector2(penScrollViewRT.offsetMax.x, -310);
        }
        RefreshForAd();
    }

    private void Start()
    {
        if (selectedIndex == 1)
            On_Header_Btn_Click(0);
    }

    private void ScrollToSelectedItem(int waitForEndFrameCount, int selectedItemIndex, RectTransform contentRT)
    {
        GeneralDataManager.Inst.No_Click_Panel_On_Off(true);
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
        else if (selectedIndex == 3)
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
        else
        {
            GeneralDataManager.UnlockedClickParticleIndexes = unlockedIndexes;
            foreach (ClickParticleItem clickParticleItem in clickParticleItems)
            {
                if (clickParticleItem.index == selectedIndexForUnlock)
                {
                    clickParticleItem.GetComponent<Animation>().Play();
                    clickParticleItem.Invoke(nameof(clickParticleItem.On_Particle_Btn_Click), 1f);
                    break;
                }
            }
        }
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

    internal void DeselectParticle(int index)
    {
        foreach (ClickParticleItem clickParticleItem in clickParticleItems)
        {
            if (clickParticleItem.index == index)
            {
                clickParticleItem.SetThis();
                break;
            }
        }
    }

    internal void UnlockItemCallBack()
    {
        if (selectedIndex == 0)
        {
            ScrollToSelectedItem(0, selectedIndexForUnlock, penScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>());
        }
        else if (selectedIndex == 1)
        {
            ScrollToSelectedItem(0, selectedIndexForUnlock, lensesScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>());
        }
        else if (selectedIndex == 2)
        {
            ScrollToSelectedItem(0, GeneralDataManager.Inst.orderToShowPupils.IndexOf(selectedIndexForUnlock), pupilScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>());
        }
        else if (selectedIndex == 3)
        {
            ScrollToSelectedItem(0, GeneralDataManager.Inst.orderToShowPattern.IndexOf(selectedIndexForUnlock), patternScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>());
        }
        else
        {
            ScrollToSelectedItem(0, selectedIndexForUnlock, particleScrollViewRT.GetChild(0).GetChild(0).GetComponent<RectTransform>());
        }
    }

    internal void RefreshForAd()
    {
        if (AdsManager.Inst.isBannerLoaded)
        {
            penScrollViewRT.offsetMin = lensesScrollViewRT.offsetMin = pupilScrollViewRT.offsetMin = patternScrollViewRT.offsetMin = particleScrollViewRT.offsetMin = new(penScrollViewRT.offsetMin.x, 170 + GameManager.Inst.bannerHeight);
        }
        else
        {
            penScrollViewRT.offsetMin = lensesScrollViewRT.offsetMin = pupilScrollViewRT.offsetMin = patternScrollViewRT.offsetMin = particleScrollViewRT.offsetMin = new(penScrollViewRT.offsetMin.x, 170);
        }
    }

    internal void IncreasePriceOfCurrentType()
    {
        List<int> shopPrices = GeneralDataManager.ShopPrices;
        shopPrices[selectedIndex] += 100;
        GeneralDataManager.ShopPrices = shopPrices;
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
            case 4:
                particleScrollViewRT.gameObject.SetActive(false);
                break;
        }

        selectedIndex = index;

        switch (selectedIndex)
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
                unlockedIndexes = GeneralDataManager.UnlockedPupils;
                pupilScrollViewRT.gameObject.SetActive(true);
                break;
            case 3:
                unlockedIndexes = GeneralDataManager.UnlockedPatterns;
                patternScrollViewRT.gameObject.SetActive(true);
                break;
            case 4:
                unlockedIndexes = GeneralDataManager.UnlockedClickParticleIndexes;
                particleScrollViewRT.gameObject.SetActive(true);
                break;
        }
    }

    public void On_Back_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }
}