using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(Button))]
public class TypewriterCanvasToggleController : MonoBehaviour
{
    [System.Serializable]
    public class CanvasAction
    {
        public GameObject canvasObject;
        public float delay = 0f;
        public bool useFade = false;
        public float fadeDuration = 0.5f;
    }

    [Header("Typewriter Settings")]
    public DialogueTypewriter typewriter;
    public bool showButtonAfterText = true;
    public float checkInterval = 0.1f;

    [Header("Canvas Control")]
    public List<CanvasAction> canvasesToDisable = new List<CanvasAction>();
    public List<CanvasAction> canvasesToEnable = new List<CanvasAction>();

    private Button _button;
    private Coroutine _checkRoutine;

    void Awake()
    {
        _button = GetComponent<Button>();
        gameObject.SetActive(false);
        Debug.Log("[Init] Button initialized and set inactive");
        
        InitializeCanvases();
    }

    void InitializeCanvases()
    {
        foreach (var action in canvasesToDisable)
        {
            if (action.canvasObject != null)
            {
                action.canvasObject.SetActive(false);
                Debug.Log($"[Canvas] Disabled {action.canvasObject.name} on init");
            }
        }
        
        foreach (var action in canvasesToEnable)
        {
            if (action.canvasObject != null)
            {
                action.canvasObject.SetActive(false);
                Debug.Log($"[Canvas] Disabled {action.canvasObject.name} on init");
            }
        }
    }

    void OnEnable()
    {
        if (typewriter != null)
        {
            Debug.Log($"[Typewriter] Connected to typewriter with {typewriter.TotalSegments} segments");
            _checkRoutine = StartCoroutine(CheckTypewriterState());
        }
        else
        {
            Debug.LogError("[Typewriter] No typewriter reference assigned!");
        }
        
        _button.onClick.AddListener(OnButtonClick);
    }

    void OnDisable()
    {
        if (_checkRoutine != null)
            StopCoroutine(_checkRoutine);
        
        _button.onClick.RemoveListener(OnButtonClick);
    }

    IEnumerator CheckTypewriterState()
    {
        while (true)
        {
            Debug.Log($"[State Check] Current: {typewriter.CurrentSegmentIndex} | Total: {typewriter.TotalSegments} | Typing: {typewriter.IsTyping}");
            
            if (typewriter.CurrentSegmentIndex >= typewriter.TotalSegments - 1 && !typewriter.IsTyping)
            {
                ShowButton();
                yield break; // Stop checking once shown
            }
            
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void ShowButton()
    {
        if (showButtonAfterText)
        {
            gameObject.SetActive(true);
            Debug.Log($"[Button] Activated at segment {typewriter.CurrentSegmentIndex}/{typewriter.TotalSegments}");
        }
    }

    public void OnButtonClick()
    {
        Debug.Log("[Button] Click detected - executing canvas actions");
        gameObject.SetActive(false);
        
        ExecuteCanvasActions(canvasesToDisable, false);
        ExecuteCanvasActions(canvasesToEnable, true);
    }

    private void ExecuteCanvasActions(List<CanvasAction> actions, bool targetState)
    {
        foreach (var action in actions)
        {
            if (action.canvasObject == null)
            {
                Debug.LogWarning("[Action] Null canvas object in list!");
                continue;
            }

            if (action.delay <= 0 && !action.useFade)
            {
                action.canvasObject.SetActive(targetState);
                Debug.Log($"[Action] {action.canvasObject.name} set to {targetState} (instant)");
            }
            else
            {
                StartCoroutine(ToggleCanvasWithEffects(
                    action.canvasObject,
                    targetState,
                    action.delay,
                    action.useFade ? action.fadeDuration : 0
                ));
            }
        }
    }

    private IEnumerator ToggleCanvasWithEffects(GameObject canvasObj, bool targetState, float delay, float fadeDuration)
    {
        if (delay > 0)
        {
            Debug.Log($"[Action] Queued {canvasObj.name} toggle in {delay}s");
            yield return new WaitForSeconds(delay);
        }

        if (fadeDuration > 0)
        {
            CanvasGroup group = canvasObj.GetComponent<CanvasGroup>() ?? canvasObj.AddComponent<CanvasGroup>();
            float startAlpha = targetState ? 0 : 1;
            float endAlpha = targetState ? 1 : 0;

            canvasObj.SetActive(true);
            group.alpha = startAlpha;

            Debug.Log($"[Action] Fading {canvasObj.name} {(targetState ? "in" : "out")} over {fadeDuration}s");

            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
                yield return null;
            }

            if (!targetState) canvasObj.SetActive(false);
            Debug.Log($"[Action] Finished fading {canvasObj.name}");
        }
        else
        {
            canvasObj.SetActive(targetState);
            Debug.Log($"[Action] {canvasObj.name} set to {targetState} (delayed)");
        }
    }
}