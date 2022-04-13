using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
//Ashish Code
public class FuelManager : MonoBehaviour
{
    int timeToWaitinMin = 10;
    int timeToWaitinSeconds = 0;
    int fuels;
    long totalSecs;
    DateTime futureDateTime;
    TimeSpan futureTime;
    TimeSpan remainTime;
    string futuresaved;
    string pastSaved;
    string panelFuel;

    //Unlimited Fuels 
    DateTime unlimitedDT;
    TimeSpan unlimitedTime;
    bool isUnlimitedFuels;
    string unlimitedsaved;
    //Unlimited Fuels 
    #region Properties
    public string UnlimitedSaveTime
    {
        get
        {
            return PlayerPrefs.GetString(StringConstants.unlimitedTime);
        }
        set
        {
            string s = value;
            PlayerPrefs.SetString(StringConstants.unlimitedTime, s);
        }
    }

    public string PopFuel
    {
        get
        {
            return panelFuel = PlayerPrefs.GetString(StringConstants.fuelTime, "10:00");
        }
        set
        {
            if (GameManager.instance.uIManager.panelFuelText != null)
                GameManager.instance.uIManager.panelFuelText.text = value;
            panelFuel = value;
            PlayerPrefs.SetString(StringConstants.fuelTime, panelFuel);
        }
    }
    public string FutureSaveTime
    {
        get
        {
            return PlayerPrefs.GetString(StringConstants.futureTime);
        }
        set
        {
            string s = value;
            futuresaved = value;
            PlayerPrefs.SetString(StringConstants.futureTime, s);
        }
    }
    public string PastSavedTime
    {
        get
        {
            return PlayerPrefs.GetString(StringConstants.pastTime);
        }
        set
        {
            string s = value;
            PlayerPrefs.SetString(StringConstants.pastTime, s);
        }
    }
    public int Fuels
    {
        get
        {
            return fuels = PlayerPrefs.GetInt(StringConstants.fuelCount, 10);
        }
        set
        {
            fuels = value;
            if (fuels >= 10)
            {
                fuels = 10;
                ResetTimer();
                GameManager.instance.uIManager.IsFullFuel(true);
            }
            else if (fuels < 10)
            {
                GameManager.instance.uIManager.IsFullFuel(false);
            }
            PlayerPrefs.SetInt(StringConstants.fuelCount, fuels);
            if (GameManager.instance.uIManager.topFuelText != null)
                GameManager.instance.uIManager.topFuelText.text = fuels.ToString() + "/10";
        }
    }

    #endregion


    void Start()
    {
        futuresaved = FutureSaveTime;
        pastSaved = PastSavedTime;
        int _val = 0;
        if (futuresaved != "")
        {
            futureTime = DateTime.Parse(futuresaved) - DateTime.Now;
            futureDateTime = (DateTime.Now.Add(futureTime));

            TimeSpan pastTime = DateTime.Now - DateTime.Parse(pastSaved);
            remainTime = futureDateTime - DateTime.Now;
            //if time difference is greater than 10 
            if (remainTime.TotalSeconds < 0)
            {
                _val = (int)(pastTime.TotalSeconds / 600.00d);
                if (_val > 0)
                {
                    futureDateTime = futureDateTime.AddMinutes(_val * 10); //Adding more time if we comeback after 10mins, if still fuels are not full
                    PastSavedTime = DateTime.Now.ToString();
                }
            }
        }
        else
        {
            ResetTimer();
        }
        Fuels += _val;
        if (GameManager.instance.uIManager.topFuelText != null)
            GameManager.instance.uIManager.topFuelText.text = Fuels.ToString() + "/10";

        //LocalNotifications
        if (Fuels < 10)
        {
            int fuel = Fuels;
            int calculateRemainingFuelsLeft = 10 * (9 - fuel); // 10 * (9 - 2) => 10 * 7 => 1 hr 10 mins
            TimeSpan notificationTimeSpan = new TimeSpan(0, 0, calculateRemainingFuelsLeft + remainTime.Minutes, remainTime.Seconds);
            LocalNotifications.localNotificationsInstance.SendNotification(notificationTimeSpan);
        }
    }
    private void ResetTimer()
    {
        futuresaved = "";
        pastSaved = "";
        futureTime = new TimeSpan(0, timeToWaitinMin, timeToWaitinSeconds);
        futureDateTime = DateTime.Now.Add(futureTime);
        PastSavedTime = DateTime.Now.ToString();
    }

    void Update()
    {
        //Check UnlimitedFuel
        unlimitedsaved = UnlimitedSaveTime;
        if (unlimitedsaved != "")
        {
            unlimitedTime = DateTime.Parse(unlimitedsaved) - DateTime.Now;
            if (unlimitedTime.TotalSeconds < 43200 && unlimitedTime.TotalSeconds > 0)
            {
                isUnlimitedFuels = true;
            }
            else
            {
                isUnlimitedFuels = false;
            }
        }
        //Check UnlimitedFuel

        //if fuel is full then return
        if (Fuels == 10)
        {
            ResetTimer();
            return;
        }
        TimeSpan remainingTime = futureDateTime - DateTime.Now;
        totalSecs = (long)remainingTime.TotalSeconds;
        if (Fuels < 10 && totalSecs >= 0)
        {
            PopFuel = remainingTime.Minutes.ToString("0" + "m") + "  " + remainingTime.Seconds.ToString("00" + "s");
        }
        else
        {
            if (Fuels < 10)
            {
                Fuels++;
                ResetTimer();
            }
        }
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause)
            StoreData();
    }
    private void OnApplicationQuit()
    {
        StoreData();
    }
    public void StoreData()
    {
        PlayerPrefs.SetInt(StringConstants.fuelCount, fuels);
        PlayerPrefs.SetString(StringConstants.fuelTime, PopFuel);
        FutureSaveTime = futureDateTime.ToString();
    }


    public void AddFuel(int _val)
    {
        if (Fuels < 10)
        {
            Fuels += _val;
            if (Fuels >= 10)
            {
                Fuels = 10;
            }
        }
    }
    public void PlayButton()
    {
        if (Fuels <= 0)
        {
            GameManager.instance.uIManager.noFuelPanel.SetActive(true);
        }
        else
        if (isUnlimitedFuels)
        {
            //Dont Decrease fuels until 12 hours, from the time player buyed the pack
        }
        else
        {
            //Decrease the life
            Fuels--;
            GameManager.instance.sceneManager.NextScene(1);
        }
    }
    public void UnlimitedFuel()
    {
        unlimitedDT = DateTime.Now.AddHours(12);
        isUnlimitedFuels = true;
        UnlimitedSaveTime = unlimitedDT.ToString();
    }
}
