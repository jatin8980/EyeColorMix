using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class IAPProductButton : MonoBehaviour
{
    //public TextMeshProUGUI titleText, oldPriceText, priceText, offerNameText;
    //Je rite file ma reward ni index set kari 6 te rite karvi.
    /*[Header("Put text here in same order of data.")]
    public TextMeshProUGUI[] rewardTexts;*/

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            IAPManager.inst.BuyProduct(IAPManager.inst.productIDs[int.Parse(this.name)]);
        }
        else
        {
            GameManager.Inst.Show_Toast("No Internet Connection!");
        }
    }

    /*public void Update_IAP_Button()
    {
        IAPManager.IAPItem iAPItem = IAPManager.inst.iAPSavedDataDictonary[IAPManager.inst.productIDs[int.Parse(this.name)]];
        if (titleText != null)
            titleText.text = iAPItem.packName;

        for (int i = 0; i < iAPItem.rewards.Count; i++)
        {
            if (iAPItem.rewards.ElementAt(i).Key != "removeads")
                rewardTexts[i].text = iAPItem.rewards.ElementAt(i).Value.ToString();
        }

        if (offerNameText != null)
        {
            if (iAPItem.offerName.Contains("Null"))
                offerNameText.transform.parent.gameObject.SetActive(false);
            else
                offerNameText.text = iAPItem.offerName;
        }

        priceText.text = iAPItem.price;

        if (oldPriceText != null && iAPItem.offerPr > 0)
        {
            string priceString = priceText.text;
            priceString = String.Concat(priceString.Where(c => !Char.IsWhiteSpace(c)));
            string currencySign = "";
            string stringStorePrice = "";
            int lastIndexOfDot = priceString.LastIndexOf(".");

            for (int i = 0; i < priceString.Length; i++)
            {
                string ch = priceString.Substring(i, 1);
                if (ch.ToString().Contains(".") && i == lastIndexOfDot)
                {
                    stringStorePrice += ".";
                    continue;
                }
                if (int.TryParse(ch.ToString(), out int parsed))
                    stringStorePrice += parsed.ToString();
                else
                    currencySign += ch.ToString();
            }

            float storePrice = float.Parse(String.Format("{0:0.00}", stringStorePrice));
            float oldPrice = (storePrice * 100f) / (100f - iAPItem.offerPr);
            oldPriceText.text = currencySign + String.Format("{0:0.00}", oldPrice);
        }
    }*/
}