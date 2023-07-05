using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnableDisableAnimation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float animationDuration = 1f;

    private bool isAnimating;

    public UnityEvent OnContentInEvent;
    public UnityEvent OnContentOffEvent;

    public void OnContentIn()
    {
        StartCoroutine(EnableCoroutine());
    }

    public void OnContentOff()
    {
        StartCoroutine(DisableCoroutine());
    }

    private System.Collections.IEnumerator EnableCoroutine()
    {
        if (canvasGroup != null && !isAnimating)
        {
            isAnimating = true;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            float endAlpha = 1f;

            while (elapsedTime < animationDuration)
            {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / animationDuration);
                canvasGroup.alpha = alpha;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
            canvasGroup.interactable = true;
            isAnimating = false;

            // Chamar o evento OnContentInEvent
            OnContentInEvent.Invoke();
        }
    }

    private System.Collections.IEnumerator DisableCoroutine()
    {
        if (canvasGroup != null && !isAnimating)
        {
            isAnimating = true;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            float endAlpha = 0f;

            while (elapsedTime < animationDuration)
            {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / animationDuration);
                canvasGroup.alpha = alpha;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
            isAnimating = false;

            // Chamar o evento OnContentOffEvent
            OnContentOffEvent.Invoke();
        }
    }
}
