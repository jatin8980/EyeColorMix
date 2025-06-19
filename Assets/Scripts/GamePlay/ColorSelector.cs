using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] private Image colorChooseImg, currentChoosedColorImg, borderImage, colorNameBgImage;
    [SerializeField] private TextMeshProUGUI colorNameText;
    private RectTransform colorChooseRT, currentChoosedColorRT;
    internal bool isFingerOnColorChooseImage, isColorChoosed;
    private Texture2D colorChooseTexture;
    private void OnEnable()
    {
        currentChoosedColorImg.gameObject.SetActive(false);
        currentChoosedColorImg.color = GameManager.Inst.gamePlayUi.drawController.alreadyDrawColor;
        borderImage.color = colorNameBgImage.color = Color.white;
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
            currentChoosedColorImg.color = colorNameBgImage.color = colorChooseTexture.GetPixel(x, y);
            SetBorderColor(currentChoosedColorImg.color);
            isColorChoosed = true;

            ColorTextObSetActive(false);
            foreach (ColorDetail colorDetail in GeneralDataManager.Inst.colorDetails)
            {
                ColorUtility.TryParseHtmlString(colorDetail.hex, out Color cl);
                if (Mathf.Abs(currentChoosedColorImg.color.r - cl.r) < 0.05f &&
                    Mathf.Abs(currentChoosedColorImg.color.g - cl.g) < 0.05f &&
                    Mathf.Abs(currentChoosedColorImg.color.b - cl.b) < 0.05f)
                {
                    colorNameText.text = colorDetail.subtitle;
                    ColorTextObSetActive(isFingerOnColorChooseImage);
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isFingerOnColorChooseImage)
            {
                isFingerOnColorChooseImage = false;
                currentChoosedColorImg.gameObject.SetActive(false);
                GameManager.Inst.gamePlayUi.pencilController.MovePencilToDefaultPos();
                if (GeneralDataManager.TutorialStep == 2 && UserTutorialController.Inst == null)
                {
                    GameManager.Inst.Show_Popup(GameManager.Popups.TutorialPopUp);
                }
            }
        }
    }

    internal void SetBorderColor(Color color) => borderImage.color = color;

    internal void ColorTextObSetActive(bool active)
    {
        colorNameBgImage.gameObject.SetActive(active);
        colorNameText.gameObject.SetActive(active);
    }

    internal void SetImage(Sprite sprite)
    {
        colorChooseImg.sprite = sprite;
        colorChooseTexture = sprite.texture;
    }

    internal Color GetSelectedColor() => currentChoosedColorImg.color;

    internal void SetSelectedColor(Color color) => currentChoosedColorImg.color = color;

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
            ColorTextObSetActive(!isFromTutorial);
        }
    }
}