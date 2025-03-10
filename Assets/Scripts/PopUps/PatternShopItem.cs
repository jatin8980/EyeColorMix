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
            ShopPopUpController.Inst.patternItems.Add(this);
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
            bgImage.color = new Color32(204, 204, 204, 255);
            levelNumberText.text = "Level " + levelNeededToUnlock;
            levelNumberText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            if (levelNeededToUnlock == -1)
            {
                if (ShopPopUpController.Inst.unlockedIndexes.Contains(patternIndex))
                {
                    adOb.SetActive(false);
                    lockIcon.SetActive(false);
                    bgImage.color = Color.white;
                }
                else
                {
                    adOb.SetActive(true);
                    lockIcon.SetActive(true);
                    bgImage.color = new Color32(204, 204, 204, 255);
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

        if (levelNeededToUnlock == -1 && !ShopPopUpController.Inst.unlockedIndexes.Contains(patternIndex))
        {
            GameManager.Inst.Show_Toast("Pattern is locked!");
            return;
        }
    }
}