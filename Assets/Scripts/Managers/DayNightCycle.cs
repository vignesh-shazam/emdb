using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Sun")]
    [SerializeField] private Light directionalLight;

    [Header("Day Settings")]
    [SerializeField] private float sunriseHour = 6f;
    [SerializeField] private float sunsetHour = 18f;

    [Header("Light Settings")]
    [SerializeField] private float dayLightIntensity = 1.2f;
    [SerializeField] private float nightLightIntensity = 0.05f;

    [Header("Ambient Settings")]
    [SerializeField] private float dayAmbientIntensity = 1f;
    [SerializeField] private float nightAmbientIntensity = 0.2f;

    private void Update()
    {
        if (GameTimeManager.Instance == null)
        {
            return;
        }

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        float currentTime =
            GameTimeManager.Instance.CurrentHour +
            GameTimeManager.Instance.CurrentMinute / 60f;

        float dayProgress = Mathf.InverseLerp(
            sunriseHour,
            sunsetHour,
            currentTime
        );

        dayProgress = Mathf.Clamp01(dayProgress);

        float daylightAmount =
            Mathf.Sin(dayProgress * Mathf.PI);

        float lightIntensity = Mathf.Lerp(
            nightLightIntensity,
            dayLightIntensity,
            daylightAmount
        );

        float ambientIntensity = Mathf.Lerp(
            nightAmbientIntensity,
            dayAmbientIntensity,
            daylightAmount
        );

        if (directionalLight != null)
        {
            directionalLight.intensity = lightIntensity;

            float sunAngle = Mathf.Lerp(
                -20f,
                200f,
                dayProgress
            );

            directionalLight.transform.rotation =
                Quaternion.Euler(sunAngle, -30f, 0f);
        }

        RenderSettings.ambientIntensity = ambientIntensity;
    }
}