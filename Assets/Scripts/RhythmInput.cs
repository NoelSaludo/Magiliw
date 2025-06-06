using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RhythmInput : MonoBehaviour
{
    private InputAction trig1;
    private InputAction trig2;
    private InputAction trig3;
    private InputAction trig4;

    [SerializeField] private Image[] triggerImages;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trig1 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger1");
        trig2 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger2");
        trig3 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger3");
        trig4 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger4");
    }

    // Update is called once per frame
    void Update()
    {
        // Reset images if triggers are not pressed
        for (int i = 0; i < triggerImages.Length; i++)
        {
            triggerImages[i].color = Color.red; // Reset to red
        }
        
        if (trig1.IsPressed())
        {
            Debug.Log("Trigger 1 pressed");
            UpdateImage(0, true);
        }
        if (trig2.IsPressed())
        {
            Debug.Log("Trigger 2 pressed");
            UpdateImage(1, true);
        }
        if (trig3.IsPressed())
        {
            Debug.Log("Trigger 3 pressed");
            UpdateImage(2, true);
        }
        if (trig4.IsPressed())
        {
            Debug.Log("Trigger 4 pressed");
            UpdateImage(3, true);
        }
        
    }
    
    void UpdateImage(int index, bool isActive)
    {
        if (index >= 0 && index < triggerImages.Length)
        {
            triggerImages[index].color = isActive ? Color.green : Color.red;
        }
    }
}
