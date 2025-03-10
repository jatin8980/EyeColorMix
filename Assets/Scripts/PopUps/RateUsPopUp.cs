using UnityEngine;

public class RateUsPopUp : MonoBehaviour
{
    public void On_No_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }

    public void On_Yes_Btn_Click()
    {
        GameManager.Inst.On_Rate_Btn_Click();
    }
}