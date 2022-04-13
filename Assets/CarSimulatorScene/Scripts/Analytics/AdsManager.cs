using System.Collections;
using UnityEngine;
using System;
using GoogleMobileAds.Common;
using System.Collections.Generic;
#if UNITY_ANDROID
using GoogleMobileAds.Api;
#endif

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    #region Private Variables

    private BannerView bannerView;
    private InterstitialAd interstitial;
    public RewardedAd rewardedAd;
    public bool isIntAdShown = false;

    public bool isInterstitialActive = false;
    public bool isBannerActivated = false;
    public int currentRewardedVideoIndex;
    public RewardBtnType rewardBtnType = RewardBtnType.None;
    #endregion

    #region Ad Id's
    // test id's ----------------------------------------
    private readonly string androidAppId = "ca-app-pub-3940256099942544~3347511713";
    private readonly string androidRewardedAdId = "ca-app-pub-3940256099942544/5224354917";
    private readonly string androidInterstitialAdId = "ca-app-pub-3940256099942544/1033173712";
    private readonly string androidBannerAdId = "ca-app-pub-3940256099942544/6300978111";


    private readonly string iosAppId = "ca-app-pub-3940256099942544~1458002511";
    private readonly string iosBannerAdId = "ca-app-pub-3940256099942544/2934735716";
    private readonly string iosInterstitialAdID = "ca-app-pub-3940256099942544/4411468910";
    private readonly string iosRewardedAdId = "ca-app-pub-3940256099942544/1712485313";

    // orginal id's -------------------------------------
    //   private string androidAppId = "";
    //private string androidRewardedAdId = "";
    //private string androidInterstitialAdId = "";
    //private string androidBannerAdId = "";


    //private string iosAppId = "";
    //private string iosBannerAdId = "";
    //private string iosInterstitialAdID = "";
    //private string iosRewardedAdId = "";

    #endregion

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {

#if UNITY_ANDROID
        string appId = androidAppId;
#elif UNITY_IPHONE
		string appId = iosAppId; 
#else
		string appId = "unexpected_platform";
#endif

        MobileAds.SetiOSAppPauseOnBackground(true);
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((initStatus) =>
        {
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        //MonoBehaviour.print("Adapter: " + className + " not ready.");
                        break;
                    case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        // MonoBehaviour.print("Adapter: " + className + " is initialized.");

                        break;
                }
            }
        });
        StartCoroutine(RequestAdsWithDelay());
    }
    #endregion

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().AddKeyword("CarSimulator").Build();
    }

    #region Rewarded Ads
    public void CreateAndLoadRewardedAd()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        //if (!UnityRemoteData.isFullAdsEnabled)
        //{
        //    return;
        //}

#if UNITY_EDITOR
        string adUnitId = androidRewardedAdId;
#elif UNITY_ANDROID
		string adUnitId = androidRewardedAdId;
#elif UNITY_IPHONE
		string adUnitId = iosRewardedAdId;
#else
		string adUnitId = "unexpected_platform";
#endif
        if (rewardedAd == null || !rewardedAd.IsLoaded())
        {
            // Create new rewarded ad instance.
            rewardedAd = new RewardedAd(adUnitId);

            // Called when an ad request has successfully loaded.
            rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            // Create an empty ad request.
            AdRequest request = CreateAdRequest();
            // Load the rewarded ad with the request.
            rewardedAd.LoadAd(request);
        }
    }

    public void ShowRewardedAd()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //  DialogController.instance.ShowDialog(DialogType.NoInternetDialog, DialogShow.OVER_CURRENT);
        }
        //if (!UnityRemoteData.isFullAdsEnabled)
        //{
        //    return;
        //}
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
            HideBanner();
        }
        else
        {
            if (rewardedAd == null || !rewardedAd.IsLoaded())
            {
#if UNITY_ANDROID

#elif UNITY_IOS
			//MobileNativePopups.OpenAlertDialog(
			//"Rewarded ad", "Rewarded ad is not ready yet",
			//"OK", 
			//() => { Debug.Log("Accept was pressed"); });
#endif
                //No internet PopUp
                //  DialogController.instance.ShowDialog(DialogType.NoInternetDialog, DialogShow.OVER_CURRENT);
                CreateAndLoadRewardedAd();
            }

        }
    }
    #endregion

    #region Interstitial Ads
    public void RequestInterstitial()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        //if (CPlayerPrefs.GetBool(AllStringConstants.ads, true) == false)
        //{
        //    return;
        //}

        //if (LevelController.GetUnlockLevel() < UnityRemoteData.adsStartlevel)
        //{
        //    return;
        //}
        if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isIntAdsEnabled)
        {
            return;
        }

        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        // string adUnitId = "unused";
        string adUnitId = androidInterstitialAdId;
#elif UNITY_ANDROID
		string adUnitId = androidInterstitialAdId;
#elif UNITY_IPHONE
		string adUnitId = iosInterstitialAdID;
#else
		string adUnitId = "unexpected_platform";
