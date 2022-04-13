using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationContentView : MonoBehaviour
{
    public List<GameObject> listObjects;
    public GameObject notificationTextObj;
    public static NotificationContentView instance;
    public bool isAlreadyExist = false;
    public string id = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        isAlreadyExist = false;
    }

    void Start()
    {
        listObjects = new List<GameObject>();
        CreateItems(1);
    }
    IEnumerator ResetID()
    {
        yield return new WaitForSeconds(1f);
        id = "";
    }
    IEnumerator NotificationPopUp(string message, int points, bool isTrue, string _id)
    {
        //if same and != null ids, are called again and again within a second -> then reset it 
        if (id == _id && id != "")
        {
            yield return new WaitForSeconds(Time.deltaTime);
            StartCoroutine(ResetID());
        }
        else
        {
            id = _id;
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => isAlreadyExist == false);
            GameObject newGo = GetObjectFromPool();
            newGo.GetComponent<NotificationText>().CreateNotification(message, points, isTrue);

            //Setting Score Here //Ashish Code
            if (isTrue)
                GameManager.instance.scoreManager.Score += points;
            else
                GameManager.instance.scoreManager.Score -= points;
            //Setting Score Here //Ashish Code

            //Instructions PopUp
            switch (id)
            {
                case StringConstants.playerNotStopped_ID:
                    InstructionManager.instance.GiveInstruction(InstructionType.StopSign);
                    break;

                default:
                    break;
            }
        }
    }
    public void CreateNotification(string message, int points, bool isTrue, string id)
    {
        // GameObject newGo = GetObjectFromPool();
        // newGo.GetComponent<NotificationText>().CreateNotification(message);
        StartCoroutine(NotificationPopUp(message, points, isTrue, id));
    }

    GameObject GetObjectFromPool()
    {
        foreach (var item in listObjects)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        return GetObjectFromPool();
    }

    void CreateItems(int objectsize)
    {
        for (int i = 0; i < objectsize; i++)
        {
            GameObject newTxtGo = Instantiate(notificationTextObj, transform);
            newTxtGo.GetComponent<RectTransform>();
            newTxtGo.SetActive(false);
            listObjects.Add(newTxtGo);
        }
    }
}
