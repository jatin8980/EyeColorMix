using UnityEngine;
using static SoundManager;
using UnityEngine.Networking;

public class SettingPopUp : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject saveChoiceTickOb, musicOffLine, soundOffLine, vibrateOffLine;

    #endregion

    #region Start & Awake

    private void Awake()
    {
        saveChoiceTickOb.SetActive(GeneralDataManager.IsChoiceSave);
        musicOffLine.SetActive(!IsMusicOn);
        soundOffLine.SetActive(!IsSoundOn);
        vibrateOffLine.SetActive(!IsVibrationOn);
    }

    #endregion

    #region Button Methods

    public void On_Sound_Btn_Click()
    {
        IsSoundOn = !IsSoundOn;
        soundOffLine.SetActive(!IsSoundOn);
        if (!IsSoundOn) SoundManager.Inst.Stop(SoundType.SoundEffect);
    }

    public void On_Music_Btn_Click()
    {
        IsMusicOn = !IsMusicOn;
        musicOffLine.SetActive(!IsMusicOn);
        if (IsMusicOn) SoundManager.Inst.Play("BackgroundMusic", true);
        else SoundManager.Inst.Stop("BackgroundMusic");
    }

    public void On_Vibration_Btn_Click()
    {
        IsVibrationOn = !IsVibrationOn;
        vibrateOffLine.SetActive(!IsVibrationOn);
    }

    public void On_Rate_Button_Click()
    {
        GameManager.Inst.On_Rate_Btn_Click();
    }

    public void On_Share_Button_Click()
    {
        Share_Game();
    }

    public void On_Contact_Button_Click()
    {
        SendEmail();
    }

    public void On_Privacy_Button_Click()
    {
        Application.OpenURL(Application.platform == RuntimePlatform.Android ?
                   GeneralDataManager.Inst.app_Detail.Privacy :
                   "https://sites.google.com/view/easyfungames/policy");
    }

    public void On_cancel_Button_Click()
    {
        GameManager.Inst.HidePopUp(gameObject);
    }

    public void On_CheckBox_Btn_Click()
    {
        GeneralDataManager.IsChoiceSave = !GeneralDataManager.IsChoiceSave;
        saveChoiceTickOb.SetActive(GeneralDataManager.IsChoiceSave);
    }
    #endregion

    #region  Functions
    private static void SendEmail()
    {
        string email = Application.platform == RuntimePlatform.Android ? GeneralDataManager.Inst.app_Detail.Mail : "morefun.connect@gmail.com";
        var subject = MyEscapeURL(Application.productName + " " + Application.platform + " Support V" + Application.version);
        var body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private static string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    private static void Share_Game()
    {
        AdsManager.Inst.CanShowAppOpen = false;
#if UNITY_ANDROID
        var intentClass = new AndroidJavaClass("android.content.Intent");
        var intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), GeneralDataManager.Inst.ShareMessage);

        var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        var chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share your high score");
        currentActivity.Call("startActivity", chooser);
#elif UNITY_IOS
        new NativeShareLink().SetText(GeneralDataManager.Inst.ShareMessage).Share();
#endif
    }
    #endregion
}