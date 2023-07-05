using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BugaScreen : MonoBehaviour
{
    public bool showOnAwake = false;
    public float showTransitionTime = 0.5f; // Tempo de transição para o show em segundos
    public float hideTransitionTime = 0.5f; // Tempo de transição para o hide em segundos
    public float showDelay = 0f; // Atraso antes de iniciar a transição para o show
    public float hideDelay = 0f; // Atraso antes de iniciar a transição para o hide
    public bool useShowAnimation = false; // Flag para indicar se a transição de Show utiliza um clip de animação
    public AnimationClip showClip; // O clip de animação para a transição de Show (arraste no editor)
    public bool useHideAnimation = false; // Flag para indicar se a transição de Hide utiliza um clip de animação
    public AnimationClip hideClip; // O clip de animação para a transição de Hide (arraste no editor)

    [SerializeField] private bool toShow = false;
    [SerializeField] private bool toHide = false;

    public UnityEvent onShow;
    public UnityEvent onHide;
    public UnityEvent onCompleteShow;
    public UnityEvent onCompleteHide;

    private CanvasGroup canvasGroup;
    private Coroutine transitionCoroutine;
    private Animator animator;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();

        if (showOnAwake)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        StartCoroutine(ShowWithDelay());
    }

    public void Hide()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        StartCoroutine(HideWithDelay());
    }

    private IEnumerator TransitionCanvasGroup(CanvasGroup canvasGroup, float startValue, float endValue, float transitionDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float alpha = Mathf.Lerp(startValue, endValue, t);

            canvasGroup.alpha = alpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endValue;

        if (!toShow)
        {
            gameObject.SetActive(false);
        }

        transitionCoroutine = null;

        if (endValue == 1f)
        {
            onCompleteShow.Invoke();
        }
        else
        {
            onCompleteHide.Invoke();
        }
    }

    private void PlayTransitionAnimation(AnimationClip clip, bool show)
    {
        animator.enabled = true;
        animator.Play(clip.name, 0, show ? 0f : 1f);
    }

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSeconds(showDelay);

        if (useShowAnimation && showClip != null)
        {
            PlayTransitionAnimation(showClip, true);
            transitionCoroutine = StartCoroutine(TransitionCanvasGroup(canvasGroup, 0f, 1f, showClip.length));
        }
        else
        {
            transitionCoroutine = StartCoroutine(TransitionCanvasGroup(canvasGroup, 0f, 1f, showTransitionTime));
        }

        onShow.Invoke();
    }

    private IEnumerator HideWithDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        if (useHideAnimation && hideClip != null)
        {
            PlayTransitionAnimation(hideClip, true);
            transitionCoroutine = StartCoroutine(TransitionCanvasGroup(canvasGroup, 1f, 0f, hideClip.length));
        }
        else
        {
            transitionCoroutine = StartCoroutine(TransitionCanvasGroup(canvasGroup, 1f, 0f, hideTransitionTime));
        }

        onHide.Invoke();
    }

    public void ToShow(bool show)
    {
        toShow = show;

        if (toShow)
        {
            Show();
        }
        else
        {
            Hide();
        }

        Debug.Log($"ToShow received for object: {gameObject.name}. Show: {toShow}");
    }

    public void ToHide(bool hide)
    {
        toHide = hide;

        if (toHide)
        {
            Hide();
        }
        else
        {
            Show();
        }

        Debug.Log($"ToHide received for object: {gameObject.name}. Hide: {toHide}");
    }
}
