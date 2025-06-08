using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class CanvasTransition
{
    [Header("Text Settings")]
    public TMP_Text targetText;
    public string requiredEndText = "";
    public bool checkTypingCompletion = true;
    public float checkInterval = 0.1f;

    [Header("Disable Settings")]
    public GameObject canvasToDisable;
    public float disableDelay = 0f;
    public bool fadeOutOnDisable = false;
    public float fadeOutDuration = 0.5f;
    public bool destroyAfterDisable = false;

    [Header("Enable Settings")]
    public GameObject canvasToEnable;
    public float enableDelay = 0f;
    public bool fadeInOnEnable = false;
    public float fadeInDuration = 0.5f;

    [Header("Events")]
    public UnityEvent onTransitionStart;
    public UnityEvent onTransitionComplete;
}

public class TogglePreLoader : MonoBehaviour
{
    [Header("Transition Settings")]
    public List<CanvasTransition> transitions = new List<CanvasTransition>();

    [Header("Debug Settings")]
    public bool verboseLogging = true;

    private Dictionary<TMP_Text, Coroutine> activeCheckRoutines = new Dictionary<TMP_Text, Coroutine>();

    void Awake()
    {
        InitializeAllTransitions();
    }

    void InitializeAllTransitions()
    {
        if (verboseLogging) Debug.Log("[TogglePreLoader] Initializing all transitions");

        foreach (var transition in transitions)
        {
            if (transition.canvasToDisable != null)
            {
                if (verboseLogging) Debug.Log($"[Init] Found canvas to disable: {transition.canvasToDisable.name}");
            }
            else
            {
                Debug.LogWarning("[Init] Missing canvas to disable reference");
            }

            if (transition.canvasToEnable != null)
            {
                transition.canvasToEnable.SetActive(false);
                if (verboseLogging) Debug.Log($"[Init] Disabled canvas to enable: {transition.canvasToEnable.name}");
            }
            else
            {
                Debug.LogWarning("[Init] Missing canvas to enable reference");
            }

            if (transition.targetText == null)
            {
                Debug.LogError("[Init] Missing target text reference");
            }
        }
    }

    void OnEnable()
    {
        if (verboseLogging) Debug.Log("[TogglePreLoader] Enabled - starting all check routines");

        foreach (var transition in transitions)
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
        if (verboseLogging) Debug.Log("[TogglePreLoader] Disabled - stopping all check routines");

        foreach (var routine in activeCheckRoutines)
        {
            if (routine.Value != null)
            {
                StopCoroutine(routine.Value);
            }
        }
        activeCheckRoutines.Clear();
    }

    IEnumerator CheckTextStateRoutine(CanvasTransition transition)
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

            // Check if text ends with required string
            if (!string.IsNullOrEmpty(transition.requiredEndText))
            {
                shouldTransition = transition.targetText.text.EndsWith(transition.requiredEndText);
                if (verboseLogging && !shouldTransition)
                    Debug.Log($"[Check] Text '{transition.targetText.name}' doesn't end with '{transition.requiredEndText}'");
            }

            // Additional check for typewriter effect if needed
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

    void ExecuteTransition(CanvasTransition transition)
    {
        if (verboseLogging) Debug.Log($"[Transition] Starting transition for text '{transition.targetText.name}'");

        transition.onTransitionStart?.Invoke();
        StartCoroutine(TransitionRoutine(transition));
    }

    IEnumerator TransitionRoutine(CanvasTransition transition)
    {
        // Handle canvas to disable
        if (transition.canvasToDisable != null)
        {
            if (transition.disableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.disableDelay}s before disabling canvas");
                yield return new WaitForSeconds(transition.disableDelay);
            }

            if (transition.fadeOutOnDisable && transition.fadeOutDuration > 0)
            {
                yield return StartCoroutine(FadeCanvas(transition.canvasToDisable, false, transition.fadeOutDuration));
            }
            else
            {
                transition.canvasToDisable.SetActive(false);
                if (verboseLogging) Debug.Log($"[Transition] Disabled canvas: {transition.canvasToDisable.name}");
            }

            if (transition.destroyAfterDisable)
            {
                Destroy(transition.canvasToDisable);
                if (verboseLogging) Debug.Log($"[Transition] Destroyed canvas: {transition.canvasToDisable.name}");
            }
        }

        // Handle canvas to enable
        if (transition.canvasToEnable != null)
        {
            if (transition.enableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.enableDelay}s before enabling canvas");
                yield return new WaitForSeconds(transition.enableDelay);
            }

            if (transition.fadeInOnEnable && transition.fadeInDuration > 0)
            {
                yield return StartCoroutine(FadeCanvas(transition.canvasToEnable, true, transition.fadeInDuration));
            }
            else
            {
                transition.canvasToEnable.SetActive(true);
                if (verboseLogging) Debug.Log($"[Transition] Enabled canvas: {transition.canvasToEnable.name}");
            }
        }

        transition.onTransitionComplete?.Invoke();
        if (verboseLogging) Debug.Log("[Transition] Transition complete");
    }

    IEnumerator FadeCanvas(GameObject canvas, bool fadeIn, float duration)
    {
        CanvasGroup group = canvas.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = canvas.AddComponent<CanvasGroup>();
            if (verboseLogging) Debug.Log($"[Fade] Added CanvasGroup to {canvas.name}");
        }

        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        group.alpha = startAlpha;

        if (fadeIn) canvas.SetActive(true);
        if (verboseLogging) Debug.Log($"[Fade] Starting {(fadeIn ? "fade in" : "fade out")} for {canvas.name} over {duration}s");

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }

        group.alpha = endAlpha;

        if (!fadeIn) canvas.SetActive(false);
        if (verboseLogging) Debug.Log($"[Fade] Completed {(fadeIn ? "fade in" : "fade out")} for {canvas.name}");
    }
}