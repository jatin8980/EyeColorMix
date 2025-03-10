using UnityEngine;

public class MarkTRItem : MonoBehaviour
{
    internal int vertexIndex;
    internal Vector3 realPos;
    internal float lastDistance = 15;
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameController.Inst.stretchController.SnapToCurrentPos(this);
    }
}