using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] private Image colorChooseImg, currentChoosedColorImg, borderImage, colorChangeAnimImage;
    private RectTransform colorChooseRT, currentChoosedColorRT;
    internal bool isFingerOnColorChooseImage, isColorChoosed;
    private Texture2D colorChooseTexture;
    private void OnEnable()
    {
        currentChoosedColorImg.gameObject.SetActive(false);
        currentChoosedColorImg.color = GameManager.Inst.gamePlayUi.drawController.alreadyDrawColor;
        borderImage.color = Color.white;
    }

    private void Start()
    {
        colorChooseRT = colorChooseImg.GetComponent<RectTransform>();
        currentChoosedColorRT = currentChoosedColorImg.GetComponent<RectTransform>();
        float size = Mathf.Min(GetComponent<RectTransform>().rect.width, 475);
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(size, size);
    }

    private void Update()
    {
        if (isFingerOnColorChooseImage || GeneralDataManager.TutorialStep == 1)
        {
            Vector2 anchorPos;
            if (GeneralDataManager.TutorialStep == 1)
            {
                currentChoosedColorRT.position = UserTutorialController.Inst.dummyRT.position;
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(colorChooseRT, Input.mousePosition, GameManager.Inst.mainCamera, out anchorPos);
                anchorPos = new Vector2(Mathf.Clamp(anchorPos.x, -colorChooseRT.rect.width / 2 + 11, colorChooseRT.rect.width / 2 - 11),
                    Mathf.Clamp(anchorPos.y, -colorChooseRT.rect.height / 2 + 11, colorChooseRT.rect.height / 2 - 11));
                currentChoosedColorRT.anchoredPosition = anchorPos;
            }
            anchorPos = currentChoosedColorRT.anchoredPosition;

            GameManager.Inst.gamePlayUi.pencilController.targetPos = currentChoosedColorRT.position;
            GameManager.Inst.gamePlayUi.pencilController.targetPos.z = 90f;
            GameManager.Inst.gamePlayUi.pencilController.smoothTargetPos = GameManager.Inst.gamePlayUi.pencilController.targetPos;
            int x = Mathf.FloorToInt(colorChooseTexture.width * (anchorPos.x - (-colorChooseRT.rect.width / 2)) / colorChooseRT.rect.width);
            int y = Mathf.FloorToInt(colorChooseTexture.width * (anchorPos.y - (-colorChooseRT.rect.height / 2)) / colorChooseRT.rect.height);
            currentChoosedColorImg.color = colorChooseTexture.GetPixel(x, y);
            isColorChoosed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isFingerOnColorChooseImage)
            {
                isFingerOnColorChooseImage = false;
                currentChoosedColorImg.gameObject.SetActive(false);
                GameManager.Inst.gamePlayUi.pencilController.MovePencilToDefaultPos();
                colorChangeAnimImage.DOKill();
                colorChangeAnimImage.DOFade(1, 0.2f).OnComplete(() =>
                {
                    borderImage.color = currentChoosedColorImg.color;
                    colorChangeAnimImage.DOFade(0, 0.2f);
                });

                if (GeneralDataManager.TutorialStep == 2 && UserTutorialController.Inst == null)
                {
                    GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
                }
            }
        }
    }

    internal void SetImage(Sprite sprite)
    {
        colorChooseImg.sprite = sprite;
        colorChooseTexture = sprite.texture;
    }

    internal Color GetSelectedColor() => currentChoosedColorImg.color;

    public void On_Color_Choose_Img_Down(bool isFromTutorial)
    {
        if (UserTutorialController.Inst != null && !isFromTutorial && GeneralDataManager.TutorialStep == 1)
        {
            GameManager.Inst.HidePopUp(UserTutorialController.Inst.gameObject);
            GeneralDataManager.TutorialStep++;// to 2
        }
        if (GameManager.Inst.gamePlayUi.currentStepCount <= 1)
        {

            isFingerOnColorChooseImage = !isFromTutorial;
            currentChoosedColorImg.gameObject.SetActive(true);
            GameManager.Inst.gamePlayUi.pencilController.MovePencilToTargetPos();
        }
    }
}