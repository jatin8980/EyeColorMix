using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpClickClose : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.name != name) return;
        SoundManager.Inst.Play("Click");

        if (GameManager.activePopUps.Last() == GameManager.Popups.DailyRewardPopUp)
        {
            GameManager.activePopUpsObs.Last().GetComponent<DailyRewardPopUp>().On_Close_Btn_Click();
            return;
        }
        GameManager.Inst.HidePopUp(gameObject);
    }
}