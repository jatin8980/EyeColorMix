using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PupilItem : UIBehaviour, IDynamicScrollViewItem
{
    [SerializeField] private Image pupilIcon;
    [SerializeField] private GameObject selectedOb, adOb;
    [SerializeField] private Text levelNumberText;
    internal int pupilIndex, index;

    protected override void Start()
    {
        if (transform.GetSiblingIndex() > 0)
        {
            PupilController.Inst.pupilItems.Add(this);
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                PupilController.Inst.OnEnable();
        }
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
        pupilIndex = GeneralDataManager.Inst.orderToShowPupils[index];
        pupilIcon.sprite = GameManager.Inst.gamePlayUi.pupilIcons[pupilIndex];
        SetThis();
    }

    internal void SetThis()
    {
        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPupil[index];
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
                if (PupilController.Inst.unlockedPupils.Contains(pupilIndex))
                {
                    selectedOb.SetActive(pupilIndex == PupilController.Inst.selectedPupilIndex);
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
                selectedOb.SetActive(pupilIndex == PupilController.Inst.selectedPupilIndex);
                adOb.SetActive(false);
            }
            levelNumberText.transform.parent.gameObject.SetActive(false);
        }

    }

    public void On_Pupil_Btn_Click()
    {
        if (PupilController.Inst.selectedPupilIndex == pupilIndex)
            return;

        int levelNeededToUnlock = GeneralDataManager.Inst.levelNeededToUnlockPupil[index];
        if (GeneralDataManager.Level < levelNeededToUnlock)
        {
            GameManager.Inst.Show_Toast("Unlocks At Level " + levelNeededToUnlock);
            return;
        }

        if (levelNeededToUnlock == -1 && !PupilController.Inst.unlockedPupils.Contains(pupilIndex))
        {
            PupilController.Inst.selectedPupilIndexForUnlock = pupilIndex;
            if (GeneralDataManager.Inst.testMode)
            {
                PupilController.Inst.UnlockPupilCallBack();
            }
            else
            {
                AdsManager.Inst.RequestAndLoadRewardedAd("UnlockPupil");
            }
            return;
        }

        int previousSelectedPupil = PupilController.Inst.selectedPupilIndex;
        PupilController.Inst.selectedPupilIndex = pupilIndex;
        PupilController.Inst.RefreshAtIndex(previousSelectedPupil);
        SetThis();

        GameManager.Inst.gamePlayUi.RefreshPupil(pupilIndex);
        if (UserTutorialController.Inst != null && GeneralDataManager.TutorialStep == 7)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.TutorialStep++;//to 8
            GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
            GameManager.Inst.gamePlayUi.NextBtnActiveSelf(true);
        }
    }
}