using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnlockItemPopUp : MonoBehaviour
{
    [SerializeField] private Text amountText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform coinsBtnRT, watchAdBtnRT, unlockBtnRT, bgRT;
    private int coinAmount = 0;
    private UnityAction callBack, onCoinsUsedCallBack;

    internal void SetThis(Sprite iconSp, int offset, int coinAmount, UnityAction callBack, UnityAction onCoinsUsedCallBack = null, bool isPencil = false)
    {
        icon.sprite = iconSp;
        if (coinAmount > 0)
        {
            this.coinAmount = coinAmount;
            amountText.text = coinAmount.ToString();
            coinsBtnRT.gameObject.SetActive(true);
            watchAdBtnRT.gameObject.SetActive(true);
            unlockBtnRT.gameObject.SetActive(false);
        }
        else
        {
            coinsBtnRT.gameObject.SetActive(false);
            watchAdBtnRT.gameObject.SetActive(false);
            unlockBtnRT.gameObject.SetActive(true);
            messageText.text = "Watch an video ad to\nunlock this item for free";
            bgRT.sizeDelta = new Vector2(bgRT.rect.width, 830);
        }
        if (isPencil)
        {
            RectTransform iconRT = icon.GetComponent<RectTransform>();
            iconRT.offsetMax = new Vector2(193, -22);
            iconRT.offsetMin = new Vector2(-7, -222);
            iconRT.localEulerAngles = new Vector3(0, 0, -4);
        }
        else
        {
            if (offset > 0)
            {
                RectTransform iconRT = icon.GetComponent<RectTransform>();
                iconRT.offsetMax = new Vector2(-offset, -offset);
                iconRT.offsetMin = new Vector2(offset, offset);
            }
        }

        this.callBack = callBack;
        this.onCoinsUsedCallBack = onCoinsUsedCallBack;
    }

    internal void UnlockItemCallBack()
    {
        callBack?.Invoke();
        GameManager.Inst.HidePopUp(gameObject);
    }

    public void On_Coins_Btn_Click()
    {
        if (GeneralDataManager.Coins < coinAmount)
        {
            GameManager.Inst.Show_Toast("Not enough coins!");
            return;
        }

        GeneralDataManager.Coins -= coinAmount;
        GameManager.Inst.SetCoinsText(GeneralDataManager.Coins.ToString());
        UnlockItemCallBack();
        onCoinsUsedCallBack?.Invoke();
    }

    public void On_WatchAd_Btn_Click()
    {
        AdsManager.Inst.RequestAndLoadRewardedAd("UnlockItem");
    }

    public void On_Close_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }
}