using UnityEngine;
using UnityEngine.UI;

public class ColorButtonBlinker : MonoBehaviour
{
    public Button targetButton;
    public Color lowColor = Color.gray;
    public Color highColor = Color.white;
    public float duration = 1f;

    private Image buttonImage;
    private float timer;
    private bool goingToHigh = true;

    void Start()
    {
        if (targetButton == null)
        {
            Debug.LogError("ColorButtonBlinker: No button assigned.");
            enabled = false;
            return;
        }

        buttonImage = targetButton.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("ColorButtonBlinker: Button has no Image component.");
            enabled = false;
            return;
        }

        buttonImage.color = lowColor;
    }

    void Update()
    {
        if (buttonImage == null) return;

        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / duration, 1f);
        buttonImage.color = Color.Lerp(lowColor, highColor, t);
    }
}
