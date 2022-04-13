using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdRewardManager : MonoBehaviour
{
    [SerializeField] GameObject needle;
    [SerializeField] Button claimBtn;
    [SerializeField] Color fadedColor;
    [SerializeField] Image[] imgArray;
    Quaternion a;
    Quaternion b;
    Quaternion desPoint;
    bool isWatchPressed;

    void Start()
    {
        isWatchPressed = false;

        a = Quaternion.Euler(0, 0, 90f);
        b = Quaternion.Euler(0, 0, -90f);
        desPoint = a;
        claimBtn.onClick.AddListener(OnButtonClick);
        StartCoroutine(IPointerMove());
    }

    void Update()
    {
        //    if (needle.transform.rotation == b)
        //    {
        //        desPoint = a;
        //    }
        //    if (needle.transform.rotation == a)
        //    {
        //        desPoint = b;
        //    }
        //    needle.transform.rotation = Quaternion.Euler(0, 0, 2f * Time.deltaTime);
        //if (needle.transform.rotation == b)
        //{
        //    desPoint = a;
        //}
        //if (needle.transform.rotation == a)
        //{
        //    desPoint = b;
        //}
        ////needle.transform.rotation = Quaternion.Lerp(needle.transform.rotation, desPoint, Time.deltaTime * 0.8f);
        //needle.transform.rotation = Quaternion.RotateTowards(needle.transform.rotation, desPoint, Time.deltaTime * 0.8f);


        //if (())
        //{
        //    print("herererere");
        //}
        //print(needle.transform.eulerAngles.z);
        //PointerMove();

    }

    //void PointerMover()
    //{
    //    while (!isWatchPressed)
    //    {
    //        if (needle.transform.rotation == b)
    //        {
    //            desPoint = a;
    //        }
    //        if (needle.transform.rotation == a)
    //        {
    //            desPoint = b;
    //        }
    //        needle.transform.rotation = Quaternion.LerpUnclamped(needle.transform.rotation, desPoint, Time.deltaTime * 0.8f);
    //    }

    //}

    IEnumerator IPointerMove()
    {
        float i = 0;
        int variable = 25;
        while (!isWatchPressed)
        {
            if (needle.transform.rotation.z <= b.z || needle.transform.rotation.z >= a.z)
                variable = -variable;
            i += variable;
            needle.transform.rotation = Quaternion.Euler(0, 0, i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnButtonClick()
    {
        isWatchPressed = true;
        float stopAngle = needle.transform.eulerAngles.z;
        int selected = 0;
        if (stopAngle <= 90 && stopAngle >= 75)
        {
            selected = 0;
        }
        else if (stopAngle < 75 && stopAngle >= 35)
        {
            selected = 1;
        }
        else if ((stopAngle < 30 && stopAngle >= 0) || (stopAngle <= 360 && stopAngle >= 335))
        {
            selected = 2;
        }
        else if (stopAngle < 335 && stopAngle >= 285)
        {
            selected = 3;
        }
        else if (stopAngle < 285 && stopAngle >= 270)
        {
            selected = 4;
        }

        AnimateSelected(selected);
    }
    void AnimateSelected(int selected)
    {
        for (int i = 0; i < imgArray.Length; i++)
        {
            if (i != selected)
            {
                imgArray[i].color = fadedColor;
            }
        }
        //amount doubling and Adding

        print("Reward" + selected);
    }
}
