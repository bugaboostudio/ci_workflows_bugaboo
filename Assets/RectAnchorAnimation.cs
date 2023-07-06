using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;

public class RectAnchorAnimation : MonoBehaviour
{
    [SerializeField]
    float Duration;
    RectTransform Rect;
    TweenerCore<Vector2, Vector2, VectorOptions> Tween;

    [SerializeField]
    Vector2 DefaultMin, DefaultMax;

    private void Awake()
    {
        Rect = (RectTransform)transform;
    }

    public void ResetAnchor()
    {
        Rect.DOAnchorMin(DefaultMin, Duration);
        Rect.DOAnchorMax(DefaultMax, Duration);
    }

    public void SetMaxAnchorY(float Val) =>CallTween( Rect.DOAnchorMax(new Vector2(Rect.anchorMax.x, Val), Duration)); 
    public void SetMinAnchorY(float Val) =>CallTween( Rect.DOAnchorMin(new Vector2(Rect.anchorMin.x, Val), Duration)); 
    public void SetMaxAnchorX(float Val) =>CallTween( Rect.DOAnchorMax(new Vector2(Val,Rect.anchorMax.y), Duration)); 
    public void SetMinAnchorX(float Val) =>CallTween( Rect.DOAnchorMin(new Vector2(Val, Rect.anchorMin.y), Duration)); 
    
    void CallTween(TweenerCore<Vector2, Vector2, VectorOptions> Target)
    {
        Tween?.Kill();
        Tween = Target;
    }    
    

}
