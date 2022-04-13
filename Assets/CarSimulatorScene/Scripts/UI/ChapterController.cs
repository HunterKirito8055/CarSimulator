using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterController : MonoBehaviour
{
    [SerializeField] SceneController sceneController;
    [SerializeField] Color selectedColor;
    [SerializeField] Color normalColor;
    [SerializeField] Color vehicleButton_Color;
    [SerializeField] Color vehicleButton_SelectedColor;
    [SerializeField] Scrollbar scrollbar;

    private float scroll_pos = 0;
    float[] pos;
    float slideLerpVal = 0.05f;


    [SerializeField] ChapterPrefab chapterPrefab;

    [SerializeField] VehicleChapterInfo[] vehicleChapterInfos;
    [SerializeField] Button buttonPrefab;
    [SerializeField] Transform content;
    [SerializeField] Button[] vehicleTypeButtons;
    [SerializeField] Button lessonsPlayBtn;

    int selectedIndex;
    public int SelectIndex
    {
        get
        {
            return selectedIndex;
        }
        set
        {
            selectedIndex = value;
            PreResetScrollBarPositions();
            for (int i = 0; i < vehicleChapterInfos.Length; i++)
            {
                vehicleTypeButtons[i].GetComponent<Image>().color = i == selectedIndex ? vehicleButton_SelectedColor : vehicleButton_Color;
                //Make the selected index chapters as child of content to scroll  
                //Disable other vehicle chapter objects expect current
                foreach (var obj in vehicleChapterInfos[i].chapterInfos)
                {
                    obj.thisObject.transform.SetParent(i == selectedIndex ? content.transform : this.transform); //parent = i == selectedIndex ? content.transform : this.transform;
                    obj.thisObject.SetActive(i == selectedIndex ? true : false);
                }
            }
            ResetScrollBar();
        }
    }

    void Start()
    {
        CreateLevels();
        //Reset scroll position to car first chapter
        lessonsPlayBtn.onClick.AddListener(() => DisableVehicleChapters(0));
    }
   
    private void OnEnable()
    {
        ResetScrollBar();
    }
    private void OnDisable()
    {
        PreResetScrollBarPositions();
    }

    void ResetScrollBar()
    {
        scrollbar.value = 0;
        scroll_pos = 0;
        scrollbar.value = 0;
    }
    void PreResetScrollBarPositions()
    {
        scrollbar.value = 1;
        scrollbar.size = 1;
    }


    void CreateLevels()
    {
        foreach (var item in vehicleChapterInfos)
        {
            for (int i = 0; i < item.chapterInfos.Count; i++)
            {
                ChapterPrefab newGo = Instantiate(chapterPrefab, content);

                //Assign chapterInformation
                newGo.name = item.chapterInfos[i].chapterName;
                newGo.chapterId = item.chapterInfos[i].chapterId;
                newGo.vehicleChapter = item.vehicleChapter;
                newGo.chapterName = item.chapterInfos[i].chapterName;
                newGo.numberOfLevels = item.chapterInfos[i].numberOfLevels;
                newGo.chapterImg.sprite = item.chapterInfos[i].chapterImg_Sprite;
                newGo.chapterText.text = item.chapterInfos[i].chapterName;

                item.chapterInfos[i].selectedImage = newGo.selectedImage;
                item.chapterInfos[i].thisObject = newGo.gameObject;

                if (item.vehicleChapter != StringConstants.car)
                {
                    //disable all expect car chapters 
                    newGo.gameObject.SetActive(false);
                }

                //Dont lock the first chapter
                if (i == 0 && item.vehicleChapter == StringConstants.car)
                {
                    PlayerPrefs.SetInt(StringConstants.unlockedChapter + newGo.vehicleChapter + newGo.chapterName, 1);
                }
                newGo.isUnlocked = PlayerPrefs.GetInt(StringConstants.unlockedChapter + newGo.vehicleChapter + newGo.chapterName, 0) == 0 ? true : false;
                //newGo.Lock.gameObject.SetActive(newGo.isUnlocked);
                newGo.Lock.gameObject.SetActive(false);
                for (int j = 0; j < item.chapterInfos[i].numberOfLevels; j++)
                {
                    Button button = Instantiate(buttonPrefab, newGo.levelButtonParent);
                    //button.interactable = !newGo.isUnlocked;
                    button.interactable = true;
                    button.GetComponentInChildren<Text>().text = (j + 1).ToString();
                    newGo.levelButtons.Add(button.GetComponent<Button>());
                    button.onClick.AddListener(() =>
                    {
                        GameMaster.instance.selected_ChapterId = newGo.chapterId;
                        GameMaster.instance.selected_LevelNumber = button.gameObject.transform.GetSiblingIndex();
                        GameMaster.instance.selected_vehicle = item.vehicleChapter;
                        GameManager.instance.PlayButton();
                    });
                }
            }
        }
        //Start with first chapter page
        SelectIndex = 0;
    }

    public void DisableVehicleChapters(int index)
    {
        SelectIndex = index;
        StringConstants.selectedVehicleMode = (VehicleMode)index;
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
                    vehicleChapterInfos[SelectIndex].chapterInfos[i].selectedImage.color = selectedColor;
                    for (int j = 0; j < pos.Length; j++)
                    {
                        if (j != i)
                        {
                            vehicleChapterInfos[SelectIndex].chapterInfos[j].selectedImage.color = normalColor;
                        }
                    }
                }
            }
        }
    }


}

[System.Serializable]
public class ChapterInfo
{
    public int chapterId;
    public string chapterName;
    public int numberOfLevels;
    public Sprite chapterImg_Sprite;
    public Image selectedImage;
    public GameObject thisObject;
}

[System.Serializable]
public class VehicleChapterInfo
{
    public string vehicleChapter;
    public List<ChapterInfo> chapterInfos = new List<ChapterInfo>();
}