/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class dayNightScript : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float currentTime;
    public float timeSpeed = 1f;
    [Header("CurrentTime")]
    public string currentTimeString;
    
    [Header("Light Settings")]
    public Light sunLight;
    public float sunPosition = 1f;
    public float sunIntensity = 1f;
    public AnimationCurve sunIntensityMultiplier;
    public AnimationCurve lightTemperatureCurve;

    public bool isDay = true;

    // Start is called before the first frame update

    void Start() 
    {
        updateTimeText();
        checkShadowStatus();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime * timeSpeed;
        if (currentTime >= 24) 
        {
            currentTime = 0;
        }

        updateTimeText();
        updateLight();
        checkShadowStatus();
    }

    private void OnValidate()
    {
        updateLight();
        checkShadowStatus();
    }
    void updateTimeText()
    {
        currentTimeString = Mathf.Floor(currentTime).ToString("00") + ":" + ((currentTime % 1) * 60).ToString("00");

    }

    void updateLight()
    {
        float sunRotation = currentTime/24 * 360;
        sunLight.transform.rotation = Quaternion.Euler(sunRotation - 90f, sunPosition, 0f);

        float normalizedTime = currentTime/24;
        float intensityCurve = sunIntensityMultiplier.Evaluate(normalizedTime);

        HDAdditionalLightData sunLightData = sunLight.GetComponent<HDAdditionalLightData>();

        if (sunLightData != null)
        {
            sunLightData.intensity = intensityCurve * sunIntensity;
        }
        float tempMult = lightTemperatureCurve.Evaluate(normalizedTime);
        Light lightComp = sunLight.GetComponent<Light>();

        if (lightComp != null)
        {
            lightComp.colorTemperature = tempMult * 10000f;
        }
    }

    void checkShadowStatus()
    {
        HDAdditionalLightData sunLightData = sunLight.GetComponent<HDAdditionalLightData>();
        float currentSunRoration = currentTime;

        if (currentSunRoration >= 6f && currentSunRoration <= 18f)
        {
            sunLightData.EnableShadows(true);
            isDay = true;
        } 
        else 
        {
            sunLightData.EnableShadows(false);
            isDay = false;
        }
    }
}*/