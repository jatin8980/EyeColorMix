using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightItem : UIBehaviour, IDynamicScrollViewItem
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject selectedOb, adOb;
    [SerializeField] private Text levelNumberText;
    internal int highlightIndex;
    private int index;

    protected override void Start()
    {
        if (transform.GetSiblingIndex() > 0)
        {
            HighlightController.Inst.highlightItems.Add(this);
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                HighlightController.Inst.OnEnable();
        }
    }

    public void onUpdateItem(int index)
    {
        this.index = index;

        this.index = index;
        if (index == 0)
        {
            highlightIndex = -1;
            icon.sprite = Resources.Load<Sprite>("Sprites/none");
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
            icon.transform.parent.GetComponent<Image>().enabled = false;
        }
        else
        {
            highlightIndex = GeneralDataManager.Inst.orderToShowHighlights[index - 1];
            icon.sprite = GameManager.Inst.gamePlayUi.highlightIcons[highlightIndex];
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
            icon.transform.parent.GetComponent<Image>().enabled = true;
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
        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockHighlight[index - 1];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            adOb.SetActive(false);
            selectedOb.SetActive(false);
            levelNumberText.text = "Level " + levelNeededToUnlock;
            levelNumberText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            selectedOb.SetActive(highlightIndex == HighlightController.Inst.selectedHighlightIndex);
            adOb.SetActive(false);
            levelNumberText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void On_HighlightBtn_Click()
    {
        if (index == 0)
        {
            int previousHighlight = HighlightController.Inst.selectedHighlightIndex;
            if (previousHighlight != -1)
            {
                HighlightController.Inst.selectedHighlightIndex = -1;
                HighlightController.Inst.RefreshAtIndex(previousHighlight);
                GameManager.Inst.gamePlayUi.RefreshHighlight(-1);
            }
            return;
        }

        if (HighlightController.Inst.selectedHighlightIndex == highlightIndex)
            return;

        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockHighlight[index - 1];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            GameManager.Inst.Show_Toast("Unlocks At Level " + levelNeededToUnlock);
            return;
        }

        int previousHighlightIndex = HighlightController.Inst.selectedHighlightIndex;
        HighlightController.Inst.selectedHighlightIndex = highlightIndex;
        if (previousHighlightIndex != -1)
            HighlightController.Inst.RefreshAtIndex(previousHighlightIndex);
        SetThis();
        GameManager.Inst.gamePlayUi.RefreshHighlight(highlightIndex);

        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.TutorialStep++;// to 13
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
        }
    }
}