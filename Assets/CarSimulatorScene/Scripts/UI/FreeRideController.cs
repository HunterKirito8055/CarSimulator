using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeRideController : MonoBehaviour
{
    [SerializeField] SceneController sceneController;

    [SerializeField] Transform content;
    [SerializeField] Scrollbar scrollbar;
    float slideLerpVal = 0.05f;

    public float scroll_pos = 0;
    public float[] pos;


    [SerializeField] FreeRideChapterInfo[] freeRideChapterInfos;
    [SerializeField] Color selectedColor;
    [SerializeField] Color normalColor;
    [SerializeField] FreeRidePrefab prefab;

    // Start is called before the first frame update
    void Start()
    {
        CreateLevels();
    }
    void CreateLevels()
    {
        foreach (var item in freeRideChapterInfos)
        {
            FreeRidePrefab gO = Instantiate(prefab, content);

            //Assign chapterInformation
            gO.name = item.chapterName;
            gO.chapterImage.sprite = item.chapterImg_Sprite;
            gO.chapterName.text = item.chapterName;
            item.selectedImage = gO.selectedImage;
            item.lockImage = gO.lockImage;

            item.lockImage.gameObject.SetActive(item.isLock); //Lock open for 1st world
            if(!item.isLock) //If not locked then add to the button listener
            gO.adButton.onClick.AddListener(delegate { OpenFreeRideWorld(item.chapterId); });
        }
    }

    void OpenFreeRideWorld(int _index)
    {
        GameMaster.instance.freeRide_Chapter = _index;
        sceneController.NextScene(1);
    }
    void Update()
    {
        if (content.childCount > 0)
        {
            pos = new float[content.childCount];
            float distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }
            if (Input.GetMouseButton(0))
            {
                scroll_pos = scrollbar.value;
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) //Checking the distance and make the nearest prefab as center
                    {
                        scrollbar.value = Mathf.Lerp(scrollbar.value, pos[i], slideLerpVal);
                    }
                }
            }

            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    freeRideChapterInfos[i].selectedImage.color = selectedColor;
                    for (int j = 0; j < pos.Length; j++)
                    {
                        if (j != i)
                        {
                            freeRideChapterInfos[j].selectedImage.color = normalColor;
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class FreeRideChapterInfo
{
    public int chapterId;
    public bool isLock;
    public string chapterName;
    public Sprite chapterImg_Sprite;
    public Image selectedImage;
    public Image lockImage;
}
