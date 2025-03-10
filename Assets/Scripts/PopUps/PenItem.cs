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
            ShopPopUpController.Inst.penItems.Add(this);
    }

    internal void SetThis()
    {
        penIcon.sprite = Resources.Load<Sprite>("Sprites/Pens/PenThumbs/" + index);
        if (ShopPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            selectedOb.SetActive(GeneralDataManager.SelectedPenIndex == index);
            lockOb.SetActive(false);
            bgImage.color = Color.white;
        }
        else
        {
            bgImage.color = new Color32(204, 204, 204, 255);
            lockOb.SetActive(true);
            selectedOb.SetActive(false);
        }
    }

    public void On_Pen_Btn_Click()
    {
        if (GeneralDataManager.SelectedPenIndex == index)
            return;
        if (!ShopPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            GameManager.Inst.Show_Toast("Pen is locked!");
            return;
        }

        int previousePenIndex = GeneralDataManager.SelectedPenIndex;
        GeneralDataManager.SelectedPenIndex = index;
        ShopPopUpController.Inst.DeselectPen(previousePenIndex);
        SetThis();
        GameManager.Inst.gamePlayUi.pencilController.SetTheme(index);
    }
}