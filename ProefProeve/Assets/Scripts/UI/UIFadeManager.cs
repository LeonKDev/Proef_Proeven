using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIFadeManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float fadeDelay = 0.1f;

    private void Awake()
    {
        // Make sure all UI elements have CanvasGroup components
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                EnsureCanvasGroup(child.gameObject);
            }
        }
    }

    private CanvasGroup EnsureCanvasGroup(GameObject uiElement)
    {
        CanvasGroup group = uiElement.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = uiElement.AddComponent<CanvasGroup>();
        }
        return group;
    }

    public void ShowUI(GameObject uiElement, bool instant = false)
    {
        if (uiElement == null) return;

        CanvasGroup canvasGroup = EnsureCanvasGroup(uiElement);
        uiElement.SetActive(true);

        if (instant)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            StopAllCoroutinesForUI(uiElement);
            StartCoroutine(FadeIn(canvasGroup));
        }
    }

    public void HideUI(GameObject uiElement, bool instant = false)
    {
        if (uiElement == null) return;

        CanvasGroup canvasGroup = EnsureCanvasGroup(uiElement);

        if (instant)
        {
            canvasGroup.alpha = 0;
            uiElement.SetActive(false);
        }
        else
        {
            StopAllCoroutinesForUI(uiElement);
            StartCoroutine(FadeOut(canvasGroup));
        }
    }

    private void StopAllCoroutinesForUI(GameObject uiElement)
    {
        // Stop any existing fade coroutines for this UI element
        MonoBehaviour[] scripts = uiElement.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
            {
                StopCoroutine(FadeIn(uiElement.GetComponent<CanvasGroup>()));
                StopCoroutine(FadeOut(uiElement.GetComponent<CanvasGroup>()));
            }
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(fadeDelay);

        float elapsedTime = 0;
        float startValue = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startValue, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(fadeDelay);

        float elapsedTime = 0;
        float startValue = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startValue, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);
    }
}