using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectItem : UIBehaviour, IDynamicScrollViewItem
{
    [SerializeField] private Image effectIcon;
    [SerializeField] private GameObject selectedOb, adOb;
    [SerializeField] private Text levelNumberText;
    internal int effectIndex;
    private int index;

    protected override void Start()
    {
        if (transform.GetSiblingIndex() > 0)
        {
            EffectsController.Inst.effectItems.Add(this);
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                EffectsController.Inst.OnEnable();
        }
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
        if (index == 0)
        {
            effectIndex = -1;
            effectIcon.sprite = Resources.Load<Sprite>("Sprites/none");
            effectIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
        }
        else
        {
            effectIndex = GeneralDataManager.Inst.orderToShowEffects[index - 1];
            effectIcon.sprite = EffectsController.Inst.effectIcons[effectIndex];
            effectIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
        }
        SetThis();
    }

    internal void SetThis()
    {
        if (index == 0)
        {
            selectedOb.SetActive(false);
            adOb.SetActive(false);
            levelNumberText.transform.parent.gameObject.SetActive(false);
            return;
        }
        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockEffect[index - 1];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            adOb.SetActive(false);
            selectedOb.SetActive(false);
            levelNumberText.text = "Level " + levelNeededToUnlock;
            levelNumberText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            selectedOb.SetActive(effectIndex == EffectsController.Inst.selectedEffectIndex);
            adOb.SetActive(false);
            levelNumberText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void On_Effect_Btn_Click()
    {
        if (index == 0)
        {
            int previousSelected = EffectsController.Inst.selectedEffectIndex;
            if (previousSelected != -1)
            {
                EffectsController.Inst.selectedEffectIndex = -1;
                EffectsController.Inst.RefreshAtIndex(previousSelected);
                EffectsController.Inst.ApplyEffect(-1);
            }
            return;
        }

        if (EffectsController.Inst.selectedEffectIndex == effectIndex)
            return;

        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockEffect[index - 1];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            GameManager.Inst.Show_Toast("Unlocks At Level " + levelNeededToUnlock);
            return;
        }

        int previousSelectedEffect = EffectsController.Inst.selectedEffectIndex;
        EffectsController.Inst.selectedEffectIndex = effectIndex;
        if (previousSelectedEffect != -1)
            EffectsController.Inst.RefreshAtIndex(previousSelectedEffect);
        SetThis();
        EffectsController.Inst.ApplyEffect(effectIndex);

        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.TutorialStep++;// to 12
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
        }
    }
}
