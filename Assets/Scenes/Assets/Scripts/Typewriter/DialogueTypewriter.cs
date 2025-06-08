using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(TMP_Text))]
public class DialogueTypewriter : MonoBehaviour
{
    [Header("Text Segments")]
    public List<string> textSegments = new List<string>();
    
    [Header("Typing Settings")]
    public float charactersPerSecond = 20f;
    public float delayBetweenSegments = 0.5f;
    public bool startAutomatically = true;
    public bool clearTextOnStart = true;

    [Header("Interaction")]
    public bool allowSkip = true;
    public float minTypingDurationBeforeSkip = 0.5f;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnTypingBegin;
    public UnityEngine.Events.UnityEvent OnSegmentComplete;
    public UnityEngine.Events.UnityEvent OnAllSegmentsComplete;

    // Public accessors
    public bool IsTyping { get; private set; }
    public int CurrentSegmentIndex { get; private set; }
    public int TotalSegments => textSegments.Count;

    private TMP_Text _textComponent;
    private float _timer;
    private int _visibleCharacters;
    private bool _inDelay;
    private float _typingStartTime;

    void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
        if (clearTextOnStart) _textComponent.text = "";
    }

    void OnEnable()
    {
        if (startAutomatically && textSegments.Count > 0)
        {
            StartTypingSegment(0);
        }
    }

    void Update()
    {
        if (!IsTyping) return;

        if (_inDelay)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _inDelay = false;
                StartTypingSegment(CurrentSegmentIndex + 1); // Start next segment after delay
            }
            return;
        }

        _timer += Time.deltaTime * charactersPerSecond;
        _visibleCharacters = Mathf.FloorToInt(_timer);
        _visibleCharacters = Mathf.Clamp(_visibleCharacters, 0, textSegments[CurrentSegmentIndex].Length);

        _textComponent.text = textSegments[CurrentSegmentIndex].Substring(0, _visibleCharacters);

        if (_visibleCharacters >= textSegments[CurrentSegmentIndex].Length)
        {
            CompleteCurrentSegment();
        }
    }

    public void StartTypingSegment(int segmentIndex)
    {
        if (segmentIndex < 0 || segmentIndex >= textSegments.Count) return;

        CurrentSegmentIndex = segmentIndex;
        _textComponent.text = "";
        _timer = 0;
        _visibleCharacters = 0;
        IsTyping = true;
        _inDelay = false;
        _typingStartTime = Time.time;
        OnTypingBegin.Invoke();
    }

    private void CompleteCurrentSegment()
    {
        _textComponent.text = textSegments[CurrentSegmentIndex];
        IsTyping = false;
        OnSegmentComplete.Invoke();

        if (CurrentSegmentIndex < textSegments.Count - 1)
        {
            StartNextSegmentAfterDelay();
        }
        else
        {
            OnAllSegmentsComplete.Invoke();
        }
    }

    private void StartNextSegmentAfterDelay()
    {
        _inDelay = true;
        _timer = delayBetweenSegments;
        IsTyping = true;
        // Do NOT increment CurrentSegmentIndex here (handled in Update after delay)
    }

    public void SkipCurrentSegment()
    {
        if (!allowSkip || Time.time - _typingStartTime < minTypingDurationBeforeSkip) return;

        if (_inDelay)
        {
            _inDelay = false;
        }
        CompleteCurrentSegment();
    }

    public void AdvanceToNextSegment()
    {
        if (CurrentSegmentIndex < textSegments.Count - 1)
        {
            StartTypingSegment(CurrentSegmentIndex + 1);
        }
    }

    public void ResetToFirstSegment()
    {
        StartTypingSegment(0);
    }
}