using UnityEngine;

public class ExitPopUp : MonoBehaviour
{
    public void On_No_Btn_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }

    public void On_Yes_Btn_Click()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
