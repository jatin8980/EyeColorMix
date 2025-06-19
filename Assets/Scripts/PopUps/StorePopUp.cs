using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class StorePopUp : MonoBehaviour
{
    [SerializeField] private RectTransform coinPrefabRT, contentRT;
    [SerializeField] private GameObject loaderOb;
    internal static StorePopUp Inst;


    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        IAPManager.inst.InitilizitionIAP();
        RefreshRemoveAdsOb();
        SetupIAPList();
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(false);
    }

    private void OnDestroy()
    {
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(true);
    }

    internal IEnumerator CollectCoins(int childIndex)
    {
        Vector3 targetPos = GameManager.Inst.GetCoinTarget().position;
        coinPrefabRT.sizeDelta = new(75, 75);
        float delay = 0.1f;
        Vector3 startPos = contentRT.GetChild(childIndex).GetChild(0).position;
        for (int i = 0; i < 10; i++)
        {
            RectTransform rt = Instantiate(coinPrefabRT, GameManager.Inst.coinCollectParentTR);
            rt.position = startPos;
            rt.DOMove(targetPos, 1f).SetDelay(delay);
            Destroy(rt.gameObject, 1 + delay);
            delay += 0.1f;
        }
        yield return new WaitForSeconds(1f + delay);
        GameManager.Inst.SetCoinsText(GeneralDataManager.Coins.ToString());
        SoundManager.Inst.Play("CoinCollect");
    }

    internal void LoaderSetActive(bool show) => loaderOb.SetActive(show);

    internal void SetupIAPList()
    {
        for (int i = 1; i < contentRT.childCount; i++)
        {
            contentRT.GetChild(i).GetComponent<IAPProductButton>().Update_IAP_Button();
        }
    }

    internal void RefreshRemoveAdsOb()
    {
        contentRT.GetChild(contentRT.childCount - 1).gameObject.SetActive(!GeneralDataManager.IsPurchaseAdsRemoved);
    }

    internal void FreeCoinsCallBack()
    {
        GeneralDataManager.Coins += 100;
        GameManager.Inst.StartCoroutine(CollectCoins(0));
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