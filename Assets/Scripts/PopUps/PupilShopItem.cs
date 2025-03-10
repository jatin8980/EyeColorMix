using UnityEngine;
using UnityEngine.UI;

public class PupilShopItem : MonoBehaviour
{
    [SerializeField] private Image pupilIcon, bgImage;
    [SerializeField] private GameObject lockIcon, adOb;
    [SerializeField] private Text levelNumberText;
    internal int pupilIndex, index;

    private void Start()
    {
        if (transform.parent.GetSiblingIndex() > 0)
            ShopPopUpController.Inst.pupilItems.Add(this);
    }

    internal void SetIndex(int index)
    {
        this.index = index;
        pupilIndex = GeneralDataManager.Inst.orderToShowPupils[index];
        pupilIcon.sprite = GameManager.Inst.gamePlayUi.pupilIcons[pupilIndex];
    }

    internal void SetThis()
    {

        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPupil[index];
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
                if (ShopPopUpController.Inst.unlockedIndexes.Contains(pupilIndex))
                {
                    lockIcon.SetActive(false);
                    adOb.SetActive(false);
                    bgImage.color = Color.white;
                }
                else
                {
                    lockIcon.SetActive(true);
                    adOb.SetActive(true);
                    bgImage.color = new Color32(204, 204, 204, 255);
                }
            }
            else
            {
                lockIcon.SetActive(false);
                adOb.SetActive(false);
                bgImage.color = Color.white;
            }
            levelNumberText.transform.parent.gameObject.SetActive(false);
        }

    }

    public void On_Pupil_Btn_Click()
    {
        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPupil[index];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            GameManager.Inst.Show_Toast("Unlocks At Level " + levelNeededToUnlock);
            return;
        }

        if (levelNeededToUnlock == -1 && !ShopPopUpController.Inst.unlockedIndexes.Contains(pupilIndex))
        {
            GameManager.Inst.Show_Toast("Pupil is locked!");
            return;
        }

    }
}