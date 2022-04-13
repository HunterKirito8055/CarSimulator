using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainEffect : MonoBehaviour
{
    public Sprite[] sprite;
    public Color color;
    public int particlePoolSize = 10;
    [Range(0.01f, 1f)]
    public float rainRate = 0.05f;

    float tempRate = 0f;

    [SerializeField] Image rainImg;
    List<Image> particles;
    int screenHeight, screenWidth;
    float rainStartTime = 40;
    float rainingTime = 10;

    bool isRaining;
    public bool IsRaining
    {
        get
        {
            return isRaining;
        }
        set
        {
            isRaining = value;
            if (isRaining)
                StartCoroutine(Play());
            else
            {
                StopCoroutine(Play());
            }
        }
    }

    void Start()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        particles = new List<Image>();
        Create();
        StartCoroutine(StartRain(rainStartTime,rainingTime));
    }

    void Create()
    {
        for (int i = 0; i < particlePoolSize; i++)
        {
            Image newParticle = Instantiate(rainImg, transform);
            newParticle.transform.SetParent(transform);  //.parent = transform;
            newParticle.transform.position = GetRandomPosition();
            newParticle.transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
            newParticle.transform.Rotate(Vector3.forward * Random.Range(0f, 180f));
            newParticle.sprite = sprite[Random.Range(0, sprite.Length)];
            newParticle.color = color;
            particles.Add(newParticle);
        }
    }


    IEnumerator Play()
    {
        yield return null;
        while (IsRaining)
        {
            tempRate += Time.deltaTime;
            if (tempRate >= rainRate)
            {
                GetImageFromPool();
                tempRate = 0f;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    Image GetImageFromPool()
    {
        foreach (var item in particles)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                item.gameObject.transform.position = GetRandomPosition();
                StartCoroutine(IECrossFade(item));
                return item;
            }
        }
        Create();
        return GetImageFromPool();
    }

    public IEnumerator IECrossFade(Image image)
    {
        yield return null;

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        float wait = 1.8f;

        image.CrossFadeAlpha(0, wait, image.transform);
        Vector3 pos = new Vector3(image.transform.position.x, image.transform.position.y - 15f, image.transform.position.z);
        while (wait > 0)
        {
            wait -= Time.deltaTime;
            image.transform.position = Vector3.Lerp(image.transform.position, pos, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        image.gameObject.SetActive(false);
    }


    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(0, screenWidth), Random.Range(0, screenHeight), 0);
    }


    IEnumerator StartRain(float startTime, float stopTime)
    {
        yield return new WaitForSeconds(startTime);
        //StopRain
        IsRaining = true;
        yield return new WaitForSeconds(stopTime);
        IsRaining = false;
    }
}
