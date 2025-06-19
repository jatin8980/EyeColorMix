using UnityEngine;

public class InterstitialAdLoader : MonoBehaviour
{
    [SerializeField] private float loadTime = 5f;
    private void OnEnable()
    {
        Invoke(nameof(AutoDisable), loadTime);
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(false);
    }

    private void AutoDisable() => gameObject.SetActive(false);

    private void OnDisable()
    {
        if (AdsManager.IsQuittingGame) return;
        AdsManager.Inst.OnLoaderDisable();
        CancelInvoke(nameof(AutoDisable));
        AdsManager.Inst.CanShowAppOpen = true;
        if (GameManager.activePopUps.Contains(GameManager.Popups.LevelCompletePopUp) &&
            !GameManager.activePopUps.Contains(GameManager.Popups.StorePopUp))
            GameManager.Inst.homeScreen.SetActiveEye(true);
    }
}