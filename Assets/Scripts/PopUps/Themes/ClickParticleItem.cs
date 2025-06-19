using UnityEngine;
using UnityEngine.UI;

public class ClickParticleItem : MonoBehaviour
{
    [SerializeField] private Image particleIcon, bgImage;
    [SerializeField] private GameObject selectedOb, lockOb;
    internal int index;

    private void Start()
    {
        if (transform.parent.GetSiblingIndex() > 0)
            ThemesPopUpController.Inst.clickParticleItems.Add(this);
    }

    internal void SetThis()
    {
        RectTransform iconRT = particleIcon.GetComponent<RectTransform>();
        if (index == -1)
        {
            particleIcon.sprite = Resources.Load<Sprite>("Sprites/none");
            iconRT.offsetMax = new Vector2(-60, -60);
            iconRT.offsetMin = new Vector2(60, 60);
        }
        else
        {
            iconRT.offsetMax = new Vector2(-20, -20);
            iconRT.offsetMin = new Vector2(20, 20);
            particleIcon.sprite = Resources.Load<Sprite>("Sprites/ClickParticleThumbs/" + index);
        }

        if (ThemesPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            selectedOb.SetActive(GeneralDataManager.SelectedClickParticleIndex == index);
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

    public void On_Particle_Btn_Click()
    {
        if (GeneralDataManager.SelectedClickParticleIndex == index)
            return;
        if (!ThemesPopUpController.Inst.unlockedIndexes.Contains(index))
        {
            GameManager.Inst.Show_Popup(GameManager.Popups.UnlockItemPopUp, false);
            ThemesPopUpController.Inst.selectedIndexForUnlock = index;
            FindAnyObjectByType<UnlockItemPopUp>().SetThis(particleIcon.sprite,20, -1, ThemesPopUpController.Inst.UnlockItemCallBack);
            return;
        }

        int previousParticleIndex = GeneralDataManager.SelectedClickParticleIndex;
        GeneralDataManager.SelectedClickParticleIndex = index;
        ThemesPopUpController.Inst.DeselectParticle(previousParticleIndex);
        SetThis();
        GameManager.Inst.SetSelectedClickParticle();
    }
}
