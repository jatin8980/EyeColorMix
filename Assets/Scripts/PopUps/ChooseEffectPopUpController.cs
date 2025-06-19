using DG.Tweening;
using UnityEngine;

public class ChooseEffectPopUpController : MonoBehaviour
{
    [SerializeField] private GameObject patternLockOb;

    private void Start()
    {
        RectTransform bgRt = transform.GetChild(0).GetComponent<RectTransform>();
        bgRt.anchoredPosition = new Vector2(0, -50);
        RefreshForAd();
        patternLockOb.SetActive(GeneralDataManager.Level < 3);
    }

    internal void RefreshForAd()
    {
        RectTransform rt = transform.GetChild(0).GetComponent<RectTransform>();
        rt.DOKill();
        if (AdsManager.Inst.isBannerLoaded)
        {
            rt.DOAnchorPosY(710f + GameManager.Inst.bannerHeight, 0.2f);
        }
        else
        {
            rt.DOAnchorPosY(710f, 0.2f);
        }
    }

    public void On_Effects_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
        GameManager.Inst.gamePlayUi.EffectsSetActive(true);
    }

    public void On_Pattern_Btn_Click()
    {
        if (GeneralDataManager.Level < 3)
        {
            GameManager.Inst.Show_Toast("Unlocks at level 3");
            return;
        }
        GameManager.Inst.HidePopUp(gameObject);
        GameManager.Inst.gamePlayUi.PatternSetActive(true);
    }
}