using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    public int selected_LevelNumber;
    public int selected_ChapterId;
    public int freeRide_Chapter;
    public string selected_vehicle;


    //Interstitial Counter Vals
    [HideInInspector] public int win_Counter = 0;
    [HideInInspector] public int fail_Counter = 0;
    [HideInInspector] public int pauseExit_Counter = 0;
    [HideInInspector] public int restart_Counter = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void ShowInterstAd_AtRestart()
    {
        if (UnityRemoteData.isAds_RestartEnabled && (restart_Counter >= UnityRemoteData.restartCounter))
        {
            restart_Counter = 0;
            AdsManager.instance.ShowInterstitial();
        }
    }
    public void ShowInterstAd_AtWin()
    {
        win_Counter++;
        if (UnityRemoteData.isAds_PauseEnabled && (win_Counter >= UnityRemoteData.winCounter))
        {
            win_Counter = 0;
            AdsManager.instance.ShowInterstitial();
        }
    }
    public void ShowInterstAd_AtFail()
    {
        fail_Counter++;
        if (UnityRemoteData.isAds_FailEnabled && (fail_Counter >= UnityRemoteData.failCounter))
        {
            fail_Counter = 0;
            AdsManager.instance.ShowInterstitial();
        }
    }
    public void ShowInterstAd_AtPauseExit()
    {
        if (UnityRemoteData.isAds_PauseEnabled && (pauseExit_Counter >= UnityRemoteData.pauseBtnCounter))
        {
            pauseExit_Counter = 0;
            AdsManager.instance.ShowInterstitial();
        }
    }
}

