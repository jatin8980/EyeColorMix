using DG.Tweening;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPopUp : MonoBehaviour
{
    [SerializeField] private RectTransform doubleRewardBtnTr, coinPrefabRT, coinStartPos, closeBtnTR;
    [SerializeField] private GameObject claimBtnOb;
    [SerializeField] private Text doubleRewardText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Animation giftAnim;
    private int rewardAmount = 50;

    private void Start()
    {
        doubleRewardBtnTr.gameObject.SetActive(false);
        closeBtnTR.gameObject.SetActive(false);
        claimBtnOb.SetActive(true);
        GameManager.Inst.SetCoinParentAbovePopUp(true);
        switch (GeneralDataManager.DailyRewardDayCount)
        {
            case 1:
                rewardAmount = Random.Range(4, 6) * 10;
                break;
            case 2:
                rewardAmount = Random.Range(6, 8) * 10;
                break;
            case 3:
                rewardAmount = Random.Range(8, 10) * 10;
                break;
            case 4:
                rewardAmount = Random.Range(10, 12) * 10;
                break;
            case 5:
                rewardAmount = Random.Range(12, 14) * 10;
                break;
            case 6:
                rewardAmount = Random.Range(14, 16) * 10;
                break;
            default:
                rewardAmount = Random.Range(14, 17) * 10;
                break;
        }
        doubleRewardText.text = "Get " + (rewardAmount * 2);
        rewardText.text = "+" + rewardAmount;
    }

    private void OnDestroy()
    {
        GameManager.Inst.SetCoinParentAbovePopUp(false);
    }

    private void ClaimCoins()
    {
        GameManager.Inst.StartCoroutine(CollectCoins(false, GeneralDataManager.Coins));
        doubleRewardBtnTr.localScale = Vector3.zero;
        doubleRewardBtnTr.gameObject.SetActive(true);
        doubleRewardBtnTr.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        closeBtnTR.localScale = Vector3.zero;
        closeBtnTR.gameObject.SetActive(true);
        closeBtnTR.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    private IEnumerator CollectCoins(bool isDoubleReward, int coinsToSet)
    {
        yield return new WaitForEndOfFrame();
        Vector3 targetPos = GameManager.Inst.GetCoinTarget().position;
        coinPrefabRT.sizeDelta = new(65, 65);
        float delay = 0.1f;
        for (int i = 0; i < 10; i++)
        {
            RectTransform rt = Instantiate(coinPrefabRT, GameManager.Inst.coinCollectParentTR);
            rt.position = coinStartPos.position;
            rt.DOMove(targetPos, 1f).SetDelay(delay);
            rt.DOSizeDelta(new(75, 75), 1f).SetDelay(delay);
            Destroy(rt.gameObject, 1 + delay);
            delay += 0.1f;
        }
        yield return new WaitForSeconds(1f + delay);
        GameManager.Inst.SetCoinsText(coinsToSet.ToString());
        SoundManager.Inst.Play("CoinCollect");
        if (this != null)
        {
            if (isDoubleReward)
            {
                GameManager.Inst.HidePopUp(gameObject);
            }
        }
    }

    internal void DoubleRewardCallBack()
    {
        rewardText.text = "+" + (rewardAmount * 2);
        GeneralDataManager.Coins += rewardAmount;
        doubleRewardBtnTr.gameObject.SetActive(false);
        GameManager.Inst.StartCoroutine(CollectCoins(true, GeneralDataManager.Coins));
    }

    public void On_Claim_Btn_Click()
    {
        giftAnim.Play();
        GeneralDataManager.Coins += rewardAmount;
        claimBtnOb.SetActive(false);
        GeneralDataManager.DailyRewardClaimedDate = System.DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        Invoke(nameof(ClaimCoins), 1.67f);
        if (GeneralDataManager.DailyRewardDayCount < 7)
            GeneralDataManager.DailyRewardDayCount++;
    }

    public void On_Double_Reward_Btn_Click()
    {
        AdsManager.Inst.RequestAndLoadRewardedAd("DailyDoubleReward");
    }

    public void On_Close_Btn_Click()
    {
        if (closeBtnTR.gameObject.activeSelf)
        {
            GameManager.Inst.HidePopUp(gameObject);
        }
    }
}
