using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    internal static GameController Inst;
    public List<SpriteRenderer> beforeSRs;
    public StretchController stretchController;
    internal int currentCircleIndex = 0, collidedCount, totalCollidersToCount;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        stretchController.gameObject.SetActive(true);
    }

    internal void BeforeSpriteSetActive(bool show)
    {
        beforeSRs[0].transform.parent.parent.gameObject.SetActive(show);
    }
}
