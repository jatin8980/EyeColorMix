using UnityEngine;
using UnityEngine.Events;

public class NoInternetPopUp : MonoBehaviour
{
    internal UnityAction callBack;

    private void Start()
    {
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(false);
    }
    public void On_Retry_Btn_Click()
    {
        if (GameManager.Is_Internet_Available())
        {
            GameManager.Inst.HidePopUp(gameObject);
            if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
                GameManager.Inst.homeScreen.SetActiveEye(true);
            callBack?.Invoke();
        }
        else
        {
            GameManager.Inst.Show_Toast("Check Network Connection!");
        }
    }
}
