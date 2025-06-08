using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public class DialogueStep
    {
        public string name;
        public string text;
        public Sprite background;
        public Sprite character;
    }

    public DialogueStep[] steps;
    private int currentStep = 0;

    public GameObject nextButton;
    public GameObject startMinigameButton;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Image backgroundImage;
    public Image characterImage;
    public float typingSpeed = 0.025f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    public void Start()
    {
        ShowStep(currentStep);
    }

    public void Next()
    {
        if (isTyping)
        {
            // Instantly finish typing
            StopCoroutine(typingCoroutine);
            dialogueText.text = steps[currentStep].text;
            isTyping = false;
            return;
        }

        currentStep++;
        if (currentStep < steps.Length)
        {
            ShowStep(currentStep);
        }
        else
        {
            dialogueText.text = "The End.";
            nameText.text = "";
        }
    }

    void ShowStep(int stepIndex)
    {
        DialogueStep step = steps[stepIndex];

        if (step.background != null)
            backgroundImage.sprite = step.background;

        if (step.character != null)
        {
            characterImage.sprite = step.character;
            characterImage.gameObject.SetActive(true);
        }
        else
        {
            characterImage.gameObject.SetActive(false);
        }
        
        nameText.text = step.name;

        if (stepIndex == 34)
        {
            nextButton.SetActive(false);
            startMinigameButton.SetActive(true);
        }
        else
        {
            nextButton.SetActive(true);
            startMinigameButton.SetActive(false);
        }
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(step.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
    
    public void StartMinigame()
    {
        SceneManager.LoadScene("Scenes/MainGame");
    }
}