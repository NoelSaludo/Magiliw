using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(TMP_Text))]
public class DialogueTitleToggle : MonoBehaviour
{
    [System.Serializable]
    public class ToggleElement
    {
        [Tooltip("The text to display when the element is active")]
        public string displayText;

        [Tooltip("Segment index to start showing this text (inclusive)")]
        public int startDisplayIndex;

        [Tooltip("Segment index to stop showing this text (inclusive)")]
        public int endDisplayIndex;
    }

    [Header("Segment Source")]
    [Tooltip("Reference to the DialogueTypewriter that controls the current segment")]
    public DialogueTypewriter sourceTypewriter;

    [Header("Toggle Elements")]
    public List<ToggleElement> toggleElements = new List<ToggleElement>();

    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        if (_text == null)
        {
            Debug.LogError("DialogueTitleToggle: TMP_Text component is missing.");
            enabled = false;
        }

        if (sourceTypewriter == null)
        {
            Debug.LogError("DialogueTitleToggle: Source DialogueTypewriter is not assigned.");
            enabled = false;
        }
    }

    void Update()
    {
        if (sourceTypewriter == null) return;

        int currentIndex = sourceTypewriter.CurrentSegmentIndex;
        bool textSet = false;

        foreach (var element in toggleElements)
        {
            if (currentIndex >= element.startDisplayIndex && currentIndex <= element.endDisplayIndex)
            {
                _text.text = element.displayText;
                textSet = true;
                break;
            }
        }

        if (!textSet)
        {
            _text.text = "";
        }
    }
}
