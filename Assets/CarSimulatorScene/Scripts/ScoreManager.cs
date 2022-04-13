using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int score = 0;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
           
            if(score >= 0)
            {
                greenScoreBar.fillAmount = ScoreValue((float)score);
                redScoreBar.fillAmount = 0;
            }
            else if (score <= 0)
            {
                redScoreBar.fillAmount = -ScoreValue((float)score);
                greenScoreBar.fillAmount = 0;
            }
            scoreTxt.text = score.ToString();
        }
    }

    //Score
    [SerializeField] Image redScoreBar;
    [SerializeField] Image greenScoreBar;
    [SerializeField] Text scoreTxt;
    //Score
    private void Awake()
    {
        Score = 0;
    }
  
    float ScoreValue(float score)
    {
        float totalScore = score / 500;
        return totalScore;
    }
  
}
