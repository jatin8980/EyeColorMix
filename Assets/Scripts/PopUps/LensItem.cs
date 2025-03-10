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
            ShopPopUpController.Inst.lensItems.Add(this);
    }

    internal void SetThis()
    {
        lensIcon.sprite = Resources.Load<Sprite>("Sprites/Lens/Thumbs/" + index);
        if (ShopPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            selectedOb.SetActive(GeneralDataManager.SelectedLensIndex == index);
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

    public void On_Lens_Btn_Click()
    {
        if (GeneralDataManager.SelectedLensIndex == index)
            return;
        if (!ShopPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            GameManager.Inst.Show_Toast("Case is locked!");
            return;
        }

        int previouseIndex = GeneralDataManager.SelectedLensIndex;
        GeneralDataManager.SelectedLensIndex = index;
        ShopPopUpController.Inst.DeselectLens(previouseIndex);
        SetThis();
        GameManager.Inst.gamePlayUi.lensController.SetLens(index);
    }
}
