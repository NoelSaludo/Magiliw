using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ButtonPreLoader : MonoBehaviour
{
    [System.Serializable]
    public class CanvasTransition
    {
        [Header("Canvas Settings")]
        public GameObject canvasToDisable;
        public GameObject canvasToEnable;
        public float disableDelay = 0f;
        public float enableDelay = 0f;
        
        [Header("Fade Settings")]
        public bool fadeOutOnDisable = false;
        public float fadeOutDuration = 0.5f;
        public bool fadeInOnEnable = false;
        public float fadeInDuration = 0.5f;
        
        [Header("Destruction Settings")]
        public bool destroyAfterDisable = false;
    }

    [Header("Button Reference")]
    public Button targetButton;
    
    [Header("Canvas Transitions")]
    public List<CanvasTransition> transitions = new List<CanvasTransition>();
    
    [Header("Debug Settings")]
    public bool verboseLogging = true;

    void Awake()
    {
        // Get the button if not assigned
        if (targetButton == null)
        {
            targetButton = GetComponent<Button>();
        }

        // Set up click listener
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(OnButtonClick);
            if (verboseLogging) Debug.Log($"[ButtonPreLoader] Initialized on button: {targetButton.name}");
        }
        else
        {
            Debug.LogError("[ButtonPreLoader] No button component found!");
        }
    }

    void OnDestroy()
    {
        // Clean up listener
        if (targetButton != null)
        {
            targetButton.onClick.RemoveListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        if (verboseLogging) Debug.Log($"[ButtonPreLoader] Button clicked: {targetButton.name}");
        StartCoroutine(ExecuteAllTransitions());
    }

    IEnumerator ExecuteAllTransitions()
    {
        foreach (var transition in transitions)
        {
            yield return StartCoroutine(ProcessTransition(transition));
        }
    }

    IEnumerator ProcessTransition(CanvasTransition transition)
    {
        // Handle canvas to disable
        if (transition.canvasToDisable != null)
        {
            if (transition.disableDelay > 0)
            {
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.disableDelay}s before disabling {transition.canvasToDisable.name}");
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
                if (verboseLogging) Debug.Log($"[Transition] Waiting {transition.enableDelay}s before enabling {transition.canvasToEnable.name}");
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