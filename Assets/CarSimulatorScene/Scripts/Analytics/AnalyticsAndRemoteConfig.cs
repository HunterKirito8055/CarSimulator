//using Firebase;
//using Firebase.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;


//public class AnalyticsAndRemoteConfig : MonoBehaviour
//{
//    public static AnalyticsAndRemoteConfig instance;
//    private Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
//    private bool isRemoteInitialized = false;
//    private static bool isAnalyticsInitialized = false;
//    private FirebaseApp app;

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else if (instance != this)
//        {
//            Destroy(gameObject);
//        }

//        return;
//    }

//    private void Start()
//    {
//#if UNITY_ANDROID
//        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//        {
//            DependencyStatus dependencyStatus = task.Result;
//            if (dependencyStatus == Firebase.DependencyStatus.Available)
//            {
//                // Create and hold a reference to your FirebaseApp,
//                // where app is a Firebase.FirebaseApp property of your application class.
//                app = Firebase.FirebaseApp.DefaultInstance;
//                InitializeFirebaseComponents();

//                // Set a flag here to indicate whether Firebase is ready to use by your app.
//            }
//            else
//            {
//                InitializeFirebaseComponents();
//                UnityEngine.Debug.LogError(string.Format(
//                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                // Firebase Unity SDK is not safe to use here.
//            }
//        });
//#else
//        InitializeFirebaseComponents();
//#endif
//        DontDestroyOnLoad(gameObject);
//    }

//    private void InitializeFirebaseComponents()
//    {
//        // Crashlytics will use the DefaultInstance, as well;
//        // this ensures that Crashlytics is intitialized.
//        //Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

//        // this ensures that Remote data is intitialized.
//        System.Threading.Tasks.Task.WhenAll(
//            InitializeRemoteConfig()
//          ).ContinueWith(task => { GetRemoteData(); });

//        // this ensures that Analytics data is intitialized.
//        InitializeAnalytics();

//    }

//    private void InitializeAnalytics()
//    {
//       // Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
//    }

//    // Initialize remote config, and set the default values.


//    // [START set_defaults]
//    System.Threading.Tasks.Task InitializeRemoteConfig()
//    {
//        Dictionary<string, object> defaults = new Dictionary<string, object>();
//        // These are the values that are used if we haven't fetched data from the
//        // server
//        // yet, or if we ask for values that the server doesn't have:
//        //------------------------------------------------------------------------------------------------------------------

//        //global

//        defaults.Add(UnityRemoteData.Inters_Ads, true);
//        defaults.Add(UnityRemoteData.Full_Ads, true);
//        defaults.Add(UnityRemoteData.Ad_Banner, true);
//        defaults.Add(UnityRemoteData.Ad_Win_available, true);
//        defaults.Add(UnityRemoteData.Ad_Fail_available, true);
//        defaults.Add(UnityRemoteData.Ad_AtPauseExit_available, true);

//        //InterstitialAD Placements

//        defaults.Add(UnityRemoteData.Ad_WelcomePlayButton, true);
//        defaults.Add(UnityRemoteData.Ad_MaptoHomeButton, true);
//        defaults.Add(UnityRemoteData.Ad_MaptoGameScreenButton, true);
//        defaults.Add(UnityRemoteData.Ad_Gameplayto_MapOrRestart, true);
//        defaults.Add(UnityRemoteData.Ad_GameWinto_NextOrMap, true);
//        defaults.Add(UnityRemoteData.Ad_GameLoseto_RestartOrMap, true);

//        defaults.Add(UnityRemoteData.Ad_StartLevel, 3);
//        defaults.Add(UnityRemoteData.Ad_Win_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_fail_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_PauseExitBtnCounter, 2);

//        // InterstitialAD Counters
//        defaults.Add(UnityRemoteData.Ad_WelcomePlayButton_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_MaptoHomeButton_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_MaptoGameScreenButton_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_Gameplayto_MapOrRestart_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_GameWinto_NextOrMap_Counter, 2);
//        defaults.Add(UnityRemoteData.Ad_GameLoseto_RestartOrMap_Counter, 2);



