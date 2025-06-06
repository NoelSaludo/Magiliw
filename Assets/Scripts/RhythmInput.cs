using UnityEngine;
using UnityEngine.InputSystem;

public class RhythmInput : MonoBehaviour
{
    private InputAction trig1;
    private InputAction trig2;
    private InputAction trig3;
    private InputAction trig4;
    
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
        if (trig1.triggered)
        {
            Debug.Log("Trigger 1 pressed");
        }
        if (trig2.triggered)
        {
            Debug.Log("Trigger 2 pressed");
        }
        if (trig3.triggered)
        {
            Debug.Log("Trigger 3 pressed");
        }
        if (trig4.triggered)
        {
            Debug.Log("Trigger 4 pressed");
        }
    }
}
