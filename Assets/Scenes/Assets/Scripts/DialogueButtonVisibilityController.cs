using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class CanvasToggleController : MonoBehaviour
{
    [System.Serializable]
    public class CanvasAction
    {
        public Canvas canvas;
        public float delay = 0f;
        public bool useFade = false;
        public float fadeDuration = 0.5f;
    }

    [Header("Button Settings")]
    public bool hideButtonAfterClick = true;

    [Header("Canvas Control")]
    public List<CanvasAction> canvasesToHide = new List<CanvasAction>();
    public List<CanvasAction> canvasesToLoad = new List<CanvasAction>();

    [Header("Optional Fade")]
    public CanvasGroup buttonFadeGroup;
    public float buttonFadeOutDuration = 0.3f;

    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        if (hideButtonAfterClick)
        {
            if (buttonFadeGroup != null && buttonFadeOutDuration > 0)
                StartCoroutine(FadeOutButton());
            else
                gameObject.SetActive(false);
        }

        // Process hide/show actions
        ExecuteCanvasActions(canvasesToHide, false);
        ExecuteCanvasActions(canvasesToLoad, true);
    }

    private void ExecuteCanvasActions(List<CanvasAction> actions, bool targetState)
    {
        foreach (var action in actions)
        {
            if (action.canvas == null) continue;

            if (action.delay <= 0 && !action.useFade)
            {
                // Instant toggle
                action.canvas.enabled = targetState;
            }
            else
            {
                StartCoroutine(ToggleCanvasWithEffects(
                    action.canvas, 
                    targetState, 
                    action.delay, 
                    action.useFade ? action.fadeDuration : 0
                ));
            }
        }
    }

    private IEnumerator ToggleCanvasWithEffects(Canvas canvas, bool targetState, float delay, float fadeDuration)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        CanvasGroup group = canvas.GetComponent<CanvasGroup>();
        if (group == null && fadeDuration > 0)
            group = canvas.gameObject.AddComponent<CanvasGroup>();

        if (fadeDuration > 0 && group != null)
        {
            float startAlpha = targetState ? 0 : 1;
            float endAlpha = targetState ? 1 : 0;

            canvas.enabled = true;
            group.alpha = startAlpha;

            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
                yield return null;
            }

            if (!targetState) canvas.enabled = false;
        }
        else
        {
            canvas.enabled = targetState;
        }
    }

    private IEnumerator FadeOutButton()
    {
        if (buttonFadeGroup == null) yield break;

        float startAlpha = buttonFadeGroup.alpha;
        float timer = 0;

        while (timer < buttonFadeOutDuration)
        {
            timer += Time.deltaTime;
            buttonFadeGroup.alpha = Mathf.Lerp(startAlpha, 0, timer / buttonFadeOutDuration);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    // Editor utility
    public void AddNewHideAction() => canvasesToHide.Add(new CanvasAction());
    public void AddNewLoadAction() => canvasesToLoad.Add(new CanvasAction());
}