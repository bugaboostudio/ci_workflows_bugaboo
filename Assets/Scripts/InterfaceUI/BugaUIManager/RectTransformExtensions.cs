using UnityEngine;

public static class RectTransformExtensions
{
    public static void CenterOnScreen(this RectTransform rectTransform, Canvas canvas)
    {
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }
}

