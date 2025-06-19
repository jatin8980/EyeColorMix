using Mosframe;
using UnityEngine;
using UnityEngine.UI;

public class PenItem : MonoBehaviour
{
    [SerializeField] private Image penIcon, bgImage;
    [SerializeField] private GameObject selectedOb, lockOb;
    internal int index;

    private void Start()
    {
        if (transform.parent.GetSiblingIndex() > 0)
            ThemesPopUpController.Inst.penItems.Add(this);
    }

    internal void SetThis()
    {
        penIcon.sprite = Resources.Load<Sprite>("Sprites/Pens/PenThumbs/" + index);
        if (ThemesPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            selectedOb.SetActive(GeneralDataManager.SelectedPenIndex == index);
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

    public void On_Pen_Btn_Click()
    {
        if (GeneralDataManager.SelectedPenIndex == index)
            return;
        if (!ThemesPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.UnlockItemPopUp, false);
            ThemesPopUpController.Inst.selectedIndexForUnlock = index;
            FindAnyObjectByType<UnlockItemPopUp>().SetThis(penIcon.sprite, 0, GeneralDataManager.ShopPrices[ThemesPopUpController.Inst.selectedIndex],
                ThemesPopUpController.Inst.UnlockItemCallBack, ThemesPopUpController.Inst.IncreasePriceOfCurrentType, true);
            return;
        }

        int previousePenIndex = GeneralDataManager.SelectedPenIndex;
        GeneralDataManager.SelectedPenIndex = index;
        ThemesPopUpController.Inst.DeselectPen(previousePenIndex);
        SetThis();
        GameManager.Inst.gamePlayUi.pencilController.SetTheme(index);
    }
}