//        //    //--------------------------------------------------------------------------------------------------------------------------

//       // Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
//       //.ContinueWithOnMainThread(task =>
//       //{
//       //    // [END set_defaults]

//       //    isRemoteInitialized = true;

//       //});

//       // if (isRemoteInitialized)
//       // {
//       //     FetchDataAsync();
//       // }
//       // return FetchDataAsync();
//    }



//    //Start a fetch request.
//    //public Task FetchDataAsync()
//    //{
//    //    Debug.Log("Fetching Remote Data ");
//    //    System.Threading.Tasks.Task fetchTask =
//    //    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero);
//    //    return fetchTask.ContinueWithOnMainThread(FetchComplete);
//    //}

//    private void FetchComplete(Task fetchTask)
//    {
//        if (fetchTask.IsCanceled)
//        {
//            Debug.Log("Fetch canceled.");
//        }
//        else if (fetchTask.IsFaulted)
//        {
//            Debug.Log("Fetch encountered an error.");
//        }
//        else if (fetchTask.IsCompleted)
//        {
//            Debug.Log("Fetch completed successfully!");

//        }

//        //var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
//        //switch (info.LastFetchStatus)
//        //{
//        //    case Firebase.RemoteConfig.LastFetchStatus.Success:
//        //        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
//        //        .ContinueWithOnMainThread(task =>
//        //        {
//        //            Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
//        //                           info.FetchTime));
//        //            GetRemoteData();
//        //        });


//        //        break;
//        //    case Firebase.RemoteConfig.LastFetchStatus.Failure:
//        //        switch (info.LastFetchFailureReason)
//        //        {
//        //            case Firebase.RemoteConfig.FetchFailureReason.Error:
//        //                Debug.Log("Fetch failed for unknown reason");
//        //                break;
//        //            case Firebase.RemoteConfig.FetchFailureReason.Throttled:
//        //                Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
//        //                break;
//        //        }
//        //        GetRemoteData();
//        //        break;
//        //    case Firebase.RemoteConfig.LastFetchStatus.Pending:
//        //        Debug.Log("Latest Fetch call still pending.");
//        //        GetRemoteData();
//        //        break;

