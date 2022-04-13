using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityBtn : MonoBehaviour
{
    //Anand Code
    [SerializeField] Text qualityText; 
    int currentQualityLevel = 0;
    int maxQualityLevel = 2;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeQuality);
        currentQualityLevel = QualitySettings.GetQualityLevel();
        qualityText.text = "QLevel " + currentQualityLevel.ToString();
    }
    public GameObject g;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
           // QualitySettings. = ShadowQuality.Disable;
        }
            //    var GameObject = Instantiate(g);
            //    GameObject.transform.SetParent(g.gameObject.transform.parent);
            //    GameObject.name = "Waypoint-3";
            //    GameObject.transform.localPosition = g.transform.localPosition;
            //}
        }
    public void ChangeQuality()
    {
        currentQualityLevel++;
        if (currentQualityLevel > maxQualityLevel)
        {
            currentQualityLevel = 0;
        }
        QualitySettings.SetQualityLevel(currentQualityLevel);
        qualityText.text = "QLevel " + currentQualityLevel.ToString();
    }
    //Anand Code
}
