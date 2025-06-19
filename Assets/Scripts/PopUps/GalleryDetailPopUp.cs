using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryDetailPopUp : MonoBehaviour
{
    [SerializeField] private Image rightEyeIcon, leftEyeIcon, bodyImage, zoomIcon;
    [SerializeField] private Text titleText;
    [SerializeField] private List<Image> colorsImage;
    [SerializeField] private RectTransform shareSSAreaRT, bodyRT, scaleParent, leftEyeRt;
    [SerializeField] private Sprite zoomInSprite, zoomOutSprite;
    private List<Vector2> characterPoses = new() { new(-388, -2195), new(-288, -2644), new(-330, -2055), new(-364, -2649), new(-347, -2492), new(-383, -2672), new(-405, -2007), new(-352, -2754), new(-498, -2300), new(-404, -1963) };
    private List<Vector2> characterSizes = new() { new(4046, 7308), new(5325, 9690), new(4046, 7308), new(5677, 9404), new(4520, 7977), new(4932, 8908), new(4182, 8158), new(4732, 8708), new(4032, 7298), new(4032, 8008) };
    private List<Vector2> leftEyePoses = new() { new(-619, -15), new(-751, -15), new(-773, -15), new(-700, -15), new(-667, -15), new(-732, -15), new(-720, -35), new(-683, -15), new(-718, -23), new(-695, -15) };
    private List<Vector2> zoomOutPoses = new() { new(50, 50), new(20, 17), new(47, 32), new(46, 43), new(34, 46), new(34, 19), new(45, 18), new(34, 50), new(42, 62), new(42, 32) };
    private List<float> zoomOutScales = new() { 0.12f, 0.12f, 0.115f, 0.12f, 0.12f, 0.115f, 0.12f, 0.12f, 0.12f, 0.12f };
    private int characterIndex = 0;
    private bool isZoomIn = true;

    private IEnumerator ShareCoroutine()
    {
        zoomIcon.transform.parent.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        var cam = Camera.main;
        var corners = new Vector3[4];
        shareSSAreaRT.GetWorldCorners(corners);
        var bl = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        var tl = RectTransformUtility.WorldToScreenPoint(cam, corners[1]);
        var tr = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        var height = tl.y - bl.y;
        var width = tr.x - bl.x;

        var tex = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
        var rex = new Rect(bl.x, bl.y, width, height);
        tex.ReadPixels(rex, 0, 0);
        tex.Apply();
        zoomIcon.transform.parent.gameObject.SetActive(true);
        AdsManager.Inst.CanShowAppOpen = false;
        new NativeShareLink().AddFile(tex, Application.productName + ".png").SetText(GeneralDataManager.Inst.ShareMessage).Share();
    }

    internal void SetThis(Sprite sprite, int index)
    {
        rightEyeIcon.sprite = leftEyeIcon.sprite = sprite;
        characterIndex = GalleryPopUp.Inst.listOfCharacterPlayedInLevel[index];
        bodyImage.sprite = GameManager.Inst.homeScreen.modelBodies[characterIndex];
        titleText.text = "Level " + (index + 1).ToString();
        zoomIcon.sprite = zoomInSprite;
        bodyRT.sizeDelta = characterSizes[characterIndex];
        bodyRT.anchoredPosition = characterPoses[characterIndex];
        leftEyeRt.anchoredPosition = leftEyePoses[characterIndex];
        List<Color32> colorsUsed = GeneralDataManager.ListOfColorsUsedInLevel[index];
        for (int i = 0; i < colorsImage.Count; i++)
        {
            colorsImage[i].color = colorsUsed[i];
        }
    }

    public void On_Close_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }

    public void On_Share_Btn_Click()
    {
        StartCoroutine(ShareCoroutine());
    }

    public void On_Mask_Btn_Click()
    {
        if (isZoomIn)
        {
            isZoomIn = false;
            scaleParent.DOKill();
            scaleParent.DOAnchorPos(zoomOutPoses[characterIndex], 0.5f);
            scaleParent.DOScale(zoomOutScales[characterIndex], 0.5f);
            zoomIcon.sprite = zoomOutSprite;
        }
        else
        {
            isZoomIn = true;
            scaleParent.DOKill();
            scaleParent.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.InCubic);
            scaleParent.DOScale(1f, 0.5f).SetEase(Ease.InCubic);
            zoomIcon.sprite = zoomInSprite;
        }
    }
}