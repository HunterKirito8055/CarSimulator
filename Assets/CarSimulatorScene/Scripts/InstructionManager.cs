using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
{
    public static InstructionManager instance;

    #region SerializedFields
    [SerializeField] RectTransform textBarRect;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Text msgText;

    [Space]
    [SerializeField] RectTransform defaultRect;
    [SerializeField] RectTransform engineSeatbeltPosRect;
    [SerializeField] RectTransform wiperPosRect;
    [SerializeField] RectTransform headlightPosRect;
    #endregion

    bool isShowing = false;
    public bool IsShowing
    {
        get { return isShowing; }
        set
        {
            isShowing = value;
            if (!value && (instructionsQueue.Count >= 1))
            {
                ShowInstruction((InstructionType)instructionsQueue.Dequeue());
            }
        }
    }
    Queue instructionsQueue = new Queue();

    private void Awake()
    {
        instance = this;
    }

    #region Counters
    int defaultCounter = 2;
    int blinkersCounter = 0;
    int laneCounter = 0;
    int crashCounter = 0;
    int stopSignCounter = 0;
    int wiperCounter = 0;
    #endregion

    void ChangePostion(RectTransform to)
    {
        textBarRect.anchorMin = to.anchorMin;
        textBarRect.anchorMax = to.anchorMax;
        textBarRect.transform.localPosition = to.transform.localPosition;
    }
    public void GiveInstruction(InstructionType instruction)
    {
        if (!IsShowing && (instructionsQueue.Count == 0))
        {
            ShowInstruction(instruction);
        }
        else
        {
            //queue
            instructionsQueue.Enqueue(instruction);
        }
    }
    void ShowInstruction(InstructionType instruction)
    {
        string msg = "";
        switch (instruction)
        {
            case InstructionType.EngineStart:
                ChangePostion(engineSeatbeltPosRect);
                msg = "Turn on the Engine first to get started.";
                break;
            case InstructionType.Seatbelt:
                ChangePostion(engineSeatbeltPosRect);
                msg = "Put your seat belt to ensure safe travel";
                break;
            case InstructionType.Blinkers:
                blinkersCounter++;
                if (blinkersCounter == defaultCounter)
                {
                    msg = "Turn on the side blinkers to avoid crash during turns";
                    blinkersCounter = 0;
                }
                break;
            case InstructionType.HeadLight:
                ChangePostion(headlightPosRect);
                msg = "Turn on the head light during the night";
                break;
            case InstructionType.Wipers:
                wiperCounter++;
                if (wiperCounter == defaultCounter)
                {
                    ChangePostion(wiperPosRect);
                    msg = "Switch the wipers according to the climate";
                    wiperCounter = 0;
                }
                break;
            case InstructionType.StopSign:
                stopSignCounter++;
                if (stopSignCounter == defaultCounter)
                {
                    msg = "Must stop at stop sign to ensure safe cross";
                    stopSignCounter = 0;
                }
                break;
            case InstructionType.Lane:
                laneCounter++;
                if (laneCounter == defaultCounter)
                {
                    msg = "Stay on the dedicated lane to avoid crashes";
                    laneCounter = 0;
                }
                break;
            case InstructionType.Crash:
                crashCounter++;
                if (crashCounter == defaultCounter)
                {
                    msg = "Do not crash the vehicle";
                    crashCounter = 0;
                }
                break;
            case InstructionType.None:
                msg = "";
                break;
            default:
                break;
        }

        if (!msg.Equals(""))
        {
            StartCoroutine(IShowInstruction(msg));
        }
    }

  
    IEnumerator IShowInstruction(string msg)
    {
        IsShowing = true;
        msgText.text = msg;
        //canvasGroup.alpha = 1;
        while (canvasGroup.alpha < 1)
        {   
            canvasGroup.alpha += 0.2f;
            yield return new WaitForSeconds(0.02f);
        }
        
        yield return new WaitForSeconds(2.0f);
        //canvasGroup.alpha = 0;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 0.2f;
            yield return new WaitForSeconds(0.02f);
        }
        ChangePostion(defaultRect);
        IsShowing = false;
    }
}
public enum InstructionType
{
    EngineStart,
    Seatbelt,
    Blinkers,
    HeadLight,
    Wipers,
    StopSign,
    Lane,
    Crash,
    None
}