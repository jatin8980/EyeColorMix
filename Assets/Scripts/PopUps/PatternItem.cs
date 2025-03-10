using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PatternItem : UIBehaviour, IDynamicScrollViewItem
{
    [SerializeField] private Image patternIcon;
    [SerializeField] private GameObject selectedOb, adOb;
    [SerializeField] private Text levelNumberText;
    internal int patternIndex;
    private int index;

    protected override void Start()
    {
        if (transform.GetSiblingIndex() > 0)
        {
            PatternController.Inst.patternItems.Add(this);
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                PatternController.Inst.OnEnable();
        }
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
        if (index == 0)
        {
            patternIndex = -1;
            patternIcon.sprite = Resources.Load<Sprite>("Sprites/none");
            patternIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
        }
        else
        {
            patternIndex = GeneralDataManager.Inst.orderToShowPattern[index - 1];
            patternIcon.sprite = Resources.Load<Sprite>("Sprites/PatternThumbs/" + patternIndex);
            patternIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);
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
        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPattern[index - 1];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            adOb.SetActive(false);
            selectedOb.SetActive(false);
            levelNumberText.text = "Level " + levelNeededToUnlock;
            levelNumberText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            if (levelNeededToUnlock == -1)
            {
                if (PatternController.Inst.unlockedPatterns.Contains(patternIndex))
                {
                    selectedOb.SetActive(patternIndex == PatternController.Inst.selectedPatternIndex);
                    adOb.SetActive(false);
                }
                else
                {
                    selectedOb.SetActive(false);
                    adOb.SetActive(true);
                }
            }
            else
            {
                selectedOb.SetActive(patternIndex == PatternController.Inst.selectedPatternIndex);
                adOb.SetActive(false);
            }
            levelNumberText.transform.parent.gameObject.SetActive(false);
        }

    }

    public void On_Pattern_Btn_Click()
    {
        if (index == 0)
        {
            int previousPattern = PatternController.Inst.selectedPatternIndex;
            if (previousPattern != -1)
            {
                PatternController.Inst.selectedPatternIndex = -1;
                PatternController.Inst.RefreshAtIndex(previousPattern);
                PatternController.Inst.ApplyPattern(-1);
            }
            return;
        }

        if (PatternController.Inst.selectedPatternIndex == patternIndex)
            return;

        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPattern[index - 1];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            GameManager.Inst.Show_Toast("Unlocks At Level " + levelNeededToUnlock);
            return;
        }

        if (levelNeededToUnlock == -1 && !PatternController.Inst.unlockedPatterns.Contains(patternIndex))
        {
            PatternController.Inst.selectedPatternIndexForUnlock = patternIndex;
            if (GeneralDataManager.Inst.testMode)
            {
                PatternController.Inst.UnlockPatternCallBack();
            }
            else
            {
                AdsManager.Inst.RequestAndLoadRewardedAd("UnlockPattern");
            }
            return;
        }

        if (UserTutorialController.Inst != null)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.IsPatternTutorialShowed = true;
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
        }

        int previousPatternIndex = PatternController.Inst.selectedPatternIndex;
        PatternController.Inst.selectedPatternIndex = patternIndex;
        if (previousPatternIndex != -1)
            PatternController.Inst.RefreshAtIndex(previousPatternIndex);
        SetThis();
        PatternController.Inst.ApplyPattern(patternIndex);


    }


}
