using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ImageTransition
{
    [Header("Text Settings")]
    public TMP_Text targetText;
    public string requiredEndText = "";
    public bool checkTypingCompletion = true;
    public float checkInterval = 0.1f;

    [Header("Disable Image Settings")]
    public Image imageToDisable;
    public float disableDelay = 0f;
    public bool fadeOutOnDisable = false;
    public float fadeOutDuration = 0.5f;
    public bool destroyAfterDisable = false;

    [Header("Enable Image Settings")]
    public Image imageToEnable;
    public float enableDelay = 0f;
    public bool fadeInOnEnable = false;
    public float fadeInDuration = 0.5f;

    [Header("Events")]
    public UnityEvent onTransitionStart;
    public UnityEvent onTransitionComplete;
}

public class ToggleImage : MonoBehaviour
{
    [Header("Image Transitions")]
    public List<ImageTransition> imageTransitions = new List<ImageTransition>();

    [Header("Debug Settings")]
    public bool verboseLogging = true;

    private Dictionary<TMP_Text, Coroutine> activeCheckRoutines = new Dictionary<TMP_Text, Coroutine>();

    void Awake()
    {
        InitializeAllTransitions();
    }

    void InitializeAllTransitions()
    {
        if (verboseLogging) Debug.Log("[ToggleImage] Initializing all image transitions");

        foreach (var transition in imageTransitions)
        {
            // Initialize image to disable
            if (transition.imageToDisable != null)
            {
                if (verboseLogging) Debug.Log($"[Init] Found image to disable: {transition.imageToDisable.name}");
            }
            else
            {
                Debug.LogWarning("[Init] Missing image to disable reference");
            }

            // Initialize image to enable
            if (transition.imageToEnable != null)
            {
                SetImageActiveState(transition.imageToEnable, false);
                if (verboseLogging) Debug.Log($"[Init] Disabled image: {transition.imageToEnable.name}");
            }
            else
            {
                Debug.LogWarning("[Init] Missing image to enable reference");
            }

            if (transition.targetText == null)
            {
                Debug.LogError("[Init] Missing target text reference");
            }
        }
    }

    void OnEnable()
    {
        if (verboseLogging) Debug.Log("[ToggleImage] Enabled - starting all check routines");

        foreach (var transition in imageTransitions)
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
        if (verboseLogging) Debug.Log("[ToggleImage] Disabled - stopping all check routines");

        foreach (var routine in activeCheckRoutines)
        {
            if (routine.Value != null)
            {
                StopCoroutine(routine.Value);
            }
        }
        activeCheckRoutines.Clear();
    }

    IEnumerator CheckTextStateRoutine(ImageTransition transition)
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

    void ExecuteTransition(ImageTransition transition)
    {
        if (verboseLogging) Debug.Log($"[Transition] Starting transition for text '{transition.targetText.name}'");

        transition.onTransitionStart?.Invoke();
        StartCoroutine(TransitionRoutine(transition));
    }

    IEnumerator TransitionRoutine(ImageTransition transition)
    {
        // Handle image to disable
        if (transition.imageToDisable != null)
        {
            if (transition.disableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.disableDelay}s before disabling image");
                yield return new WaitForSeconds(transition.disableDelay);
            }

            if (transition.fadeOutOnDisable && transition.fadeOutDuration > 0)
            {
                yield return StartCoroutine(FadeImage(transition.imageToDisable, false, transition.fadeOutDuration));
            }
            else
            {
                SetImageActiveState(transition.imageToDisable, false);
                if (verboseLogging) Debug.Log($"[Transition] Disabled image: {transition.imageToDisable.name}");
            }

            if (transition.destroyAfterDisable)
            {
                Destroy(transition.imageToDisable.gameObject);
                if (verboseLogging) Debug.Log($"[Transition] Destroyed image: {transition.imageToDisable.name}");
            }
        }

        // Handle image to enable
        if (transition.imageToEnable != null)
        {
            if (transition.enableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.enableDelay}s before enabling image");
                yield return new WaitForSeconds(transition.enableDelay);
            }

            if (transition.fadeInOnEnable && transition.fadeInDuration > 0)
            {
                yield return StartCoroutine(FadeImage(transition.imageToEnable, true, transition.fadeInDuration));
            }
            else
            {
                SetImageActiveState(transition.imageToEnable, true);
                if (verboseLogging) Debug.Log($"[Transition] Enabled image: {transition.imageToEnable.name}");
            }
        }

        transition.onTransitionComplete?.Invoke();
        if (verboseLogging) Debug.Log("[Transition] Transition complete");
    }

    IEnumerator FadeImage(Image image, bool fadeIn, float duration)
    {
        CanvasGroup group = image.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = image.gameObject.AddComponent<CanvasGroup>();
            if (verboseLogging) Debug.Log($"[Fade] Added CanvasGroup to {image.name}");
        }

        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        group.alpha = startAlpha;

        if (fadeIn) SetImageActiveState(image, true);
        if (verboseLogging) Debug.Log($"[Fade] Starting {(fadeIn ? "fade in" : "fade out")} for {image.name} over {duration}s");

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }

        group.alpha = endAlpha;

        if (!fadeIn) SetImageActiveState(image, false);
        if (verboseLogging) Debug.Log($"[Fade] Completed {(fadeIn ? "fade in" : "fade out")} for {image.name}");
    }

    void SetImageActiveState(Image image, bool active)
    {
        image.gameObject.SetActive(active);
    }
}