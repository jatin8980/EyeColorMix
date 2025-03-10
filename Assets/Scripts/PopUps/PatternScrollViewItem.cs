using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PatternScrollViewItem : MonoBehaviour
{
    internal float rotZ = 0;
    private Tween t;
    internal Image patternImage;
    internal RectTransform RT;

    internal void SetRotation(float targetZ)
    {
        t.Kill();
        t = DOTween.To(() => rotZ, (value) => SetRotZ(value), targetZ, 1f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            rotZ = targetZ;
        });
    }

    private void SetRotZ(float rot)
    {
        rotZ = rot;
        transform.localEulerAngles = new Vector3(0, 0, rot);
    }

    internal void DoFade(float targetValue, float duration,Ease ease)
    {
        if (targetValue > 0)
        {
            gameObject.SetActive(true);
        }
        patternImage.DOFade(targetValue, duration).SetEase(ease).OnComplete(() =>
        {
            if (targetValue == 0)
                gameObject.SetActive(false);
        });
    }
}