//        //    default:
//        //        GetRemoteData();
//        //        break;
//        //}
//    }

   // private void GetRemoteData()
   // {
        //UnityRemoteData.isFullAdsEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Full_Ads).BooleanValue;
        //UnityRemoteData.isIntAdsEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Inters_Ads).BooleanValue;
        //UnityRemoteData.isBannerAdEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_Banner).BooleanValue;
        //UnityRemoteData.isAds_WinEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_Win_available).BooleanValue;
        //UnityRemoteData.isAds_FailEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_Fail_available).BooleanValue;
        //UnityRemoteData.isAds_PauseExitBtnEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_AtPauseExit_available).BooleanValue;

        ////Interstial Ads Enabled
        //UnityRemoteData.isWelcome_PlayButtonEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_WelcomePlayButton).BooleanValue;
        //UnityRemoteData.isMaptoHomeEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_MaptoHomeButton).BooleanValue;
        //UnityRemoteData.isMaptoGameScreenEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_MaptoGameScreenButton).BooleanValue;
        //UnityRemoteData.isGameplaytoMapOrRestartEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_Gameplayto_MapOrRestart).BooleanValue;
        //UnityRemoteData.isGameWintoNextOrMapEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_GameWinto_NextOrMap).BooleanValue;
        //UnityRemoteData.isGameLosetoRestartOrMapEnabled = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_GameLoseto_RestartOrMap).BooleanValue;


        //UnityRemoteData.adsWinCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_Win_Counter).DoubleValue;
        //UnityRemoteData.adsFailCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_fail_Counter).DoubleValue;
        //UnityRemoteData.adsStartlevel = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_StartLevel).DoubleValue;
        //UnityRemoteData.adsPauseExitBtnCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_PauseExitBtnCounter).DoubleValue;

        ////Interstial Ads Counters
        //UnityRemoteData.welcome_PlayButtonCounter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_WelcomePlayButton_Counter).DoubleValue;
        //UnityRemoteData.maptoHome_Counter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_MaptoHomeButton_Counter).DoubleValue;
        //UnityRemoteData.maptoGameScreen_Counter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_MaptoGameScreenButton_Counter).DoubleValue;
        //UnityRemoteData.gameplaytoMapOrRestart_Counter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_Gameplayto_MapOrRestart_Counter).DoubleValue;
        //UnityRemoteData.gameWintoNextOrMap_Counter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_GameWinto_NextOrMap_Counter).DoubleValue;
        //UnityRemoteData.gameLosetoRestartOrMap_Counter = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(UnityRemoteData.Ad_GameLoseto_RestartOrMap_Counter).DoubleValue;

        //string Wheelreward = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("wheelRewardData").StringValue;
        //if (Wheelreward != null)
        //    UnityRemoteData.wheelRewardData = Wheelreward;

        //string dailyreward = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("DailyRewardData").StringValue;
        //if (dailyreward != null)
        //    UnityRemoteData.dailyRewardData = dailyreward;

        //Debug.Log("Remote Data:\n" +
        //    "isFullAdsEnabled - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Full_Ads).BooleanValue + " \n" +
        //            "isIntAdsEnabled - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Inters_Ads).BooleanValue + " \n" +
        //            "isBannerAdEnabled - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Banner).BooleanValue + " \n" +
        //            "isAds_WinEnabled - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Win_available).BooleanValue + " \n" +
        //            "isAds_FailEnabled - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Inters_Ads).BooleanValue.ToString() + " \n" +
        //            "isAds_PauseExitBtnEnabled - " + Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Fail_available).BooleanValue + " \n" +
        //            "adsWinCounter - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_Win_Counter).DoubleValue + " \n" +
        //            "adsFailCounter - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_fail_Counter).DoubleValue + " \n" +
        //            "adsStartlevel - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_StartLevel).DoubleValue + " \n" +
        //            "adsPauseExitBtnCounter - " + (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(UnityRemoteData.Ad_PauseExitBtnCounter).DoubleValue;

//         );


//    }


//    #region Analytics Calls

//    //PopUps and Screen Data 
//    public static void TrackData(string menu)
//    {
//        //if(isAnalyticsInitialized)
//        //FirebaseAnalytics.LogEvent("Open_Screen","Screen_Name", menu);
//        Debug.Log("Analytics: Screen_Name = " + menu);
//    }

//    public static void SetUserPropertyForUnlockedLevel(int total_playedLevels)
//    {
//        //FirebaseAnalytics.SetUserProperty("Total_Played_Levels", total_playedLevels.ToString());

//        //if (total_playedLevels >= 10)
//        //	Completed_10_Level();

//        Debug.Log("Analytics: Current Unlocked Level =" + total_playedLevels);
//    }


//    //public static void LevelStart(string difficulty,int levelNum, int coins, int hints)
//    //{
//    //    //FirebaseAnalytics.LogEvent(
//    //    //	FirebaseAnalytics.EventLevelStart,
//    //    //	new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum));

//    //    //FirebaseAnalytics.SetUserProperty("GameLevel_Progress", "Start");

//    //    //FirebaseAnalytics.SetUserProperty("Current_Level", PlayerPrefs.GetInt("CurrentUnlockedLevel").ToString());
//    //      Debug.Log("Analytics: Level Start id ="+levelNum+"   Current Level: "+(GameManager.Instance.TotalLevelsDone+1));
//    //}
//    public static void LevelStart(int levelNum)
//    {
//        //FirebaseAnalytics.LogEvent(
//        //	FirebaseAnalytics.EventLevelStart,
//        //	new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum));

//        //FirebaseAnalytics.SetUserProperty("GameLevel_Progress", "Start");

//        //    FirebaseAnalytics.SetUserProperty("Current_Level", PlayerPrefs.GetInt("CurrentUnlockedLevel").ToString());
//        Debug.Log("Analytics: Level Start no. " + levelNum);
//    }

