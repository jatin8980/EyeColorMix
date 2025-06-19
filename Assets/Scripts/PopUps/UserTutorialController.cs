using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UserTutorialController : MonoBehaviour
{
    [SerializeField] private RectTransform colorTutorialParentRT, drawTutorialParent, stretchTutorialParent, messageBgRT;
    public RectTransform dummyRT, handRT;
    internal static UserTutorialController Inst;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject tickOb;
    Tween t;

    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        dummyRT.gameObject.SetActive(false);
        tickOb.SetActive(false);
        SetActiveHand(false);

        if (!GeneralDataManager.IsPatternTutorialShowed && PatternController.Inst != null && PatternController.Inst.gameObject.activeSelf)
        {
            ShowPatternTutorial();
        }
        else
        {
            switch (GeneralDataManager.TutorialStep)
            {
                case 1:
                    StartColorTutorial();
                    break;
                case 2:
                    StartDrawTutorial();
                    break;
                case 3:
                    ShowMessageForStep3();
                    break;
                case 4:
                    ShowMessageForStep4();
                    break;
                case 5:
                    StartStretchTutorial();
                    break;
                case 6:
                    ShowMessageForStep6();
                    break;
                case 7:
                    ShowMessageForStep7();
                    break;
                case 8:
                    ShowMessageForStep8();
                    break;
                case 9:
                    ShowMessageForStep9();
                    break;
                case 10:
                    ShowMessageForStep10();
                    break;
                case 11:
                    ShowMessageForStep11();
                    break;
                case 12:
                    ShowMessageForStep12();
                    break;
            }
        }
    }

    private void OnDisable()
    {
        switch (GeneralDataManager.TutorialStep)
        {
            case 1:
                GameManager.Inst.gamePlayUi.drawController.gameObject.SetActive(true);
                GameManager.Inst.gamePlayUi.pencilController.pencilRT.pivot = new Vector2(0.5f, 1f);
                break;
        }
        t?.Kill();
    }

    private void Update()
    {
        if (GeneralDataManager.TutorialStep == 2 || GeneralDataManager.TutorialStep == 5)
        {
            if (Input.GetMouseButtonDown(0))
            {
                t.Pause();
                SetActiveHand(false);
            }
            if (Input.GetMouseButtonUp(0))
            {
                t.Play();
                GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
                SetActiveHand(true);
            }
        }
        else if (GeneralDataManager.TutorialStep == 9 || GeneralDataManager.TutorialStep == 10)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetActiveHand(false);
            }
            if (Input.GetMouseButtonUp(0))
            {
                SetActiveHand(true);
            }
        }
        else if (GeneralDataManager.TutorialStep == 7 || (GeneralDataManager.TutorialStep == 12 && (PatternController.Inst == null || !PatternController.Inst.gameObject.activeSelf)))
        {
            if (Input.GetMouseButtonDown(0))
                SetActiveHand(false);
        }
    }

    private void StartColorTutorial()
    {
        SetActiveHand(true);
        SetHand(true);
        Canvas cs = handRT.AddComponent<Canvas>();
        cs.overrideSorting = true;
        cs.sortingOrder = 102;
        handRT.GetComponent<Animation>().enabled = false;
        messageText.text = "Tap and move\nto pick the color";
        GameManager.Inst.gamePlayUi.drawController.gameObject.SetActive(false);
        colorTutorialParentRT.position = GameManager.Inst.gamePlayUi.colorSelector.transform.position;
        Vector3[] path = new Vector3[5];
        for (int i = 0; i < 4; i++)
        {
            path[i] = colorTutorialParentRT.GetChild(i).position;
        }
        dummyRT.position = path[4] = path[0];
        GameManager.Inst.gamePlayUi.colorSelector.On_Color_Choose_Img_Down(true);
        dummyRT.DOPath(path, 5f, PathType.CatmullRom).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).OnUpdate(() =>
        {
            handRT.position = dummyRT.position;
        });
        GameManager.Inst.gamePlayUi.pencilController.pencilRT.pivot = new Vector2(0.49f, 0.997f);
    }

    private void StartDrawTutorial()
    {
        SetActiveHand(true);
        handRT.GetComponent<Animation>().enabled = false;
        SetHand(true);
        messageText.text = "Fill the first circle";
        drawTutorialParent.position = GameManager.Inst.gamePlayUi.drawController.transform.position;
        drawTutorialParent.localScale = GameManager.Inst.gamePlayUi.drawController.transform.localScale;
        Vector3[] path = new Vector3[drawTutorialParent.childCount];
        for (int i = 0; i < drawTutorialParent.childCount; i++)
        {
            path[i] = drawTutorialParent.GetChild(i).position;
        }
        dummyRT.position = path[0];
        GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
        t = dummyRT.DOPath(path, 10f, PathType.CatmullRom).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).OnUpdate(() =>
        {
            GameManager.Inst.gamePlayUi.pencilController.targetPos = handRT.position = dummyRT.position;
        });
    }

    private void ShowMessageForStep3()
    {
        messageText.text = "Now fill remaining circles\nwith different colors";
    }

    private void ShowMessageForStep4()
    {
        tickOb.SetActive(true);
        messageText.text = "Amazing!\nLet's continue";
        SetActiveHand(true);
        handRT.position = GameManager.Inst.gamePlayUi.GetNextButtonPosition();
        handRT.anchoredPosition -= new Vector2(100, 0);
        handRT.localEulerAngles = new(0, 0, -25);
        messageBgRT.anchorMax = messageBgRT.anchorMin = Vector2.one;
        messageBgRT.anchoredPosition = new(0, -975 - GameManager.Inst.GetDifferenceFromTop());
    }

    private void StartStretchTutorial()
    {
        SetActiveHand(true);
        SetHand(true);
        handRT.GetComponent<Animation>().enabled = false;
        messageText.text = "Blur contacts in\nall directions";
        stretchTutorialParent.position = GameManager.Inst.gamePlayUi.drawController.transform.position;
        stretchTutorialParent.localScale = GameManager.Inst.gamePlayUi.bigScale * Vector3.one;

        Vector3[] path = new Vector3[stretchTutorialParent.childCount * 2 + 1];
        int childIndex = 0;
        for (int i = 0; i < path.Length - 1; i += 2)
        {
            path[i] = stretchTutorialParent.position;
            path[i + 1] = stretchTutorialParent.GetChild(childIndex).position;
            childIndex++;
        }
        path[path.Length - 1] = path[0];
        dummyRT.position = path[0];
        GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
        t = dummyRT.DOPath(path, 30f, PathType.CatmullRom).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).OnUpdate(() =>
        {
            GameManager.Inst.gamePlayUi.pencilController.targetPos = handRT.position = dummyRT.position;

        });
    }

    private void ShowMessageForStep6()
    {
        tickOb.SetActive(true);
        messageText.text = "Perfect!\nLet's continue";
        SetActiveHand(true);
        handRT.position = GameManager.Inst.gamePlayUi.GetNextButtonPosition();
        handRT.anchoredPosition -= new Vector2(100, 0);
        handRT.localEulerAngles = new(0, 0, -25);
        messageBgRT.anchorMax = messageBgRT.anchorMin = Vector2.one;
        messageBgRT.anchoredPosition = new(0, -975 - GameManager.Inst.GetDifferenceFromTop());
    }

    private void ShowMessageForStep7()
    {
        messageText.text = "Choose pupil\nfor contact lens";
        messageBgRT.anchoredPosition = new(0, -65);
        handRT.localEulerAngles = new(0, 0, 25);
    }

    private void ShowMessageForStep8()
    {
        tickOb.SetActive(true);
        messageText.text = "Choose pupil\nfor contact lens";
        handRT.position = GameManager.Inst.gamePlayUi.GetNextButtonPosition();
        messageBgRT.anchoredPosition = new(0, -65);
    }

    private void ShowMessageForStep9()
    {
        messageText.text = "Drag contact lens onto the left eye";
        SetActiveHand(true);
        handRT.GetComponent<Animation>().enabled = false;
        handRT.GetChild(0).GetComponent<RectTransform>().sizeDelta = handRT.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(250, 250);
    }

    private void ShowMessageForStep10()
    {
        messageText.text = "Now drag onto the right eye";
        SetActiveHand(true);
        handRT.GetComponent<Animation>().enabled = false;
        handRT.GetChild(0).GetComponent<RectTransform>().sizeDelta = handRT.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(250, 250);
    }

    private void ShowMessageForStep11()
    {
        messageText.text = "Select and tune effect";
        SetHand(true);
        handRT.GetComponent<Animation>().enabled = false;
        handRT.localEulerAngles = new(0, 0, 60);
        messageBgRT.anchoredPosition = new(0, -65);
    }

    private void ShowMessageForStep12()
    {
        messageText.text = "Choose highlight\nfor contact lens";
        messageBgRT.anchoredPosition = new(0, -65);
        handRT.localEulerAngles = new(0, 0, 25);
    }

    private void ShowPatternTutorial()
    {
        messageText.text = "Select and tune pattern";
        SetHand(true);
        handRT.GetComponent<Animation>().enabled = false;
        handRT.localEulerAngles = new(0, 0, 60);
        messageBgRT.anchoredPosition = new(0, -65);
    }

    private void SetHand(bool isDown)
    {
        handRT.GetChild(0).gameObject.SetActive(!isDown);
        handRT.GetChild(1).gameObject.SetActive(isDown);
    }

    internal IEnumerator SetHandAnimForLens(Transform startPosTR, Vector3 endPos)
    {
        handRT.position = startPosTR.position;
    Anim:
        SetHand(false);
        yield return new WaitForSeconds(0.1f);
        SetHand(true);
        yield return new WaitForSeconds(0.1f);
        handRT.DOMove(endPos, 1f);
        yield return new WaitForSeconds(1.1f);
        SetHand(false);
        yield return new WaitForSeconds(0.1f);
        handRT.DOMove(startPosTR.position, 1f);
        yield return new WaitForSeconds(1f);
        goto Anim;
    }

    internal void SetActiveHand(bool show)
    {
        handRT.gameObject.SetActive(show);
    }
}
