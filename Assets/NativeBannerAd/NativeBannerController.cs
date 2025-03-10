using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using DG.Tweening;

public class NativeBannerController : MonoBehaviour
{
    public RawImage icon;
    public RawImage[] graphics;
    public TextMeshProUGUI titleText, descriptionText, installBtnText;
    [SerializeField] private RectTransform shineImgRT;

    private void Start()
    {
        this.GetComponent<BoxCollider2D>().size = this.GetComponent<RectTransform>().rect.size;
        Invoke(nameof(DoShine), 3f);
    }

    private void DoShine()
    {
        shineImgRT.anchoredPosition = new Vector2(-55, 0);
        shineImgRT.DOAnchorPosX(shineImgRT.parent.GetComponent<RectTransform>().rect.width + 35, 450).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(() => {
            Invoke(nameof(DoShine), 3f);
        });
    }

    public void Refresh_Details(NativeAd nativeAd)
    {
        icon.texture = nativeAd.GetIconTexture();
        titleText.text = nativeAd.GetHeadlineText();
        descriptionText.text = nativeAd.GetBodyText();
        installBtnText.text = nativeAd.GetCallToActionText();

        for (int i = 0; i < graphics.Length; i++)
        {
            if (nativeAd.GetImageTextures().Count >= graphics.Length)
            {
                graphics[i].texture = nativeAd.GetImageTextures()[i];
            }
        }

        if (!nativeAd.RegisterCallToActionGameObject(this.gameObject))
        {
            Debug.LogError("Error registring Btn!");
        }
    }

    public void On_Banner_Click() => AdsManager.Inst.Invoke(nameof(AdsManager.Inst.Refresh_NativeAd), 0.2f);
}
