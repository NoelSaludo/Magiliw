using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class DisplayImageToggle : MonoBehaviour
{
    [System.Serializable]
    public class ImageToggleElement
    {
        [Tooltip("Sprite to show during the segment range")]
        public Sprite sprite;

        [Tooltip("Start displaying at this segment index (inclusive)")]
        public int startDisplayIndex;

        [Tooltip("Stop displaying after this segment index (inclusive)")]
        public int endDisplayIndex;
    }

    [Header("Dependencies")]
    [Tooltip("Reference to the DialogueTypewriter that controls segment progression")]
    public DialogueTypewriter sourceTypewriter;

    [Header("Toggle Configuration")]
    public List<ImageToggleElement> imageElements = new List<ImageToggleElement>();

    [Header("Fade Settings")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    private Image _image;
    private CanvasGroup _canvasGroup;
    private int _lastIndex = -1;
    private Coroutine _fadeCoroutine;

    void Awake()
    {
        _image = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (sourceTypewriter == null)
        {
            Debug.LogError("DisplayImageToggle: sourceTypewriter not assigned.");
            enabled = false;
        }

        _canvasGroup.alpha = 0f; // Start hidden
        _image.enabled = false;
    }

    void Update()
    {
        if (sourceTypewriter == null) return;

        int currentIndex = sourceTypewriter.CurrentSegmentIndex;
        if (currentIndex == _lastIndex) return;

        _lastIndex = currentIndex;
        UpdateDisplayedImage(currentIndex);
    }

    private void UpdateDisplayedImage(int currentIndex)
    {
        foreach (var element in imageElements)
        {
            if (currentIndex >= element.startDisplayIndex && currentIndex <= element.endDisplayIndex)
            {
                _image.sprite = element.sprite;
                _image.enabled = true;

                if (_fadeCoroutine != null)
                    StopCoroutine(_fadeCoroutine);

                _fadeCoroutine = StartCoroutine(FadeCanvasGroup(1f, fadeInDuration));
                return;
            }
        }

        // If no image matched, fade out
        if (_image.enabled)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeOutAndDisable(fadeOutDuration));
        }
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeOutAndDisable(float duration)
    {
        yield return FadeCanvasGroup(0f, duration);
        _image.enabled = false;
    }
}
