using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonTextChanger : MonoBehaviour
{
    public TMProTypewriter typewriter; // Reference to your typewriter script
    public string newText = "New Text";
    
    private Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeText);
    }
    
    void ChangeText()
    {
        if(typewriter != null)
        {
            typewriter.SetText(newText); // Use the typewriter's method
        }
    }
}