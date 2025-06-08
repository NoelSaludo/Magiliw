using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class DialogueButtonController : MonoBehaviour
{
    [Header("References")]
    public DialogueTypewriter typewriter;

    [Header("Button Behavior")]
    public bool skipCurrentSegment = true;
    public bool advanceToNextSegment = true;
    public bool loopFromStart = false;

    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        if (typewriter == null)
        {
            Debug.LogError("Typewriter reference not assigned!", this);
            return;
        }

        if (typewriter.IsTyping && skipCurrentSegment)
        {
            typewriter.SkipCurrentSegment();
        }
        else if (!typewriter.IsTyping && advanceToNextSegment)
        {
            if (typewriter.CurrentSegmentIndex >= typewriter.TotalSegments - 1 && loopFromStart)
            {
                typewriter.ResetToFirstSegment();
            }
            else
            {
                typewriter.AdvanceToNextSegment();
            }
        }
    }

    public void SetTextSegments(string[] newSegments)
    {
        if (typewriter != null)
        {
            typewriter.textSegments = new List<string>(newSegments);
        }
    }
}