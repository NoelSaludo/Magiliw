using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RhythmInput : MonoBehaviour
{
    private InputAction trig1;
    private InputAction trig2;
    private InputAction trig3;
    private InputAction trig4;

    [SerializeField] private GameObject[] triggerGO;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource audioSource;

    // Track previous trigger states to avoid repeated PlayOneShot per frame
    bool[] prevPressed = new bool[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trig1 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger1");
        trig2 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger2");
        trig3 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger3");
        trig4 = InputSystem.actions.FindActionMap("Player").FindAction("Trigger4");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.getState() == GameState.End)
            return;
        
        // Reset images if triggers are not pressed
        for (int i = 0; i < triggerGO.Length; i++)
        {
            if (triggerGO[i].TryGetComponent<Collider>(out Collider col))
                col.enabled = false;
            triggerGO[i].transform.localScale = Vector3.one;
        }

        bool[] currPressed = new bool[4];
        currPressed[0] = trig1.IsPressed();
        currPressed[1] = trig2.IsPressed();
        currPressed[2] = trig3.IsPressed();
        currPressed[3] = trig4.IsPressed();

        if (currPressed[0])
        {
            ActivateTrigger(0);
            if (!prevPressed[0] && audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        if (currPressed[1])
        {
            ActivateTrigger(1);
            if (!prevPressed[1] && audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        if (currPressed[2])
        {
            ActivateTrigger(2);
            if (!prevPressed[2] && audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        if (currPressed[3])
        {
            ActivateTrigger(3);
            if (!prevPressed[3] && audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        // Update previous pressed states
        for (int i = 0; i < 4; i++)
            prevPressed[i] = currPressed[i];
    }

    void UpdateTrigger(int index)
    {
        if (index >= 0 && index < triggerGO.Length)
        {
            triggerGO[index].transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        else
        {
            Debug.LogWarning($"Index {index} is out of bounds for triggerGO array");
        }
    }

    void ActivateTrigger(int index)
    {
        if (index >= 0 && index < triggerGO.Length)
        {
            if (triggerGO[index].TryGetComponent<Collider>(out Collider triggerCollider))
            {
                if (triggerCollider != null)
                {
                    triggerCollider.enabled = true; // Enable the collider
                    UpdateTrigger(index); // Update the image to green
                }
                else
                {
                    Debug.LogWarning($"No Collider2D found on trigger image at index {index}");
                }
            }
        }
    }
}

