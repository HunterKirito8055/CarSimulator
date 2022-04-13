using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeController : MonoBehaviour
{
    void Start()
    {
        GameMaster.instance.ShowInterstAd_AtPauseExit();
        AdsManager.instance.ShowBanner_InterstitialNotAvailable();
    }
}
