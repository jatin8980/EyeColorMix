using UnityEngine;

public class AdLoaderPanelController : MonoBehaviour
{
    private float waitTime = 10f;

    private void OnEnable()
    {
        Invoke(nameof(Disable_Panel), waitTime);
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(false);
    }

    private void Disable_Panel() => gameObject.SetActive(false);

    private void OnDisable()
    {
        CancelInvoke(nameof(Disable_Panel));
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp) &&
            !GameManager.activePopUps.Contains(GameManager.Popups.FreeCoinsPopUp))
            GameManager.Inst.homeScreen.SetActiveEye(true);
    }
}