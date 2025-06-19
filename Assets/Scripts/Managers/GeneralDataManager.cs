using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class GeneralDataManager : MonoBehaviour
{
    public GameObject noClickPanal;
    public bool testMode;
    internal static GeneralDataManager Inst;
    internal static string androidShareLink = "https://play.google.com/store/apps/details?id=",
                            iPhoneShareLink = "itms-apps://itunes.apple.com/app/id6739959573";
    internal int[] levelNeededToUnlockPupil = { 0, 0, 0, -1, 3, 3, -1, 5, -1, 5, -1, 7, 10, 13, -1, 17, 21, 26, 31, -1, 37, -1, 43, 50, 57, 64, -1, 71, -1, 78, 85, 92, 99, -1, 113, 127, 141, 155, -1, 162, 169, 176, 183, -1, 190, 197 },
        levelNeededToUnlockPattern = { 0, 0, 0, -1, 7, -1, -1, 10, 13, 17, -1, 26, -1, 37, 50, 64, 78, -1, 92, -1, 106, 120, 134, 148, -1 },
        levelNeededToUnlockEffect = { 0, 0, 0, 7, 13, 21, 31, 43, 50, 57, 64 },
        levelNeededToUnlockHighlight = { 0, 0, 0, 5, 5, 5, 10, 10, 17, 17, 21, 21, 26, 26, 31, 31, 37, 37, 43, 57, 71, 85, 99, 106, 113, 120, 127, 134, 141, 148, 155, 162, 169, 176, 183, 190, 197, 204, 204, 211, 211, 218, 218, 225, 225, 232, 232, 239, 239, 239 };

    internal List<int> orderToShowPupils = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45 },
        orderToShowPattern = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 },
        orderToShowEffects = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
        orderToShowHighlights = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };

    internal List<ColorDetail> colorDetails = new();
    internal string ShareMessage = "Unleash your creativity and color the world! 🎮In this mind-bending game, mix and match lenses of all colors to customize character’s eyes!" +
                      "\n\nAndroid : " + androidShareLink +
                     "\n\nIPhone : " + iPhoneShareLink;

    internal App_Detail app_Detail = new App_Detail();

    internal List<CharacterDetails> characterDetails = new() {
        new CharacterDetails {
            lensDragRightPos=new(-1.982f,-3.985f,0),
            lensDragLeftPos=new(1.749f,-3.985f,0),
            leftEyeCanvasPos=new(-0.487f,1.63f,0),
            rightEyeCanvasPos=new(0.552f,1.63f),
            bodyPos = new(-0.1f, -2.11f, 0),
            bodyScale=0.34f,
        },
        new CharacterDetails{
            lensDragRightPos=new(-1.561f,-3.091f,0),
            lensDragLeftPos=new(2.538f,-3.091f,0),
            leftEyeCanvasPos=new(-0.707f,1.381f,0),
            rightEyeCanvasPos=new(0.435f,1.381f,0),
            bodyPos = new(0, -2.65f, 0),
            bodyScale=0.4f,
        },
        new CharacterDetails{
            lensDragRightPos=new(-1.952f,-4.013f,0),
            lensDragLeftPos=new(2.251f,-4.013f,0),
            leftEyeCanvasPos=new(-0.627f,1.6379f,0),
            rightEyeCanvasPos=new(0.544f,1.6379f,0),
            bodyPos = new(0.05f, -1.46f, 0),
            bodyScale=0.3f,
        },
        new CharacterDetails{
            lensDragRightPos=new(-2,-4.01f,0),
            lensDragLeftPos=new(1.932f,-4.01f,0),
            leftEyeCanvasPos=new(-0.538f,1.637f,0),
            rightEyeCanvasPos=new(0.557f,1.637f,0),
            bodyPos = new(0,-2.505f, 0),
            bodyScale=0.4f,
        },
        new CharacterDetails{
            lensDragRightPos=new(-1.947f,-4.595f,0),
            lensDragLeftPos=new(1.813f,-4.595f,0),
            leftEyeCanvasPos=new(-0.505f,1.8f,0),
            rightEyeCanvasPos=new(0.5423f,1.8f,0),
            bodyPos = new(0, -2.11f, 0),
            bodyScale=0.34f,
        },
        new CharacterDetails {
            lensDragRightPos=new(-1.896f,-3.536f,0),
            lensDragLeftPos=new(1.781f,-3.536f,0),
            leftEyeCanvasPos=new(-0.4961f,1.505f,0),
            rightEyeCanvasPos=new(0.528f,1.505f),
            bodyPos = new(0f, -2.25f, 0),
            bodyScale=0.34f,
        },
        new CharacterDetails {
            lensDragRightPos=new(-2.078f,-3.644f,0),
            lensDragLeftPos=new(1.666f,-3.569f,0),
            leftEyeCanvasPos=new(-0.464f,1.514f,0),
            rightEyeCanvasPos=new(0.5789f,1.535f),
            bodyPos = new(0f, -1.39f, 0),
            bodyScale=0.3f,
        },
        new CharacterDetails {
            lensDragRightPos=new(-2.0155f,-4.362f,0),
            lensDragLeftPos=new(1.975f,-4.362f,0),
            leftEyeCanvasPos=new(-0.55f,1.735f,0),
            rightEyeCanvasPos=new(0.5614f,1.735f),
            bodyPos = new(0f,-2.77f, 0),
            bodyScale=0.38f,
        },
        new CharacterDetails {
            lensDragRightPos=new(-1.9595f,-5.245f,0),
            lensDragLeftPos=new(1.896f,-5.207f,0),
            leftEyeCanvasPos=new(-0.528f,1.9705f,0),
            rightEyeCanvasPos=new(0.546f,1.981f),
            bodyPos = new(-0.2f,-1.5f, 0),
            bodyScale=0.3f,
        },
         new CharacterDetails {
            lensDragRightPos=new(-1.8485f,-3.949f,0),
            lensDragLeftPos=new(1.8855f,-3.949f,0),
            leftEyeCanvasPos=new(-0.525f,1.62f,0),
            rightEyeCanvasPos=new(0.515f,1.62f),
            bodyPos = new(-0.085f,-1.345f, 0),
            bodyScale=0.3f,
        }
    };

    private void Awake()
    {
        Inst = this;
        if (!PlayerPrefs.HasKey(nameof(UnlockedPupils)))
        {
            UnlockedPupils = new() { };
            UnlockedPatterns = new() { };
            UnlockedImages = new() { new(), new(), new(), new(), new(), new(), new() };
            ListOfColorsUsedInLevel = new();
            ListOfCharacterPlayedInLevel = new();
            UnlockedPenIndexes = new() { 0 };
            UnlockedLensIndexes = new() { 0 };
            UnlockedClickParticleIndexes = new() { -1 };
            ShopPrices = new() { 200, 200, 200, 200, -1 };
            PlayerPrefs.SetString("LastRatePopupShowDate", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            DailyRewardClaimedDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            try
            {
                if (Directory.Exists(Application.persistentDataPath + "/.Eyes"))
                {
                    File.SetAttributes(Application.persistentDataPath + "/.Eyes", FileAttributes.Normal);
                    Directory.Delete(Application.persistentDataPath + "/.Eyes", true);
                }
                Directory.CreateDirectory(Application.persistentDataPath + "/.Eyes");
            }
            catch (Exception ex) { }
        }

        if (TutorialStep < 4)
            TutorialStep = 1;
        else if (TutorialStep == 4)
            TutorialStep = 5;
        else if (TutorialStep == 6)
            TutorialStep = 7;
        else if (TutorialStep == 8 || TutorialStep == 10)
            TutorialStep = 9;

        if (Resources.Load<TextAsset>("App_Details") != null)
            app_Detail = JsonConvert.DeserializeObject<App_Detail>(Resources.Load<TextAsset>("App_Details").text);
        if (app_Detail == null) app_Detail = new App_Detail();
        iPhoneShareLink = "itms-apps://itunes.apple.com/app/id" + app_Detail.iPhoneID;
        androidShareLink = "https://play.google.com/store/apps/details?id=" + app_Detail.AndroidID;

        Invoke(nameof(LoadColorNames), 3f);
    }

    private void LoadColorNames()
    {
        colorDetails = JsonConvert.DeserializeObject<List<ColorDetail>>(Resources.Load<TextAsset>("ColorNames").text);
    }

    internal static int SelectedPenIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedPenIndex), 0); }
        set { PlayerPrefs.SetInt(nameof(SelectedPenIndex), value); }
    }

    internal static int SelectedLensIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedLensIndex), 0); }
        set { PlayerPrefs.SetInt(nameof(SelectedLensIndex), value); }
    }

    internal static int Level
    {
        get { return PlayerPrefs.GetInt(nameof(Level), 1); }
        set { PlayerPrefs.SetInt(nameof(Level), value); }
    }

    internal static int Coins
    {
        get { return PlayerPrefs.GetInt(nameof(Coins), 200); }
        set { PlayerPrefs.SetInt(nameof(Coins), value); }
    }

    internal static int SelectedPupilIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedPupilIndex), 0); }
        set { PlayerPrefs.SetInt(nameof(SelectedPupilIndex), value); }
    }

    internal static int SelectedEffectIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedEffectIndex), -1); }
        set { PlayerPrefs.SetInt(nameof(SelectedEffectIndex), value); }
    }

    internal static int SelectedPatternIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedPatternIndex), -1); }
        set { PlayerPrefs.SetInt(nameof(SelectedPatternIndex), value); }
    }

    internal static int SelectedHighlightIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedHighlightIndex), -1); }
        set { PlayerPrefs.SetInt(nameof(SelectedHighlightIndex), value); }
    }

    internal static int SelectedClickParticleIndex
    {
        get { return PlayerPrefs.GetInt(nameof(SelectedClickParticleIndex), -1); }
        set { PlayerPrefs.SetInt(nameof(SelectedClickParticleIndex), value); }
    }

    /*internal static int AutoStretchPowerCount
    {
        get { return PlayerPrefs.GetInt(nameof(AutoStretchPowerCount), 3); }
        set { PlayerPrefs.SetInt(nameof(AutoStretchPowerCount), value); }
    }*/

    internal static int DailyRewardDayCount
    {
        get { return PlayerPrefs.GetInt(nameof(DailyRewardDayCount), 1); }
        set { PlayerPrefs.SetInt(nameof(DailyRewardDayCount), value); }
    }

    internal static int TutorialStep
    {
        get { return PlayerPrefs.GetInt(nameof(TutorialStep), 1); }
        set { PlayerPrefs.SetInt(nameof(TutorialStep), value); }
    }

    internal static List<int> ShopPrices
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(ShopPrices))); }
        set { PlayerPrefs.SetString(nameof(ShopPrices), JsonConvert.SerializeObject(value)); }
    }

    internal static List<int> UnlockedPupils//Only contains indexes of pupil which are unlocked by ad. 
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(UnlockedPupils))); }
        set { PlayerPrefs.SetString(nameof(UnlockedPupils), JsonConvert.SerializeObject(value)); }
    }

    internal static List<int> UnlockedPatterns//Only contains indexes of pattern which are unlocked by ad. 
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(UnlockedPatterns))); }
        set { PlayerPrefs.SetString(nameof(UnlockedPatterns), JsonConvert.SerializeObject(value)); }
    }

    internal static List<int> UnlockedPenIndexes
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(UnlockedPenIndexes))); }
        set { PlayerPrefs.SetString(nameof(UnlockedPenIndexes), JsonConvert.SerializeObject(value)); }
    }

    internal static List<int> UnlockedClickParticleIndexes
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(UnlockedClickParticleIndexes))); }
        set { PlayerPrefs.SetString(nameof(UnlockedClickParticleIndexes), JsonConvert.SerializeObject(value)); }
    }

    internal static List<int> UnlockedLensIndexes
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(UnlockedLensIndexes))); }
        set { PlayerPrefs.SetString(nameof(UnlockedLensIndexes), JsonConvert.SerializeObject(value)); }
    }

    internal static List<List<int>> UnlockedImages
    {
        get { return JsonConvert.DeserializeObject<List<List<int>>>(PlayerPrefs.GetString(nameof(UnlockedImages))); }
        set { PlayerPrefs.SetString(nameof(UnlockedImages), JsonConvert.SerializeObject(value)); }
    }

    internal static List<List<Color32>> ListOfColorsUsedInLevel
    {
        get { return JsonConvert.DeserializeObject<List<List<Color32>>>(PlayerPrefs.GetString(nameof(ListOfColorsUsedInLevel))); }
        set { PlayerPrefs.SetString(nameof(ListOfColorsUsedInLevel), JsonConvert.SerializeObject(value)); }
    }

    internal static List<int> ListOfCharacterPlayedInLevel
    {
        get { return JsonConvert.DeserializeObject<List<int>>(PlayerPrefs.GetString(nameof(ListOfCharacterPlayedInLevel))); }
        set { PlayerPrefs.SetString(nameof(ListOfCharacterPlayedInLevel), JsonConvert.SerializeObject(value)); }
    }

    internal static bool IsPurchaseAdsRemoved
    {
        get => PlayerPrefs.GetInt(nameof(IsPurchaseAdsRemoved), 0) == 1;
        set => PlayerPrefs.SetInt(nameof(IsPurchaseAdsRemoved), value ? 1 : 0);
    }

    internal static bool IsPatternTutorialShowed
    {
        get => PlayerPrefs.GetInt(nameof(IsPatternTutorialShowed), 0) == 1;
        set => PlayerPrefs.SetInt(nameof(IsPatternTutorialShowed), value ? 1 : 0);
    }

    internal static bool IsChoiceSave
    {
        get => PlayerPrefs.GetInt(nameof(IsChoiceSave), 1) == 1;
        set => PlayerPrefs.SetInt(nameof(IsChoiceSave), value ? 1 : 0);
    }

    internal static bool UserAlreadyGiveRating
    {
        get => PlayerPrefs.GetInt(nameof(UserAlreadyGiveRating), 0) == 1;
        set => PlayerPrefs.SetInt(nameof(UserAlreadyGiveRating), value ? 1 : 0);
    }

    internal static string Shop_Saved_Data
    {
        get { return PlayerPrefs.GetString(nameof(Shop_Saved_Data), ""); }
        set { PlayerPrefs.SetString(nameof(Shop_Saved_Data), value); }
    }

    internal static string DailyRewardClaimedDate
    {
        get { return PlayerPrefs.GetString(nameof(DailyRewardClaimedDate)); }
        set { PlayerPrefs.SetString(nameof(DailyRewardClaimedDate), value); }
    }

    internal void No_Click_Panel_On_Off(bool isOn = false)
    {
        noClickPanal.SetActive(isOn);
    }

    internal void No_Click_Panel_Off()
    {
        No_Click_Panel_On_Off(false);
    }

    internal bool IsNoClickPanelOn()
    {
        return noClickPanal.activeSelf;
    }

    public class App_Detail
    {
        public string Privacy = "";
        public string Mail = "";
        public string iPhoneID = "";
        public string AndroidID = "";
    }
}

public class CharacterDetails
{
    public Vector3 lensDragRightPos, lensDragLeftPos;
    public Vector3 leftEyeCanvasPos, rightEyeCanvasPos;
    public Vector3 bodyPos;
    public float bodyScale;
}

public class ColorDetail
{
    public string subtitle;
    public string hex;
}