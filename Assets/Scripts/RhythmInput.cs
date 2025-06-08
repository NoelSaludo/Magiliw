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

        if (trig1.WasPressedThisFrame())
        {
            ActivateTrigger(0);
            if (audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        if (trig2.WasPressedThisFrame())
        {
            ActivateTrigger(1);
            if (audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        if (trig3.WasPressedThisFrame())
        {
            ActivateTrigger(2);
            if (audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }

        if (trig4.WasPressedThisFrame())
        {
            ActivateTrigger(3);
            if (audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip);
        }
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