//    //public static void LevelWin(string difficulty, int level, int coins ,int hints) //Level Completed
//    //{
//    //    //FirebaseAnalytics.LogEvent(
//    //    //	FirebaseAnalytics.EventLevelEnd,
//    //    //	new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.currLevelName),
//    //    //	new Parameter(FirebaseAnalytics.ParameterScore, Scorevalue));

//    //    //PlayedDifficulty(GameManager.Instance.currLevelName);
//    //    //FirebaseAnalytics.SetUserProperty("GameLevel_Progress", "Completed");
//    //    //Debug.Log("Analytics: Level End id =" + GameManager.Instance.ActivePuzzleData.puzzleName + "   Current Level: " + (GameManager.Instance.TotalLevelsDone + 1)+"   Score Value : "+Scorevalue);
//    //}
//    public static void LevelWin(int _levelNum) //Level Completed
//    {
//        //FirebaseAnalytics.LogEvent(
//        //	FirebaseAnalytics.EventLevelEnd,
//        //	new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.currLevelName),
//        //	new Parameter(FirebaseAnalytics.ParameterScore, Scorevalue));

//        //PlayedDifficulty(GameManager.Instance.currLevelName);
//        //FirebaseAnalytics.SetUserProperty("GameLevel_Progress", "Completed");
//        Debug.Log("Analytics: Current Level Win : " + _levelNum);
//    }

//    public static void LevelEndFail(int _levelNum)
//    {
//        //if (!String.IsNullOrEmpty(GameManager.Instance.ActivePuzzleData.puzzleName))
//        //{
//        //	FirebaseAnalytics.LogEvent(
//        //		"level_failed",
//        //		new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.currLevelName));

//        //	//Debug.Log("Analytics: Level Failed =" + GameManager.Instance.ActivePuzzleData.puzzleName + "   Current Level:" + (GameManager.Instance.TotalLevelsDone + 1));
//        //}
//        Debug.Log("Analytics: Current Level Fail : " + _levelNum);
//    }

//    //public static void DayRewardBonus(int dayValue)
//    //{
//    //    FirebaseAnalytics.LogEvent("Daily_Bonus", "Claimed_Day", dayValue);
//    //}

//    public static void ButtonTracks(string menu, string buttonName)
//    {
//        //FirebaseAnalytics.LogEvent("Button_" + menu + "-" + buttonName);
//        Debug.Log("Analytics: Button - " + menu + "-" + buttonName);
//    }

//    public static void AdFrequency(int value) //This is Intersticial Ad frequency
//    {
//        //FirebaseAnalytics.SetUserProperty("Ad_Frequency", value.ToString());
//        //Debug.Log("Analytics: Ad Frequency =" + value);
//    }

//    public static void AdAnalytics(string id)
//    {
//        //FirebaseAnalytics.LogEvent(
//        //		id,
//        //		new Parameter(FirebaseAnalytics.ParameterLevelName, GameManager.Instance.TotalLevelsDone));

//    }


//    public static void PurchasedCount(int count, float totalPrice)
//    {
//        //print("PurchaseCount " + "-" + count);
//        //FirebaseAnalytics.SetUserProperty("Purchase_Count", count.ToString());
//        //FirebaseAnalytics.SetUserProperty("IAP_Spend_Value", totalPrice.ToString());

//        //FirebaseAnalytics.LogEvent("Purchase_Data",
//        //	new Parameter("Purchased_Count", count),
//        //	new Parameter("IAP_Spend_Value", totalPrice));
//    }

//    public static void PurchasedFailedCount(int count)
//    {

//        //FirebaseAnalytics.SetUserProperty("Purchase_Fail_Count", count.ToString());


//    }

//    public static void PlayedDifficulty(string type)
//    {
//        //string currDiff = "";

//        //if (type.Contains("easy"))
//        //	currDiff = "easy";
//        //else if (type.Contains("medium"))
//        //	currDiff = "medium";
//        //else if (type.Contains("hard"))
//        //	currDiff = "hard";
//        //else if (type.Contains("expert"))
//        //	currDiff = "expert";



