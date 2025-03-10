using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class FreeCoinsPopUp : MonoBehaviour
{
    [SerializeField] private RectTransform coinPrefabRT, coinStartPos;
    [SerializeField] private GameObject watchAdBtnOb;

    private void Start()
    {
        GameManager.Inst.SetCoinParentAbovePopUp(true);
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(false);
    }

    private void OnDestroy()
    {
        if (GameManager.activePopUps.Last() == GameManager.Popups.Null || GameManager.activePopUps.Last() == GameManager.Popups.TutorialPopUp)
            GameManager.Inst.SetCoinParentAbovePopUp(false);
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(true);
    }

    private IEnumerator CollectCoins()
    {
        Vector3 targetPos = GameManager.Inst.GetCoinTarget().position;
        coinPrefabRT.sizeDelta = new(75, 75);
        float delay = 0.1f;
        for (int i = 0; i < 10; i++)
        {
            RectTransform rt = Instantiate(coinPrefabRT, GameManager.Inst.coinCollectParentTR);
            rt.position = coinStartPos.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            rt.DOMove(targetPos, 1f).SetDelay(delay);
            Destroy(rt.gameObject, 1 + delay);
            delay += 0.1f;
        }
        yield return new WaitForSeconds(1f + delay);
        GameManager.Inst.SetCoinsText(GeneralDataManager.Coins.ToString());
        SoundManager.Inst.Play("CoinCollect");
        if (this != null)
            GameManager.Inst.HidePopUp(gameObject);
    }

    internal void FreeCoinsCallBack()
    {
        GeneralDataManager.Coins += 100;
        watchAdBtnOb.SetActive(false);
        GameManager.Inst.StartCoroutine(CollectCoins());
    }

    public void On_Close_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }

    public void On_Watch_Btn_Click()
    {
        AdsManager.Inst.RequestAndLoadRewardedAd("FreeCoins");
    }
}