using UnityEngine;
using UnityEngine.UI;

public class PatternShopItem : MonoBehaviour
{
    [SerializeField] private Image patternIcon, bgImage;
    [SerializeField] private GameObject adOb, lockIcon;
    [SerializeField] private Text levelNumberText;
    internal int patternIndex, index;

    private void Start()
    {
        if (transform.parent.GetSiblingIndex() > 0)
            ThemesPopUpController.Inst.patternItems.Add(this);
    }

    internal void SetIndex(int index)
    {
        this.index = index;
        patternIndex = GeneralDataManager.Inst.orderToShowPattern[index];
        patternIcon.sprite = Resources.Load<Sprite>("Sprites/PatternThumbs/" + patternIndex);
    }

    internal void SetThis()
    {

        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPattern[index];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            lockIcon.SetActive(false);
            adOb.SetActive(false);
            bgImage.color = ThemesPopUpController.Inst.itemGreyColor;
            levelNumberText.text = "Level " + levelNeededToUnlock;
            levelNumberText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            if (levelNeededToUnlock == -1)
            {
                if (ThemesPopUpController.Inst.unlockedIndexes.Contains(patternIndex))
                {
                    adOb.SetActive(false);
                    lockIcon.SetActive(false);
                    bgImage.color = Color.white;
                }
                else
                {
                    adOb.SetActive(true);
                    lockIcon.SetActive(true);
                    bgImage.color = ThemesPopUpController.Inst.itemGreyColor;
                }
            }
            else
            {
                adOb.SetActive(false);
                lockIcon.SetActive(false);
                bgImage.color = Color.white;
            }
            levelNumberText.transform.parent.gameObject.SetActive(false);
        }

    }

    public void On_Pattern_Btn_Click()
    {
        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPattern[index];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            GameManager.Inst.Show_Toast("Unlocks At Level " + levelNeededToUnlock);
            return;
        }

        if (levelNeededToUnlock == -1 && !ThemesPopUpController.Inst.unlockedIndexes.Contains(patternIndex))
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.UnlockItemPopUp, false);
            ThemesPopUpController.Inst.selectedIndexForUnlock = index;
            FindAnyObjectByType<UnlockItemPopUp>().SetThis(patternIcon.sprite,20, GeneralDataManager.ShopPrices[ThemesPopUpController.Inst.selectedIndex],
                ThemesPopUpController.Inst.UnlockItemCallBack, ThemesPopUpController.Inst.IncreasePriceOfCurrentType);
            return;
        }
    }
}