//        //PlayerPrefs.SetInt("PLAYED_LEVEL_" + currDiff, PlayerPrefs.GetInt("PLAYED_LEVEL_" + currDiff, 0) + 1);
//        //FirebaseAnalytics.SetUserProperty("Played_Game_Mode_" + currDiff, PlayerPrefs.GetInt("PLAYED_LEVEL_" + currDiff).ToString());
//    }

//    public static void Completed_5_days(int totalLevels)
//    {
//        //FirebaseAnalytics.LogEvent("Played_5_Days",
//        //	new Parameter(FirebaseAnalytics.ParameterLevelName, "Levels_" + totalLevels.ToString()),
//        //	new Parameter(FirebaseAnalytics.ParameterLevel, totalLevels));

//        Debug.Log("Analytics: Player For 5 days and unlocked = " + totalLevels + " levels ");
//    }

//    public static void TotalPlayedDays(int days)//Updated new
//    {
//        //FirebaseAnalytics.SetUserProperty("Total_played_days", days.ToString());
//        Debug.Log("Analytics: Total Played Days = " + days);

//    }


//    public static void ConsistentDays(int dayValue)//Updated new
//    {
//        //FirebaseAnalytics.SetUserProperty("Consistent_days", dayValue.ToString());

//        Debug.Log("Consistent_days--> " + dayValue);
//    }

//    public static void Completed_10_Level()
//    {
//        //FirebaseAnalytics.LogEvent("Completed_10_Levels",
//        //new Parameter(FirebaseAnalytics.ParameterLevelName, "Completed"));
//        Debug.Log("Completed 10th Level-");
//    }
//    public static void SpendVirtualCurrency(string placement, string type, int quantity)
//    {
//        //float priceValue = value * 0.099f;
//        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSpendVirtualCurrency,
//        //new Parameter(FirebaseAnalytics.ParameterItemName, item_Name),
//        //new Parameter(FirebaseAnalytics.ParameterQuantity, quantity),
//        //new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, Currencytype),
//        //new Parameter(FirebaseAnalytics.ParameterValue, priceValue));
//        // new Parameter(FirebaseAnalytics.ParameterPrice, priceValue),
//        // new Parameter(FirebaseAnalytics.ParameterCurrency, "USD")

//        Debug.Log("Analytics: SpendVirtualCurrency placement = " + placement + " type = " + type + " Quantity = " + quantity);
//    }

//    public static void EarnVirtualCurrency(string item_Name, int quantity)
//    {
//        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency,
//        //   new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "Coins"),
//        //   new Parameter(FirebaseAnalytics.ParameterItemName, item_Name),
//        //   new Parameter(FirebaseAnalytics.ParameterValue, quantity));

//        Debug.Log("Analytics: EarnVirtualCurrency Placement = " + item_Name + " Earned = " + quantity);
//    }




//    public static void TutorialBegin(int levelNum)//newUpdate
//    {
//        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin,
//        //new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));
//        Debug.Log("Tutorial Begined at : " + levelNum.ToString());
//    }
//    //Not integrated in Sudoku yet
//    public static void TutorialComplete(int levelNum)//newUpdate
//    {
//        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete,
//        //new Parameter(FirebaseAnalytics.ParameterLevel, levelNum));
//        Debug.Log("Tutorial ended at : " + levelNum.ToString());
//    }


//    public static void LevelAdRewardedAds(string levelNum)
//    {
//        //FirebaseAnalytics.LogEvent("LevelAdRewardedAds",
//        //	new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum.ToString()),
//        //	new Parameter(FirebaseAnalytics.ParameterLevel, GameManager.Instance.TotalLevelsDone));
//    }

//    public static void LevelAdImpressions(string levelNum)
//    {
//        //FirebaseAnalytics.LogEvent("LevelAdImpressions",
//        //	new Parameter(FirebaseAnalytics.ParameterLevelName, levelNum.ToString()),
//        //	new Parameter(FirebaseAnalytics.ParameterLevel, GameManager.Instance.TotalLevelsDone));
//    }




//    #endregion
//}