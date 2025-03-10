using TMPro;
using UnityEngine;

public class PermissionPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    private void Start()
    {
        messageText.text = messageText.text.Replace("#productName", Application.productName);
    }

    public void NotNow_Btn_Click() => GameManager.Inst.HidePopUp(gameObject);

    public void Settings_Btn_Click()
    {
        using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        var packageName = currentActivityObject.Call<string>("getPackageName");
        using var uriClass = new AndroidJavaClass("android.net.Uri");
        using var uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null);
        using var intentObject = new AndroidJavaObject("android.content.Intent",
            "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject);
        intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
        intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
        currentActivityObject.Call("startActivity", intentObject);
        NotNow_Btn_Click();
    }
}
