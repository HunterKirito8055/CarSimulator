using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using UnityEngine.Events;
public class NativePopUps : MonoBehaviour
{
    private void Start()
    {
        if (GetNativePopUp() == 0) //If false
        {
            ShowTwoButtonPopUp("Hi There", "To play this game you must accept our Terms of Services." +
                    "Review our Terms and Privacy Notice about how your data is processed.", "TERMS", "ok", OpenTermsOfServicePage, OnOkBtn);
        }
    }

    public void OpenTermsOfServicePage()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowSingleButtonPopUp("Internet", "No Internet Connection.", "ok");

        }
        else
        {
            Debug.Log("Opening url");
            Application.OpenURL("http://www.9gape.com/terms-of-service/%22%22");
        }
    }
    public void OnOkBtn()
    {
        SetNativePopUp();
    }
    public void SetNativePopUp()
    {
        PlayerPrefs.SetInt("NativePopUp", 1);
    }
    public int GetNativePopUp()
    {
        return PlayerPrefs.GetInt("NativePopUp", 0);
    }


    public void ShowSingleButtonPopUp(string _title, string _message, string _buttonName)
    {
        NativeUI.AlertPopup alert = NativeUI.Alert(_title, _message, _buttonName);
    }
    public void ShowTwoButtonPopUp(string _title, string _message, string _firstButtonName, string _secondButtonName, UnityAction _firstButtonFunction, UnityAction _secondButtonFunction)
    {
#if !UNITY_EDITOR
        NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert(_title, _message, _firstButtonName, _secondButtonName);
        alert.OnComplete += delegate (int bIndex)
          {
              switch (bIndex)
              {
                  case 0:
                      _firstButtonFunction?.Invoke();
                      break;
                  case 1:
                      _secondButtonFunction?.Invoke();
                      break;
                  default:
                      break;
              }
          };
#endif
    }
    public void ShowThreeButtonPopUp(string _title, string _message, string _firstButtonName, string _secondButtonName, string _thirdButtonName, UnityAction _firstButtonFunction, UnityAction _secondButtonFunction, UnityAction _thirdButtonFunction)
    {
        NativeUI.AlertPopup alert = NativeUI.ShowThreeButtonAlert(_title, _message, _firstButtonName, _secondButtonName, _thirdButtonName);
        alert.OnComplete += delegate (int bIndex)
        {
            switch (bIndex)
            {
                case 0:
                    _firstButtonFunction?.Invoke();
                    break;
                case 1:
                    _secondButtonFunction?.Invoke();
                    break;
                case 2:
                    _thirdButtonFunction?.Invoke();
                    break;
                default:
                    break;
            }
        };
    }
}
