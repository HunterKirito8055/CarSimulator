using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTracker : MonoBehaviour
{
    public string menu;
    public void SendButtonAnalytics(string buttonName)
    {
       SoundManager.Instance.PlayButtonSound();
    }

   
}
