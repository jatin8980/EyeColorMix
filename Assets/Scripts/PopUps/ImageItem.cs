using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageItem : UIBehaviour, IDynamicScrollViewItem
{
    [SerializeField] private GameObject lockOb;
    [SerializeField] private Image imageIcon;
    private int imageIndex;

    public void onUpdateItem(int index)
    {
        if (ChooseImagePopUpController.Inst.selectedCategoryIndex > 6)
            return;

        imageIndex = ChooseImagePopUpController.Inst.orderToShowImage[ChooseImagePopUpController.Inst.selectedCategoryIndex][index];
        imageIcon.sprite = Resources.Load<Sprite>("Sprites/ColorPickImages/" + ChooseImagePopUpController.Inst.CategoryIndexToString(ChooseImagePopUpController.Inst.selectedCategoryIndex) + "/" + imageIndex);

        if (ChooseImagePopUpController.Inst.lockedByAdImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex)
            && !ChooseImagePopUpController.Inst.unlockedImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex))
        {
            lockOb.SetActive(true);
        }
        else
        {
            lockOb.SetActive(false);
        }
    }

    public void On_Image_Btn_Click()
    {
        if (ChooseImagePopUpController.Inst.lockedByAdImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex)
            && !ChooseImagePopUpController.Inst.unlockedImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex))
        {
            ChooseImagePopUpController.Inst.imageIndexToUnlock = imageIndex;
            if (GeneralDataManager.Inst.testMode)
                ChooseImagePopUpController.Inst.UnlockImage();
            else
            {
                GameManager.Inst.Show_Popup(GameManager.Popups.UnlockItemPopUp, false);
                FindAnyObjectByType<UnlockItemPopUp>().SetThis(imageIcon.sprite, 0, 150, new(() =>
                {
                    GameManager.Inst.Show_Toast("Image unlocked!");
                    ChooseImagePopUpController.Inst.UnlockImage();
                }));
            }
        }
        else
        {
            ChooseImagePopUpController.Inst.OnImageItemClick(imageIcon.sprite);
        }
    }
}
