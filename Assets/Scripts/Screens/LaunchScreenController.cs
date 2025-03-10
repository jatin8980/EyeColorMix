using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LaunchScreenController : MonoBehaviour
{
    [SerializeField] private Slider launchSlider;
    [SerializeField] private CanvasGroup launchCanvas;
    private Tween launchTween;
    private bool isCompleted;

    private void OnEnable()
    {
        launchTween = launchSlider.DOValue(100, 7).SetEase(Ease.Linear).OnComplete(Complete);
    }

    internal void Complete()
    {
        launchTween?.Kill();
        GameManager.Inst.homeScreen.characterTR.gameObject.SetActive(true);
        GameManager.Inst.homeScreen.gameObject.SetActive(true);
        launchCanvas.DOFade(0, 0.5f).OnComplete(() =>
        {
            GameManager.Inst.Show_Screen(GameManager.Screens.Home);
            if (!isCompleted)
            {
                AdsManager.Inst.RequestNativeAd();
                SoundManager.Inst.Play("BackgroundMusic", true);
                isCompleted = true;
            }
        });
    }
}