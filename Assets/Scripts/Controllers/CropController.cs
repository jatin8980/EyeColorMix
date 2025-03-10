using UnityEngine;
using UnityEngine.UI;

public class CropController : MonoBehaviour
{
    [SerializeField] private ZoomController zoomController;
    [SerializeField] private RectTransform leftBottomRef, iconRT, selectImgRt, dummyLeftBottomRT;
    [SerializeField] private RawImage selectedImg;
    private Color[] allCol;
    private int rotateId, cropSize = 600;
    private FillType fillType;
    private float startMultiplier;
    internal int actualCropSize = 600;

    private enum FillType
    {
        LeftRightDownUp,
        UpDownLeftRight,
        DownUpRightLeft,
        RightLeftUpDown,
        RightLeftDownUp,
        DownUpLeftRight,
        LeftRightUpDown,
        UpDownRightLeft
    }

    private static Color[] DuplicateTexture(Texture source)
    {
        var renderTex = RenderTexture.GetTemporary(source.width, source.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, renderTex);
        var previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        var readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText.GetPixels();
    }

    private void OnDisable()
    {
        Input.multiTouchEnabled = false;
    }

    internal void SetThis(Texture2D texture)
    {
        selectedImg.texture = texture;
        selectedImg.color = Color.white;
        rotateId = 0;
        float minSize = Mathf.Min(selectImgRt.rect.height, selectImgRt.rect.width);
        selectImgRt.sizeDelta = new Vector2(minSize, minSize);
        cropSize = actualCropSize = Mathf.FloorToInt(minSize);
        allCol = DuplicateTexture(texture);

        var width = texture.width;
        var height = texture.height;
        minSize = Mathf.Min(width, height);
        actualCropSize = Mathf.FloorToInt(minSize);

        if (width < height)
        {
            iconRT.sizeDelta = new Vector2(cropSize, cropSize * ((float)height / width));
        }
        else
        {
            iconRT.sizeDelta = new Vector2(cropSize * ((float)width / height), cropSize);
        }
        CalculateMultiplier();
        zoomController.SetData(iconRT.sizeDelta.x, iconRT.sizeDelta.y, this);
        Input.multiTouchEnabled = true;
    }

    internal void CalculateMultiplier()
    {
        if (selectedImg.texture.width < selectedImg.texture.height)
        {
            startMultiplier = selectedImg.texture.width / iconRT.sizeDelta.x;
        }
        else
        {
            startMultiplier = selectedImg.texture.height / iconRT.sizeDelta.y;
        }
    }

    public void On_Done_Btn_Click()
    {
        if (Input.touchCount > 1)
            return;
        GameManager.Inst.loaderOb.SetActive(true);
        Invoke(nameof(SaveImg), 0.5f);
    }

