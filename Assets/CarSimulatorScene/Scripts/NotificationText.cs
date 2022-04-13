using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationText : MonoBehaviour
{
    private Text notificationText;
    public Image notificationImg;

    public Color green;
    public Color red;
    private void Awake()
    {
        notificationText = GetComponentInChildren<Text>();
      //  CreateNotification("", 0, false);
    }

    public void CreateNotification(string _message, int points, bool isEarned)
    {
        transform.SetAsLastSibling();
        if (isEarned == false)
        {
            notificationText.text = _message + "\n" + points + " - " + "Points Deducted";
            notificationImg.color = red;
        }
        else
        {
            notificationText.text = _message + "\n" + points + " - " + "Points Earned";
            notificationImg.color = green;
        }
        StartCoroutine("FadeOutMessage");
    }

    IEnumerator FadeOutMessage()
    {
        NotificationContentView.instance.isAlreadyExist = true;
        yield return new WaitForSeconds(1.2f);
        float colorAlpha = 1;
        while (notificationText.color.a > 0)
        {
            colorAlpha -= Time.deltaTime * 2f;
            notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, colorAlpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CreateNotification("", 0, false);
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 1);
        gameObject.SetActive(false);
        NotificationContentView.instance.isAlreadyExist = false;

    }

}
