using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILightManager : MonoBehaviour
{

    [SerializeField] AudioSource indicatorSound;

    [SerializeField] Renderer RF_lightRenderer;
    [SerializeField] Renderer LF_lightRenderer;
    [SerializeField] Renderer RR_lightRenderer;
    [SerializeField] Renderer LR_lightRenderer;

    [SerializeField] Material[] cloneMaterial = new Material[2];

    private int RFemissionColorID;
    private int LFemissionColorID;

    float indicatorTimer = 0f;
    [HideInInspector]public IndicatorType presentIndicator = IndicatorType.Off;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            cloneMaterial[i] = Instantiate(RR_lightRenderer.material, gameObject.transform);
            cloneMaterial[i].SetColor("_EmissionColor", Color.yellow * 0);
        }

        RR_lightRenderer.material = cloneMaterial[0];
        LR_lightRenderer.material = cloneMaterial[1];
        // LightingAll(0);

        //front Indicators
        RF_lightRenderer.materials[5].EnableKeyword("_EMISSION");
        RFemissionColorID = Shader.PropertyToID("_EmissionColor");

        LF_lightRenderer.materials[5].EnableKeyword("_EMISSION");
        LFemissionColorID = Shader.PropertyToID("_EmissionColor");
        //front Indicators
    }

    // Update is called once per frame
    void Update()
    {
        indicatorTimer += Time.deltaTime;
        Indicators(presentIndicator);
        if (indicatorTimer >= 1f)
        {
            indicatorTimer = 0f;
        }
    }

    void ResetIndicators()
    {
        //Reset Indicators
        RF_lightRenderer.materials[5].color = Color.red;
        RF_lightRenderer.materials[5].SetColor(RFemissionColorID, Color.black);
        RF_lightRenderer.materials[5].SetFloat("_Metallic", 1);

        LF_lightRenderer.materials[5].color = Color.red;
        LF_lightRenderer.materials[5].SetColor(LFemissionColorID, Color.black);
        LF_lightRenderer.materials[5].SetFloat("_Metallic", 1);

        cloneMaterial[0].SetColor("_EmissionColor", Color.yellow * 0);
        cloneMaterial[1].SetColor("_EmissionColor", Color.yellow * 0);
        //Reset Indicators
    }
    public void Indicators(IndicatorType indicatorType)
    {
        presentIndicator = indicatorType;
        //Emission Control
        float ceiling = 1f;
        float emission = Mathf.PingPong(Time.time * 2f, ceiling);
        //  float emission = floor + Mathf.PingPong(Time.time * 2f, ceiling - floor);

        //Emission Control
        switch (indicatorType)
        {
            case IndicatorType.Off:
                //LightingAll(0);
                indicatorTimer = 0f;
                ResetIndicators();
                break;
            case IndicatorType.Right:
                if (indicatorTimer >= .5f)
                {
                    // if (indicatorSound.isPlaying)
                    // indicatorSound.Stop();
                }
                else
                {
                    //if (!indicatorSound.isPlaying && indicatorTimer <= .1f)
                    //  indicatorSound.Play();
                }
                cloneMaterial[0].SetColor(RFemissionColorID, Color.yellow * emission);
                RF_lightRenderer.materials[5].SetColor(RFemissionColorID, Color.yellow * emission);

                break;
            case IndicatorType.Left:
                if (indicatorTimer >= .5f)
                {
                    //if (indicatorSound.isPlaying)
                    // indicatorSound.Stop();
                }
                else
                {
                    // if (!indicatorSound.isPlaying && indicatorTimer <= .1f)
                    // indicatorSound.Play();
                }
                LF_lightRenderer.materials[5].SetColor(LFemissionColorID, Color.yellow * emission);
                cloneMaterial[1].SetColor("_EmissionColor", Color.yellow * emission);
                break;
            case IndicatorType.All:
                if (indicatorTimer >= .5f)
                {
                    // if (indicatorSound.isPlaying)
                    // indicatorSound.Stop();
                }
                else
                {
                    //if (!indicatorSound.isPlaying && indicatorTimer <= .1f)
                    // indicatorSound.Play();
                }
                break;
            default:
                break;
        }

    }


}
public enum IndicatorType
{
    Off, Right, Left, All
}