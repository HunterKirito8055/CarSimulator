using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterPrefab : MonoBehaviour
{
    public int chapterId;
    public string vehicleChapter;
    public string chapterName;
    public Text chapterText;
    public Image chapterImg;

    //Highlight the selected chapter background 
    public Image selectedImage;

    //Lock if the player doesnt unlocked the Chapter
    public bool isUnlocked;
    public Image Lock;

    //LevelButton
    public int numberOfLevels;
    public Transform levelButtonParent;
    public List<Button> levelButtons = new List<Button>();

}
