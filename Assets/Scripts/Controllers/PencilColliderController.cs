using UnityEngine;

public class PencilColliderController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameController.Inst.collidedCount++;
        collision.gameObject.SetActive(false);
        GameManager.Inst.gamePlayUi.drawController.Vibrate();
    }
}
