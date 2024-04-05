
using System.Collections;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private float fadeInDuration = default;
    [SerializeField] private float fadeOutDuration = default;

    public readonly EventVariable<LoadingScreenUI, bool> showUI;
    public bool isFading => fadeInRoutine != null || fadeOutRoutine != null;

    private Coroutine fadeInRoutine;
    private Coroutine fadeOutRoutine;


    private LoadingScreenUI()
    {
        showUI = new EventVariable<LoadingScreenUI, bool>(this, false);
    }

    private void Awake()
    {
        showUI.onValueChange += OnValueChanged_ShowUI;
    }

    private void OnValueChanged_ShowUI(bool oldValue, bool newValue)
    {
        if(newValue)
        {
            if (fadeInRoutine == null)
                fadeInRoutine = StartCoroutine(Routine_FadeIn());
        }
        else
        {
            if (fadeOutRoutine == null)
                fadeOutRoutine = StartCoroutine(Routine_FadeOut());
        }
    }

    private IEnumerator Routine_FadeIn()
    {
        while (fadeOutRoutine != null)
            yield return null;

        float timeInState = Time.unscaledDeltaTime;
        while(timeInState < fadeInDuration)
        {
            canvasGroup.alpha = timeInState / fadeInDuration;
            yield return null;
            timeInState += Time.unscaledDeltaTime;
        }

        canvasGroup.alpha = 1;
        fadeInRoutine = null;
    }

    private IEnumerator Routine_FadeOut()
    {
        while (fadeInRoutine != null)
            yield return null;

        float timeInState = Time.unscaledDeltaTime;
        while (timeInState < fadeOutDuration)
        {
            canvasGroup.alpha = 1 - (timeInState / fadeOutDuration);
            yield return null;
            timeInState += Time.unscaledDeltaTime;
        }

        canvasGroup.alpha = 0;
        fadeOutRoutine = null;
    }

    private void OnDestroy()
    {
        showUI.onValueChange -= OnValueChanged_ShowUI;
    }
}