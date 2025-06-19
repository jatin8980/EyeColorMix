using Mosframe;
using System.Collections.Generic;
using UnityEngine;

public class GalleryPopUp : MonoBehaviour
{
    [SerializeField] private DynamicScrollView scrollView;
    public List<Sprite> rightEyes;
    internal static GalleryPopUp Inst;
    internal int spacing = 30, bigItemsPerRow = 2;
    internal List<int> listOfCharacterPlayedInLevel = new();
    internal List<Vector2> characterPoses = new() { new(12, -4), new(25, 29), new(26, 29), new(-18, 22), new(0, 25), new(-13, 13), new(18, 35), new(-10, 6), new(41, 19), new(35, 25) };
    internal List<Vector2> characterSizes = new() { new(400, 400), new(430, 430), new(440, 440), new(410, 410), new(450, 450), new(430, 430), new(430, 430), new(430, 430), new(500, 500), new(470, 470) };

    private void Awake()
    {
        Inst = this;
        listOfCharacterPlayedInLevel = GeneralDataManager.ListOfCharacterPlayedInLevel;
        if (scrollView.GetComponent<RectTransform>().rect.width > 1000)
        {
            bigItemsPerRow = 3;
            scrollView.totalItemCount = Mathf.CeilToInt(listOfCharacterPlayedInLevel.Count / 3f);
        }
        else
        {
            scrollView.totalItemCount = Mathf.CeilToInt(listOfCharacterPlayedInLevel.Count / 2f);
        }
        RefreshForAd();
    }

    public void On_Back_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }

    internal void RefreshForAd()
    {
        RectTransform rt = scrollView.GetComponent<RectTransform>();
        if (AdsManager.Inst.isBannerLoaded)
        {
            rt.offsetMin = new(rt.offsetMin.x, 150 + GameManager.Inst.bannerHeight + 20);
        }
        else
        {
            rt.offsetMin = new(rt.offsetMin.x, 150);
        }
    }
}
