using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ButtonTransition
{
    [Header("Text Settings")]
    public TMP_Text targetText;
    public string requiredEndText = "";
    public bool checkTypingCompletion = true;
    public float checkInterval = 0.1f;

    [Header("Disable Button Settings")]
    public Button buttonToDisable;
    public float disableDelay = 0f;
    public bool fadeOutOnDisable = false;
    public float fadeOutDuration = 0.5f;
    public bool destroyAfterDisable = false;

    [Header("Enable Button Settings")]
    public Button buttonToEnable;
    public float enableDelay = 0f;
    public bool fadeInOnEnable = false;
    public float fadeInDuration = 0.5f;

    [Header("Events")]
    public UnityEvent onTransitionStart;
    public UnityEvent onTransitionComplete;
}

public class ToggleButton : MonoBehaviour  // Changed to match filename
{
    [Header("Button Transitions")]
    public List<ButtonTransition> buttonTransitions = new List<ButtonTransition>();

    [Header("Debug Settings")]
    public bool verboseLogging = true;

    private Dictionary<TMP_Text, Coroutine> activeCheckRoutines = new Dictionary<TMP_Text, Coroutine>();

    void Awake()
    {
        InitializeAllTransitions();
    }

    void InitializeAllTransitions()
    {
        if (verboseLogging) Debug.Log("[ToggleButton] Initializing all button transitions");

        foreach (var transition in buttonTransitions)
        {
            if (transition.buttonToDisable != null)
            {
                if (verboseLogging) Debug.Log($"[Init] Found button to disable: {transition.buttonToDisable.name}");
            }
            else
            {
                Debug.LogWarning("[Init] Missing button to disable reference");
            }

            if (transition.buttonToEnable != null)
            {
                SetButtonActiveState(transition.buttonToEnable, false);
                if (verboseLogging) Debug.Log($"[Init] Disabled button: {transition.buttonToEnable.name}");
            }
            else
            {
                Debug.LogWarning("[Init] Missing button to enable reference");
            }

            if (transition.targetText == null)
            {
                Debug.LogError("[Init] Missing target text reference");
            }
        }
    }

    void OnEnable()
    {
        if (verboseLogging) Debug.Log("[ToggleButton] Enabled - starting all check routines");

        foreach (var transition in buttonTransitions)
        {
            if (transition.targetText != null)
            {
                if (activeCheckRoutines.ContainsKey(transition.targetText))
                {
                    StopCoroutine(activeCheckRoutines[transition.targetText]);
                }

                var routine = StartCoroutine(CheckTextStateRoutine(transition));
                activeCheckRoutines[transition.targetText] = routine;
            }
        }
    }

    void OnDisable()
    {
        if (verboseLogging) Debug.Log("[ToggleButton] Disabled - stopping all check routines");

        foreach (var routine in activeCheckRoutines)
        {
            if (routine.Value != null)
            {
                StopCoroutine(routine.Value);
            }
        }
        activeCheckRoutines.Clear();
    }

    IEnumerator CheckTextStateRoutine(ButtonTransition transition)
    {
        if (transition.targetText == null)
        {
            Debug.LogError("[Check] Invalid transition - missing target text");
            yield break;
        }

        if (verboseLogging) Debug.Log($"[Check] Starting check routine for text '{transition.targetText.name}'");

        while (true)
        {
            bool shouldTransition = true;

            if (!string.IsNullOrEmpty(transition.requiredEndText))
            {
                shouldTransition = transition.targetText.text.EndsWith(transition.requiredEndText);
                if (verboseLogging && !shouldTransition)
                    Debug.Log($"[Check] Text '{transition.targetText.name}' doesn't end with '{transition.requiredEndText}'");
            }

            if (shouldTransition && transition.checkTypingCompletion)
            {
                var typewriter = transition.targetText.GetComponent<DialogueTypewriter>();
                if (typewriter != null)
                {
                    shouldTransition = typewriter.CurrentSegmentIndex >= typewriter.TotalSegments - 1 && !typewriter.IsTyping;
                    if (verboseLogging)
                        Debug.Log($"[Check] Typewriter state: {typewriter.CurrentSegmentIndex}/{typewriter.TotalSegments} | Typing: {typewriter.IsTyping}");
                }
            }

            if (shouldTransition)
            {
                ExecuteTransition(transition);
                yield break;
            }

            yield return new WaitForSeconds(transition.checkInterval);
        }
    }

    void ExecuteTransition(ButtonTransition transition)
    {
        if (verboseLogging) Debug.Log($"[Transition] Starting transition for text '{transition.targetText.name}'");

        transition.onTransitionStart?.Invoke();
        StartCoroutine(TransitionRoutine(transition));
    }

    IEnumerator TransitionRoutine(ButtonTransition transition)
    {
        if (transition.buttonToDisable != null)
        {
            if (transition.disableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.disableDelay}s before disabling button");
                yield return new WaitForSeconds(transition.disableDelay);
            }

            if (transition.fadeOutOnDisable && transition.fadeOutDuration > 0)
            {
                yield return StartCoroutine(FadeButton(transition.buttonToDisable, false, transition.fadeOutDuration));
            }
            else
            {
                SetButtonActiveState(transition.buttonToDisable, false);
                if (verboseLogging) Debug.Log($"[Transition] Disabled button: {transition.buttonToDisable.name}");
            }

            if (transition.destroyAfterDisable)
            {
                Destroy(transition.buttonToDisable.gameObject);
                if (verboseLogging) Debug.Log($"[Transition] Destroyed button: {transition.buttonToDisable.name}");
            }
        }

        if (transition.buttonToEnable != null)
        {
            if (transition.enableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.enableDelay}s before enabling button");
                yield return new WaitForSeconds(transition.enableDelay);
            }

            if (transition.fadeInOnEnable && transition.fadeInDuration > 0)
            {
                yield return StartCoroutine(FadeButton(transition.buttonToEnable, true, transition.fadeInDuration));
            }
            else
            {
                SetButtonActiveState(transition.buttonToEnable, true);
                if (verboseLogging) Debug.Log($"[Transition] Enabled button: {transition.buttonToEnable.name}");
            }
        }

        transition.onTransitionComplete?.Invoke();
        if (verboseLogging) Debug.Log("[Transition] Transition complete");
    }

    IEnumerator FadeButton(Button button, bool fadeIn, float duration)
    {
        CanvasGroup group = button.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = button.gameObject.AddComponent<CanvasGroup>();
            if (verboseLogging) Debug.Log($"[Fade] Added CanvasGroup to {button.name}");
        }

        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        group.alpha = startAlpha;
        button.interactable = fadeIn;

        if (fadeIn) SetButtonActiveState(button, true);
        if (verboseLogging) Debug.Log($"[Fade] Starting {(fadeIn ? "fade in" : "fade out")} for {button.name} over {duration}s");

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }

        group.alpha = endAlpha;

        if (!fadeIn) SetButtonActiveState(button, false);
        if (verboseLogging) Debug.Log($"[Fade] Completed {(fadeIn ? "fade in" : "fade out")} for {button.name}");
    }

    void SetButtonActiveState(Button button, bool active)
    {
        button.gameObject.SetActive(active);
        button.interactable = active;
    }
}