#endif
        adUnitId = androidInterstitialAdId;
        if (interstitial == null) //|| !interstitial.IsLoaded())
        {

            // Create an interstitial.
            interstitial = new InterstitialAd(adUnitId);

            // Register for ad events.
            interstitial.OnAdLoaded += HandleInterstitialLoaded;
            interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
            interstitial.OnAdOpening += HandleInterstitialOpened;
            interstitial.OnAdClosed += HandleInterstitialClosed;
            interstitial.OnAdLeavingApplication += HandleAdLeftApplication;

            // Load an interstitial ad.
            interstitial.LoadAd(CreateAdRequest());
        }
    }


    public void ShowInterstitial()
    {
        //if (LevelController.GetUnlockLevel() < UnityRemoteData.adsStartlevel)
        //{
        //    return;
        //}
        //if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isIntAdsEnabled)
        //{
        //    return;
        //}
        if (interstitial != null && interstitial.IsLoaded())
        {
            interstitial.Show();
            //  isInterstitialActive = true;
            // AdFadeImage.instance.BlackScreen(1f, 0f, null);
            //  PlayerPrefs.SetInt("AdFrequency", PlayerPrefs.GetInt("AdFrequency", 0) + 1);
            //GameAnalytics.AdFrequency(PlayerPrefs.GetInt("AdFrequency", 0));
        }
        else
        {
            RequestInterstitial();
        }
    }
    #endregion


    #region BannerAD
    public void RequestBanner()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        //if (CPlayerPrefs.GetBool(AllStringConstants.ads, true) == false)
        //{
        //    return;
        //}

        //if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isBannerAdEnabled)
        //{
        //    return;
        //}

        // Clean up banner ad before creating a new one.
        if (bannerView == null)
        {

            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
		string adUnitId = androidBannerAdId;
#elif UNITY_IPHONE
		string adUnitId = iosBannerAdId;
#else
		string adUnitId = "unexpected_platform";
#endif
            // Create a 320x50 banner at the top of the screen.
            //AdSize adsize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
            // Register for ad events.
            bannerView.OnAdLoaded += HandleAdLoaded;
            bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
            bannerView.OnAdOpening += HandleAdOpened;
            bannerView.OnAdClosed += HandleAdClosed;
            bannerView.OnAdLeavingApplication += HandleAdLeftApplication;

            // Load a banner ad.
            bannerView.LoadAd(CreateAdRequest());
            // HideBanner();
        }
    }
    public void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            //if (FooterMovement.instance != null)
            //    FooterMovement.instance.AfterAdRemoveReset();
        }
    }

    public void HideBanner()
    {
        //Debug.Log("in hide  banner " + bannerCounter);
        if (bannerView != null)
        {
            isBannerActivated = false;
            bannerView.Hide();
            //if (FooterMovement.instance != null)
            //    FooterMovement.instance.AfterAdRemoveReset();
        }
    }
    public void ShowBanner()
    {
        //Debug.Log("in show  banner " + bannerCounter);
        isBannerActivated = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        //if (CPlayerPrefs.GetBool(AllStringConstants.ads, true) == false)
        //{
        //    return;
        //}

        if (!UnityRemoteData.isFullAdsEnabled || !UnityRemoteData.isBannerAdEnabled)
        {
            return;
        }

        //if (LevelController.GetUnlockLevel() < UnityRemoteData.adsStartlevel)
        //{
        //    return;
        //}

        //if (isInterstitialActive == true)
        //{
        //    return;
        //}
        if (bannerView != null)
        {
            //if (FooterMovement.instance != null)
            //    FooterMovement.instance.Adjustment();
            bannerView.Show();
        }
        else
        {
            RequestBanner();
        }
    }

    #endregion

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {

        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
        isInterstitialActive = false;
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialOpened event received");
        isInterstitialActive = true;
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialClosed event received");
        isInterstitialActive = false;
        if (isIntAdShown == true)
        {
            //Open banner after closing the interstitialad
            isIntAdShown = false;
            ShowBanner();
        }
        RequestInterstitial();
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLeftApplication event received");
    }
    #endregion

    #region RewardedAd callback handlers

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");

    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.Message);

    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        CreateAndLoadRewardedAd();
        StartCoroutine(IHandleRewardedAdClosedDelay());

    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {

        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);
        StartCoroutine(IHandleRewardDelay());
    }
    #endregion


    #region Coroutines
    public IEnumerator RequestAdsWithDelay()
    {
        yield return new WaitForSeconds(2f);
        // RequestBanner();
        //RequestBanner2();
        RequestInterstitial();
        CreateAndLoadRewardedAd();
    }
    IEnumerator IHandleRewardDelay()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        isWatchedVideo = true;
    }
    IEnumerator IHandleRewardedAdClosedDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        print("rewarded ad Closed");
        ShowBanner();
        if (isWatchedVideo)
        {
            switch (rewardBtnType)
            {
                case RewardBtnType.Coins:
                    switch (currentRewardedVideoIndex)
                    {
                        case 0:
                            //Add Coins pack 1
                            break;
                        case 1:
                            //Add Coins pack 2

                            break;
                        case 2:
                            //Add Coins pack 3

                            break;
                        default:
                            break;
                    }
                    break;
                case RewardBtnType.Fuel:
                    GameManager.instance.fuelManager.AddFuel(1);
                    break;
                case RewardBtnType.None:
                    break;
                default:
                    break;
            }
        }
        isWatchedVideo = false;
    }
    IEnumerator IHandleRewardedAdFailedDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        print("rewarded ad failed");
    }
    #endregion

    public void ShowBanner_InterstitialNotAvailable()
    {
        if (!isInterstitialActive)
            ShowBanner();
        else
            isIntAdShown = true;
    }
    bool isWatchedVideo = false;
}
public enum RewardBtnType
{
    Coins,
    Fuel,
    None
}
