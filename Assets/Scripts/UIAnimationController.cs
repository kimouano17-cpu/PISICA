using System.Collections;
using UnityEngine;

public class UIAnimationController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Coroutine currentFade;

    private void Awake()
    {
        // Automatically grab the Canvas Group component
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Call this to show the panel
    public void ShowPanel(float duration = 0.3f)
    {
        if (currentFade != null) StopCoroutine(currentFade);

        // Make it interactable immediately when fading in
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        currentFade = StartCoroutine(FadeRoutine(1f, duration));
    }

    // Call this to hide the panel
    public void HidePanel(float duration = 0.3f)
    {
        if (currentFade != null) StopCoroutine(currentFade);

        // Disable interactions immediately so player can't click hidden buttons
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        currentFade = StartCoroutine(FadeRoutine(0f, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            // Smoothly interpolate the alpha value
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
