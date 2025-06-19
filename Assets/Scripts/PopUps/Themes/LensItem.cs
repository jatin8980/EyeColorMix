using UnityEngine;
using UnityEngine.UI;

public class LensItem : MonoBehaviour
{
    [SerializeField] private Image lensIcon, bgImage;
    [SerializeField] private GameObject selectedOb, lockOb;
    internal int index;

    private void Start()
    {
        if (transform.parent.GetSiblingIndex() > 0)
            ThemesPopUpController.Inst.lensItems.Add(this);
    }

    internal void SetThis()
    {
        lensIcon.sprite = Resources.Load<Sprite>("Sprites/Lens/Thumbs/" + index);
        if (ThemesPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            selectedOb.SetActive(GeneralDataManager.SelectedLensIndex == index);
            lockOb.SetActive(false);
            bgImage.color = Color.white;
        }
        else
        {
            bgImage.color = ThemesPopUpController.Inst.itemGreyColor;
            lockOb.SetActive(true);
            selectedOb.SetActive(false);
        }
    }

    public void On_Lens_Btn_Click()
    {
        if (GeneralDataManager.SelectedLensIndex == index)
            return;
        if (!ThemesPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.UnlockItemPopUp, false);
            ThemesPopUpController.Inst.selectedIndexForUnlock = index;
            FindAnyObjectByType<UnlockItemPopUp>().SetThis(lensIcon.sprite,0, GeneralDataManager.ShopPrices[ThemesPopUpController.Inst.selectedIndex],
                ThemesPopUpController.Inst.UnlockItemCallBack, ThemesPopUpController.Inst.IncreasePriceOfCurrentType);
            return;
        }

        int previouseIndex = GeneralDataManager.SelectedLensIndex;
        GeneralDataManager.SelectedLensIndex = index;
        ThemesPopUpController.Inst.DeselectLens(previouseIndex);
        SetThis();
        GameManager.Inst.gamePlayUi.lensController.SetLens(index);
    }
}
