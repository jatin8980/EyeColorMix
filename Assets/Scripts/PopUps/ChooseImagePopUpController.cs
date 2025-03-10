using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseImagePopUpController : MonoBehaviour
{
    [SerializeField] private DynamicScrollView imageScrollView;
    [SerializeField] private GameObject chooseImageOb, viewPortOb;
    [SerializeField] private CropController cropController;
    [SerializeField] private RectTransform categoryHeaderRT;
    internal static ChooseImagePopUpController Inst;
    internal int selectedCategoryIndex = 1, spacing = 20, imageIndexToUnlock;
    internal List<List<int>> lockedByAdImages = new(), unlockedImages;
    internal List<List<int>> orderToShowImage = new() { new(), new(), new(), new(), new(), new(), new() };
    private List<int> totalImagesPerCatetory = new() { 20, 18, 20, 20, 20, 21, 20 };
    private List<bool> isAlreadyShuffled = new() { false, false, false, false, false, false, false };
    private Color32 defaultColor = new(147, 147, 147, 255), selectedColor = new(80, 80, 80, 255);

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        RectTransform rt = imageScrollView.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -50);
        RefreshForNativeAd();
        GameManager.Inst.SetActiveCoins(false);

        lockedByAdImages.Add(new() { 2, 3, 6, 8, 11, 15, 16, 17 });//imageIndex
        lockedByAdImages.Add(new() { 1, 3, 5, 6, 8, 9, 10, 12 });
        lockedByAdImages.Add(new() { 2, 6, 8, 13, 15, 17, 18, 19 });
        lockedByAdImages.Add(new() { 4, 6, 8, 10, 13, 15, 16, 19 });
        lockedByAdImages.Add(new() { 1, 3, 4, 7, 16, 17, 18, 19 });
        lockedByAdImages.Add(new() { 1, 2, 4, 5, 10, 15, 17, 19 });
        lockedByAdImages.Add(new() { 1, 2, 5, 9, 11, 13, 14, 19 });
        unlockedImages = GeneralDataManager.UnlockedImages;
        GameManager.Inst.homeScreen.characterTR.DOKill();
        GameManager.Inst.homeScreen.characterTR.DOScale(1.2f, 0.5f);
        GameManager.Inst.homeScreen.characterTR.DOLocalMove(new(0, -1.12f, 0), 0.5f);
        On_Category_Btn_Click(0);
        cropController.gameObject.SetActive(false);
        float canvasWidth = GetComponent<RectTransform>().rect.width;
        categoryHeaderRT.sizeDelta = new(Mathf.Clamp(canvasWidth - 100, 664, 900), categoryHeaderRT.rect.height);
    }

    private void OnDestroy()
    {
        GameManager.Inst.homeScreen.characterTR.DOKill();
        GameManager.Inst.homeScreen.characterTR.DOScale(GameManager.Inst.homeScreen.CharacterScale(), 0.5f);
        GameManager.Inst.homeScreen.characterTR.DOLocalMove(new Vector3(0, GameManager.Inst.homeScreen.CharacterScale() - 1f, 0), 0.5f);
        GameManager.Inst.SetActiveCoins(true);
    }

    private void ShuffleOrderToShowForCurrentCategory()
    {
        List<int> unshuffledOrder = new();
        for (int i = 0; i < totalImagesPerCatetory[selectedCategoryIndex]; i++)
        {
            unshuffledOrder.Add(i);
        }
        while (unshuffledOrder.Count > 0)
        {
            int index = Random.Range(0, unshuffledOrder.Count);
            orderToShowImage[selectedCategoryIndex].Add(unshuffledOrder[index]);
            unshuffledOrder.RemoveAt(index);
        }
    }

    internal string CategoryIndexToString(int index)
    {
        return index switch
        {
            0 => "cartoon",
            1 => "character",
            2 => "city",
            3 => "emoji",
            4 => "flowers",
            5 => "food",
            6 => "nature",
            _ => "cartoon",
        };
    }

    internal void UnlockImageCallBack()
    {
        unlockedImages[selectedCategoryIndex].Add(imageIndexToUnlock);
        GeneralDataManager.UnlockedImages = unlockedImages;
        GameManager.Inst.Show_Toast("Image unlocked!");
        imageScrollView.refresh();
    }

    internal void OnImageItemClick(Sprite sprite)
    {
        GameManager.Inst.gamePlayUi.colorSelector.SetImage(sprite);
        GameManager.Inst.Show_Screen(GameManager.Screens.GamePlay);
        GameManager.Inst.HidePopUp(gameObject);
    }

    internal void RefreshForNativeAd()
    {
        RectTransform rt = imageScrollView.GetComponent<RectTransform>();
        rt.DOKill();
        if (AdsManager.Inst.isNativeAdLoaded)
        {
            rt.DOAnchorPosY(565f, 0.2f);
        }
        else
        {
            rt.DOAnchorPosY(395f, 0.2f);
        }
    }

    public void On_Category_Btn_Click(int index)
    {
        if (selectedCategoryIndex == index)
            return;
        categoryHeaderRT.GetChild(selectedCategoryIndex).GetComponent<Image>().color = defaultColor;
        categoryHeaderRT.GetChild(selectedCategoryIndex).localScale = Vector3.one;
        categoryHeaderRT.GetChild(index).GetComponent<Image>().color = selectedColor;
        categoryHeaderRT.GetChild(index).localScale = Vector3.one * 1.15f;
        selectedCategoryIndex = index;
        imageScrollView.GetComponent<ScrollRect>().velocity = Vector2.zero;
        if (index == 7)
        {
            chooseImageOb.SetActive(true);
            viewPortOb.SetActive(false);
        }
        else
        {
            viewPortOb.SetActive(true);
            chooseImageOb.SetActive(false);
            if (!isAlreadyShuffled[selectedCategoryIndex])
            {
                isAlreadyShuffled[selectedCategoryIndex] = true;
                ShuffleOrderToShowForCurrentCategory();
            }
            imageScrollView.totalItemCount = totalImagesPerCatetory[selectedCategoryIndex];
            imageScrollView.refresh();
        }
        RectTransform contentRt = viewPortOb.transform.GetChild(0).GetComponent<RectTransform>();
        contentRt.anchoredPosition = new(0, contentRt.anchoredPosition.y);
    }

    public void On_Back_Btn_Click()
    {
        if (Input.touchCount > 1)
            return;
        GameManager.Inst.HidePopUp(gameObject);
        GameManager.Inst.Show_Screen(GameManager.Screens.Home);
    }

    public void On_Choose_Image_Btn_Click()
    {
        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image) !=
           NativeGallery.Permission.Granted)
        {
            NativeGallery.Permission permissionStatus = NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
            if (permissionStatus != NativeGallery.Permission.Granted)
            {
                if (permissionStatus == NativeGallery.Permission.Denied)
                {
                    //User choose dont ask again option, you can show popup here for instructions to allow permission from setting/appinfo
                    GameManager.Inst.Show_Popup(GameManager.Popups.Permission, false);
                }
                //Debug.LogError("No Permission!");
                return;
            }
        }

        NativeGallery.GetImageFromGallery(path =>
        {
            if (path == null)
            {
                //Debug.LogError("Path null");
                return;
            }

            var texture = NativeGallery.LoadImageAtPath(path);

            if (texture == null)
            {
                //Debug.LogError("null texture");
                return;
            }

            cropController.gameObject.SetActive(true);
            cropController.SetThis(texture);
        });
    }
}