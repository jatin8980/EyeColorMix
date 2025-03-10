using TMPro;
using UnityEngine;

public class RemoveAdsPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;

    private void Start()
    {
        IAPManager.inst.InitilizitionIAP();
        SetupPriceText();
    }

    internal void SetupPriceText()
    {
        priceText.text = IAPManager.inst.iAPSavedDataDictonary[IAPManager.inst.productIDs[0]].price;
    }

    public void On_Close_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }
}