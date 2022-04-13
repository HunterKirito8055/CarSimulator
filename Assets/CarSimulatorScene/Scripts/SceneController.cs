using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text totalScore;
    void Start()
    {
        if (totalScore != null)
        {
            int score = 0;
            score = PlayerPrefs.GetInt(StringConstants.score);
            totalScore.text = "SCORE : " + score.ToString();
        }
    }

    // Update is called once per frame
    public void GotoMainMenu()
    {
        GameMaster.instance.pauseExit_Counter++;
        StartCoroutine(ILoadLevelInAsync(0));
        // StoreFuelPrefs();
    }
    public void Reload()
    {
        StartCoroutine(ILoadLevelInAsync(SceneManager.GetActiveScene().buildIndex));

        //  StoreFuelPrefs();
    }
    void StoreFuelPrefs()
    {
        // GameManager.instance.fuelManager.StoreData();
    }

    public void NextScene(int buildIndex)
    {
        //SceneManager.LoadScene(1);
        StartCoroutine(ILoadLevelInAsync(buildIndex));
    }

    public void Nextlevel()
    {
        GameMaster.instance.selected_LevelNumber++;
        StartCoroutine(ILoadLevelInAsync(SceneManager.GetActiveScene().buildIndex));
    }

    [SerializeField] GameObject loadingPanel;
    [SerializeField] Image fillImage;

    IEnumerator ILoadLevelInAsync(int sceneIndex)
    {
        AdsManager.instance.HideBanner();
        loadingPanel.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        //asyncOperation.allowSceneActivation = false;
        //float barProgress = 0;
        //float randomLoadingComponent = Random.Range(3f, 5f);

        //while (barProgress <= 100 || asyncOperation.allowSceneActivatio   n == false)
        //{
        //    barProgress += randomLoadingComponent;
        //    fillImage.fillAmount = barProgress / 100;
        //    //Debug.Log(barProgress);
        //    Debug.Log(asyncOperation.progress);
        //    if (asyncOperation.isDone)
        //    {
        //        fillImage.fillAmount = 1;
        //        asyncOperation.allowSceneActivation = true;
        //    }
        //    yield return new WaitForSeconds(Time.deltaTime); //  / 10);
        //}
        while (!asyncOperation.isDone)
        {
            //Loading bar
            fillImage.fillAmount = asyncOperation.progress / 0.9f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(Time.deltaTime);
    }
}
