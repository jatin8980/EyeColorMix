using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Toast : MonoBehaviour
{
    [SerializeField] private CanvasGroup toastCanvasGrp;
    private IEnumerator Start()
    {
        toastCanvasGrp.DOFade(1, 0.3f);
        yield return new WaitForSeconds(2f);
        toastCanvasGrp.DOFade(0, 0.3f).OnComplete(() => Destroy(gameObject));
    }
}