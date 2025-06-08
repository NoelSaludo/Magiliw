using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMProTypewriter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Characters per second")]
    public float speed = 0.01f;
    public bool startOnEnable = true;
    public bool clearOnStart = true;
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent onTypingComplete;
    
    private TMP_Text textComponent;
    private string originalText;
    private float timer;
    private int charactersToShow;
    private bool isTyping = false;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        originalText = textComponent.text;
        
        if (clearOnStart)
        {
            textComponent.text = "";
        }
    }

    void OnEnable()
    {
        if (startOnEnable)
        {
            StartTyping();
        }
    }

    void Update()
    {
        if (!isTyping) return;
        
        timer += Time.deltaTime * speed;
        charactersToShow = Mathf.FloorToInt(timer);
        charactersToShow = Mathf.Clamp(charactersToShow, 0, originalText.Length);
        
        textComponent.text = originalText.Substring(0, charactersToShow);
        
        if (charactersToShow >= originalText.Length)
        {
            CompleteTyping();
        }
    }

    public void StartTyping()
    {
        timer = 0;
        charactersToShow = 0;
        isTyping = true;
        enabled = true;
    }

    public void CompleteTyping()
    {
        textComponent.text = originalText; // Ensure all text is shown
        isTyping = false;
        onTypingComplete.Invoke();
        enabled = false;
    }

    public void SkipToEnd()
    {
        CompleteTyping();
    }

    public void SetText(string newText)
    {
        originalText = newText;
        StartTyping();
    }
}