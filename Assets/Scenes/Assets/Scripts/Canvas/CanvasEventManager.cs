using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EventAudioAction
{
    [Tooltip("AudioSource to play")]
    public AudioSource audioToPlay;

    [Tooltip("AudioSource to stop")]
    public AudioSource audioToStop;

    public void Execute()
    {
        if (audioToStop != null && audioToStop.isPlaying)
        {
            Debug.Log($"[Audio Action] Stopping audio: {audioToStop.name}");
            audioToStop.Stop();
        }

        if (audioToPlay != null && !audioToPlay.isPlaying)
        {
            Debug.Log($"[Audio Action] Playing audio: {audioToPlay.name}");
            audioToPlay.Play();
        }
    }
}

[System.Serializable]
public class CanvasAudioEvents
{
    [Tooltip("The Canvas to monitor")]
    public Canvas targetCanvas;

    [Tooltip("Actions when canvas is activated")]
    public List<EventAudioAction> onActivatedActions = new List<EventAudioAction>();

    [Tooltip("Actions when canvas is deactivated")]
    public List<EventAudioAction> onDeactivatedActions = new List<EventAudioAction>();

    [HideInInspector] public bool previousActiveState;

    public void CheckStateChange()
    {
        if (targetCanvas == null) return;

        bool currentActiveState = targetCanvas.gameObject.activeSelf;

        if (currentActiveState != previousActiveState)
        {
            Debug.Log($"[Canvas Event] {targetCanvas.name} changed from {previousActiveState} to {currentActiveState}");

            if (currentActiveState)
            {
                ExecuteActions(onActivatedActions);
            }
            else
            {
                ExecuteActions(onDeactivatedActions);
            }

            previousActiveState = currentActiveState;
        }
    }

    public void ExecuteActions(List<EventAudioAction> actions)
    {
        foreach (var action in actions)
        {
            action.Execute();
        }
    }
}

public class CanvasEventManager : MonoBehaviour
{
    [Tooltip("List of canvas events to manage")]
    public List<CanvasAudioEvents> canvasEvents = new List<CanvasAudioEvents>();

    private void Awake()
    {
        InitializeCanvasStates();
    }

    private void Update()
    {
        MonitorCanvasStates();
    }

    private void InitializeCanvasStates()
    {
        foreach (var canvasEvent in canvasEvents)
        {
            if (canvasEvent.targetCanvas != null)
            {
                canvasEvent.previousActiveState = canvasEvent.targetCanvas.gameObject.activeSelf;
                Debug.Log($"[Manager] Initialized {canvasEvent.targetCanvas.name} with state {canvasEvent.previousActiveState}");
            }
        }
    }

    private void MonitorCanvasStates()
    {
        foreach (var canvasEvent in canvasEvents)
        {
            canvasEvent.CheckStateChange();
        }
    }

    // Public method to manually trigger a canvas's activated actions
    public void TriggerActivatedActions(int canvasIndex)
    {
        if (canvasIndex >= 0 && canvasIndex < canvasEvents.Count)
        {
            canvasEvents[canvasIndex].ExecuteActions(canvasEvents[canvasIndex].onActivatedActions);
        }
    }

    // Public method to manually trigger a canvas's deactivated actions
    public void TriggerDeactivatedActions(int canvasIndex)
    {
        if (canvasIndex >= 0 && canvasIndex < canvasEvents.Count)
        {
            canvasEvents[canvasIndex].ExecuteActions(canvasEvents[canvasIndex].onDeactivatedActions);
        }
    }

    // Editor helper to add new canvas event
    [ContextMenu("Add New Canvas Event")]
    public void AddNewCanvasEvent()
    {
        canvasEvents.Add(new CanvasAudioEvents());
        Debug.Log("[Manager] Added new canvas event slot");
    }
}