using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UIManager uIManager;
    public FuelManager fuelManager;
    public SceneController sceneManager;
    public ScoreManager scoreManager;
    public LevelTargetSystem levelTargetSystem;
    public RainEffect rainEffect;
    public AudioSource wiperSound;
    bool isWipersOn;
    [HideInInspector] public bool isCheckatPlayerStart;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        GameMaster.instance.ShowInterstAd_AtRestart();
    }
    //Ashish Code 
    public void Wipers()
    {
        isWipersOn = !isWipersOn;
        if (isWipersOn)
        {
            //Wipers On play Music
            wiperSound.Play();
            CheckRain();
        }
        else
        {
            wiperSound.Stop();
        }
    }
    void CheckRain()
    {
        if (rainEffect.IsRaining)
        {
            NotificationContentView.instance.CreateNotification(StringConstants.wipersOn, 40, true, "");
        }
        else
        {
            NotificationContentView.instance.CreateNotification(StringConstants.wipersOn, 20, false, "");
        }
    }
    public void PlayButton()
    {
        fuelManager.PlayButton();
        fuelManager.StoreData();
    }

    public void GotoMainMenu()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        sceneManager.GotoMainMenu();
        SetScore(GetScore());
    }
    public void Reload()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        sceneManager.Reload();
        SetScore(GetScore());
        GameMaster.instance.restart_Counter++;
        PlayButton();
    }
    public void NextScene()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1;
        sceneManager.NextScene(1);
        SetScore(GetScore());
    }
    public void NextLevel()
    {
        sceneManager.Nextlevel();
        PlayButton();
    }
    void SetScore(int val)
    {
        int scoreSet = scoreManager.Score;
        val += scoreSet;
        if (scoreSet > 0) //Add if we gain the points
            PlayerPrefs.SetInt(StringConstants.score, val);
    }
    int GetScore()
    {
        return PlayerPrefs.GetInt(StringConstants.score, 0);
    }


    public void RewardedAdCoins(int currentCoinIndex)
    {
        AdsManager.instance.currentRewardedVideoIndex = currentCoinIndex;
        AdsManager.instance.rewardBtnType = RewardBtnType.Coins;
    }

}
