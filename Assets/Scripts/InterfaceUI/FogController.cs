using UnityEngine;

public class FogController : MonoBehaviour
{
    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public float fogDensityOn = 0.02f;
    public float fogDensityOff = 0f;
    public float transitionDuration = 1f;
    public FogMode fogMode = FogMode.Exponential;

    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private float startDensity;
    private float targetDensity;

    public void OnMap()
    {
        if (enableFog)
            return;

        startDensity = RenderSettings.fogDensity;
        targetDensity = fogDensityOn;
        isTransitioning = true;
        transitionTimer = 0f;
    }

    public void OnMapOff()
    {
        if (!enableFog)
            return;

        startDensity = RenderSettings.fogDensity;
        targetDensity = fogDensityOff;
        isTransitioning = true;
        transitionTimer = 0f;
    }

    private void Update()
    {
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);
            float currentDensity = Mathf.Lerp(startDensity, targetDensity, t);
            RenderSettings.fogDensity = currentDensity;

            if (t >= 1f)
            {
                isTransitioning = false;
                enableFog = targetDensity > 0f;
                UpdateFog();
            }
        }
    }

    private void UpdateFog()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = enableFog ? fogDensityOn : fogDensityOff;
        RenderSettings.fogMode = fogMode;
    }
}