    private void SaveImg()
    {
        Vector2Int startPos = Vector2Int.zero;

        dummyLeftBottomRT.parent.position = iconRT.parent.position;
        leftBottomRef.position = dummyLeftBottomRT.position;

        startPos.x = Mathf.RoundToInt(leftBottomRef.anchoredPosition.x * startMultiplier);
        startPos.y = Mathf.RoundToInt(leftBottomRef.anchoredPosition.y * startMultiplier);
        Texture2D tempTexture = new Texture2D(actualCropSize, actualCropSize, TextureFormat.ARGB32, false);
        var a = -1;
        int b;

        float x = iconRT.localScale.x;
        float y = iconRT.localScale.y;
        switch (rotateId)
        {
            case 0:
                if (x == 1 && y == 1)
                    fillType = FillType.LeftRightDownUp;
                else if (x == -1 && y == -1)
                    fillType = FillType.RightLeftUpDown;
                else if (x == -1)
                    fillType = FillType.RightLeftDownUp;
                else if (y == -1)
                    fillType = FillType.LeftRightUpDown;
                break;
            case 1:
                if (x == 1 && y == 1)
                    fillType = FillType.UpDownLeftRight;
                else if (x == -1 && y == -1)
                    fillType = FillType.DownUpRightLeft;
                else if (x == -1)
                    fillType = FillType.DownUpLeftRight;
                else if (y == -1)
                    fillType = FillType.UpDownRightLeft;
                break;
            case 2:
                if (x == 1 && y == 1)
                    fillType = FillType.RightLeftUpDown;
                else if (x == -1 && y == -1)
                    fillType = FillType.LeftRightDownUp;
                else if (x == -1)
                    fillType = FillType.LeftRightUpDown;
                else if (y == -1)
                    fillType = FillType.RightLeftDownUp;
                break;

            case 3:
                if (x == 1 && y == 1)
                    fillType = FillType.DownUpRightLeft;
                else if (x == -1 && y == -1)
                    fillType = FillType.UpDownLeftRight;
                else if (x == -1)
                    fillType = FillType.UpDownRightLeft;
                else if (y == -1)
                    fillType = FillType.DownUpLeftRight;
                break;
        }

        switch (fillType)
        {
            case FillType.UpDownLeftRight:
                b = actualCropSize - 1;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    a = 0;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        a++;
                    }
                    b--;
                }
                break;

            case FillType.RightLeftUpDown:
                a = actualCropSize - 1;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    b = actualCropSize - 1;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        b--;
                    }
                    a--;
                }
                break;

            case FillType.LeftRightDownUp:
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    a++;
                    b = 0;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        b++;
                    }
                }
                break;

            case FillType.DownUpRightLeft:
                b = 0;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    a = actualCropSize - 1;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        a--;
                    }
                    b++;
                }
                break;

            case FillType.RightLeftDownUp:
                a = actualCropSize - 1;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    b = 0;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        b++;
                    }
                    a--;
                }
                break;

            case FillType.DownUpLeftRight:
                b = 0;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    a = 0;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        a++;
                    }
                    b++;
                }
                break;

            case FillType.LeftRightUpDown:
                a = 0;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    b = actualCropSize - 1;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        b--;
                    }
                    a++;
                }
                break;

            case FillType.UpDownRightLeft:
                b = actualCropSize - 1;
                for (var i = startPos.x; i < startPos.x + actualCropSize; i++)
                {
                    a = actualCropSize - 1;
                    for (var j = startPos.y; j < startPos.y + actualCropSize; j++)
                    {
                        tempTexture.SetPixel(a, b, allCol[i + j * selectedImg.texture.width]);
                        a--;
                    }
                    b--;
                }
                break;

            default: return;
        }
        tempTexture.Apply();
        GameManager.Inst.loaderOb.SetActive(false);
        ChooseImagePopUpController.Inst.OnImageItemClick(Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0.5f, 0.5f)));
    }

    public void Rotate_Btn_Click()
    {
        rotateId++;
        if (rotateId > 3) rotateId = 0;
        var angle = iconRT.localEulerAngles;
        angle.z = rotateId * -90;
        iconRT.localEulerAngles = angle;
        zoomController.Rotate();
    }

    public void On_LR_Flip_Btn_Click()
    {
        if (rotateId == 0 || rotateId == 2)
        {
            iconRT.localScale = new Vector3(iconRT.localScale.x > 0 ? -1 : 1, iconRT.localScale.y, 1);
        }
        else
        {
            iconRT.localScale = new Vector3(iconRT.localScale.x, iconRT.localScale.y > 0 ? -1 : 1, 1);
        }
    }

    public void On_DU_Flip_Btn_Click()
    {
        if (rotateId == 1 || rotateId == 3)
        {
            iconRT.localScale = new Vector3(iconRT.localScale.x > 0 ? -1 : 1, iconRT.localScale.y, 1);
        }
        else
        {
            iconRT.localScale = new Vector3(iconRT.localScale.x, iconRT.localScale.y > 0 ? -1 : 1, 1);
        }
    }
}