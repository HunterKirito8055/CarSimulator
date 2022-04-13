using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine;
using RS = UnityEngine.RemoteSettings;

public class UnityRemoteData : MonoBehaviour
{
    // public static bool vaulesUpdated = false;
    //This are keys for remote settings

    public static string Full_Ads = "Full_Ads";
    public static string Inters_Ads = "Ad_Interstitial";
    public static string Ad_Banner = "Ad_Banner";

    //RewardInterstitial_ADS
    //public static string RewardInterstitial_Ads = "RewardInterstitial_ADS";

    public static bool isIntAdsEnabled = true;
    public static bool isBannerAdEnabled = true;
    public static bool isFullAdsEnabled = true;
    public static bool isRewardInitAdEnabled = true;

    //=====================================================//

    //InterstitialAD Placements
    public static string Ad_Win_available = "Ads_At_LevelWin";
    public static string Ad_Fail_available = "Ads_At_LevelFail";
    public static string Ad_AtPause_available = "Ads_At_PauseExit";
    public static string Ad_Restart = "Ads_At_Restart";

    //InterstitialAD Placements Counters
    public static string Ad_Win_Counter = "Ad_LevelWin_Counter";
    public static string Ad_Fail_Counter = "Ad_LevelFail_Counter";
    public static string Ad_Pause_Counter = "Ad_PauseExitBtn_Counter";
    public static string Ad_RestartButton_Counter = "Ad_RestartButton_Counter";

    //Interstitial AD active
    public static bool isAds_WinEnabled = true;
    public static bool isAds_FailEnabled = true;
    public static bool isAds_PauseEnabled = true;
    public static bool isAds_RestartEnabled = true;

    //Interstitial AD Counters
    public static int winCounter = 1;
    public static int failCounter = 1;
    public static int pauseBtnCounter = 1;
    public static int restartCounter = 1;
}