using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("时间")]
    public float realSecondsPerGameDay = 600f;
    [Range(0f, 1f)] public float currentTimeOfDay = 0.25f;

    [Header("太阳/月亮")]
    public Light directionalLight;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    Light[] outdoorLights;

    void Awake()
    {
        Instance = this;
        outdoorLights = GetLightsByTag("OutdoorLight");
    }

    Light[] GetLightsByTag(string tag)
    {
        var objs = GameObject.FindGameObjectsWithTag(tag);
        var lights = new Light[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            lights[i] = objs[i].GetComponent<Light>();
        return lights;
    }

    void Update()
    {
        currentTimeOfDay += Time.deltaTime / realSecondsPerGameDay;
        if (currentTimeOfDay >= 1f) currentTimeOfDay -= 1f;

        float hour = GetGameHour();
        UpdateSun();
        UpdateOutdoorLights(hour);
    }

    void UpdateSun()
    {
        directionalLight.transform.rotation =
            Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0f);

        float intensity = GetIntensityLinear(currentTimeOfDay);

        directionalLight.intensity = intensity;
        directionalLight.color = sunColor.Evaluate(currentTimeOfDay);
        RenderSettings.ambientLight = sunColor.Evaluate(currentTimeOfDay);
        RenderSettings.fogColor = sunColor.Evaluate(currentTimeOfDay);

        // 控制天空盒亮度，保持自然感
        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_Exposure", intensity);
            DynamicGI.UpdateEnvironment();
        }
    }

    float GetIntensityLinear(float t)
    {
        float[] times  = { 0.00f, 0.05f, 0.10f, 0.20f, 0.30f, 0.40f, 0.50f, 0.60f, 0.70f, 0.80f, 0.90f, 0.95f, 1.00f };
        float[] values = { 0.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 0.00f };

        for (int i = 0; i < times.Length - 1; i++)
        {
            if (t >= times[i] && t <= times[i + 1])
            {
                float localT = (t - times[i]) / (times[i + 1] - times[i]);
                return Mathf.Lerp(values[i], values[i + 1], localT);
            }
        }
        return 0f;
    }

    void UpdateOutdoorLights(float hour)
    {
        bool outdoorOn = hour >= 18f || hour < 6f;
        foreach (var l in outdoorLights)
            if (l != null) l.enabled = outdoorOn;
    }

    public float GetGameHour() => currentTimeOfDay * 24f;
    public bool IsDaytime() => GetGameHour() >= 6f && GetGameHour() < 18f;
}