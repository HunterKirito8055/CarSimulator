using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//bhargav
public class SettingsPanel : MonoBehaviour
{

    [SerializeField] Button leftSideBtn;
    [SerializeField] Button rightSideBtn;

    [SerializeField] Button[] StreeringBtns; //follow the order in the Hierarchy

    public RCC_Settings.MobileController mobileController;
    [SerializeField] RCC_UIDashboardDisplay rCC_UIDashboardDisplay;
    [SerializeField] Button musicBtn;
    [SerializeField] Text musicBtnTxt;
    [SerializeField] Button soundBtn;
    [SerializeField] Text soundBtnTxt;
    private void Start()
    {
        ButtonBasic((int)RCC_Settings.Instance.mobileController);

        StreeringBtns[0].onClick.AddListener(() => SetMobileController(RCC_Settings.MobileController.TouchScreen));
        StreeringBtns[1].onClick.AddListener(() => SetMobileController(RCC_Settings.MobileController.Gyro));
        StreeringBtns[2].onClick.AddListener(() => SetMobileController(RCC_Settings.MobileController.SteeringWheel));
        StreeringBtns[3].onClick.AddListener(() => SetMobileController(RCC_Settings.MobileController.Joystick));
        print(RCC_Settings.Instance.mobileController);

        //for (int i = 0; i < StreeringBtns.Length; i++)
        //{
        // StreeringBtns[i].onClick.AddListener(() => SetMobileController((RCC_Settings.MobileController)i));

        // print((RCC_Settings.MobileController)i);
        //}

        leftSideBtn.onClick.AddListener(() => SetSteeringSide(RCC_Settings.StreeringSide.Left));
        rightSideBtn.onClick.AddListener(() => SetSteeringSide(RCC_Settings.StreeringSide.Right));

        if (RCC_Settings.Instance.streeringSide == RCC_Settings.StreeringSide.Left)
        {
            leftSideBtn.interactable = false;
            rightSideBtn.interactable = true;
        }
        else
        {
            leftSideBtn.interactable = true;
            rightSideBtn.interactable = false;
        }

        //music
        if (PlayerPrefs.GetInt(StringConstants.MusicOn) == 1)
        {
            musicBtnTxt.text = "music\non";
            //SoundManager.Instance.Play_BgMusic();

        }
        else
        {
            musicBtnTxt.text = "music\noff";
            //SoundManager.Instance.Stop_BgMusic();

        }

        if (PlayerPrefs.GetInt(StringConstants.SoundOn) == 1)
            soundBtnTxt.text = "sound\non";
        else
            soundBtnTxt.text = "sound\noff";

        musicBtn.onClick.AddListener(() => MusicSetup());
        soundBtn.onClick.AddListener(() => SoundSetup());

    }
    void MusicSetup()
    {
        if (PlayerPrefs.GetInt(StringConstants.MusicOn) == 1)
        {//on
            musicBtnTxt.text = "music \n off";
            PlayerPrefs.SetInt(StringConstants.MusicOn, 0);
            SoundManager.Instance.Stop_BgMusic();
        }
        else
        {
            PlayerPrefs.SetInt(StringConstants.MusicOn, 1);
            musicBtnTxt.text = "music \n on";
            SoundManager.Instance.Play_BgMusic();
        }
    }
    void SoundSetup()
    {
        if (PlayerPrefs.GetInt(StringConstants.SoundOn) == 1)
        {
            soundBtnTxt.text = "sound \n off";
            PlayerPrefs.SetInt(StringConstants.SoundOn, 0);
            RCC_Settings.Instance.audioMixer.audioMixer.SetFloat("volume", -80f);
        }
        else
        {
            soundBtnTxt.text = "sound \n on";
            PlayerPrefs.SetInt(StringConstants.SoundOn, 1);
            RCC_Settings.Instance.audioMixer.audioMixer.SetFloat("volume", 0f);
        }

    }
    void SetMobileController(RCC_Settings.MobileController _mobileController)
    {
        mobileController = _mobileController;
        ButtonBasic((int)_mobileController);
        RCC_Settings.Instance.mobileController = _mobileController;

        if (_mobileController == RCC_Settings.MobileController.SteeringWheel)
            RCC_MobileButtons.Instance.currentActiveUIButtons.indicators.SetActive(false);
        else
            RCC_MobileButtons.Instance.currentActiveUIButtons.indicators.SetActive(true);

        Debug.Log("Mobile Controller now " + _mobileController.ToString());
    }
 
    void SetSteeringSide(RCC_Settings.StreeringSide _streeringSide)
    {

        RCC_Settings.Instance.streeringSide = _streeringSide;
        if (_streeringSide == RCC_Settings.StreeringSide.Left)
        {
            leftSideBtn.interactable = false;
            rightSideBtn.interactable = true;
            RCC_MobileButtons.Instance.currentActiveUIButtons = RCC_MobileButtons.Instance.left_UI_buttons;
            rCC_UIDashboardDisplay.RightControllerButtons.SetActive(false);
            rCC_UIDashboardDisplay.LeftControllerButtons.SetActive(true);
            rCC_UIDashboardDisplay.controllerButtons = rCC_UIDashboardDisplay.LeftControllerButtons;
        }
        else
        {
            leftSideBtn.interactable = true;
            rightSideBtn.interactable = false;
            RCC_MobileButtons.Instance.currentActiveUIButtons = RCC_MobileButtons.Instance.right_UI_buttons;
            rCC_UIDashboardDisplay.LeftControllerButtons.SetActive(false);
            rCC_UIDashboardDisplay.RightControllerButtons.SetActive(true);
            rCC_UIDashboardDisplay.controllerButtons = rCC_UIDashboardDisplay.RightControllerButtons;
        }
        Debug.Log("Steering Side now " + _streeringSide.ToString());
        RCC_Settings.Instance.mobileController = mobileController;
    }

    void ButtonBasic(int num)
    {
        for (int i = 0; i < StreeringBtns.Length; i++)
        {
            if (i != num)
                StreeringBtns[i].interactable = true;
            else
                StreeringBtns[i].interactable = false;
        }
    }


}
