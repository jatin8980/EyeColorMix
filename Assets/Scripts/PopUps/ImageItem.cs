using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageItem : UIBehaviour, IDynamicScrollViewItem
{
    [SerializeField] private GameObject adOb;
    [SerializeField] private Image imageIcon;
    private int imageIndex;

    public void onUpdateItem(int index)
    {
        if (ChooseImagePopUpController.Inst.selectedCategoryIndex > 6)
            return;

        imageIndex = ChooseImagePopUpController.Inst.orderToShowImage[ChooseImagePopUpController.Inst.selectedCategoryIndex][index];
        imageIcon.sprite = Resources.Load<Sprite>("Sprites/ColorPickImages/" + ChooseImagePopUpController.Inst.CategoryIndexToString(ChooseImagePopUpController.Inst.selectedCategoryIndex) + "/" + imageIndex);

        if (ChooseImagePopUpController.Inst.lockedByAdImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex)
            && !GeneralDataManager.UnlockedImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex))
        {
            adOb.SetActive(true);
        }
        else
        {
            adOb.SetActive(false);
        }
    }

    public void On_Image_Btn_Click()
    {
        if (ChooseImagePopUpController.Inst.lockedByAdImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex)
            && !GeneralDataManager.UnlockedImages[ChooseImagePopUpController.Inst.selectedCategoryIndex].Contains(imageIndex))
        {
            ChooseImagePopUpController.Inst.imageIndexToUnlock = imageIndex;
            if (GeneralDataManager.Inst.testMode)
                ChooseImagePopUpController.Inst.UnlockImageCallBack();
            else
                AdsManager.Inst.RequestAndLoadRewardedAd("UnlockImage");
        }
        else
        {
            ChooseImagePopUpController.Inst.OnImageItemClick(imageIcon.sprite);
        }
    }
}
