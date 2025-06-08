using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float perfectThreshold = 0.1f;
    [SerializeField] private float goodThreshold = 0.3f;

    private ScoreType scoreType;

    private GameManager gameManager;

    [SerializeField] private GameObject feedbackTextPrefab;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        transform.Translate(Vector3.left * (speed * Time.deltaTime));
        if (transform.position.y < -10f)
        {
            gameManager.AddScore(ScoreType.Miss);
            CreateFeedbackText(ScoreType.Miss, new Vector3(0, 5, 0));
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Note hit by player");
            float distance = Mathf.Abs(transform.position.y - other.transform.position.y);

            if (distance <= perfectThreshold)
                scoreType = ScoreType.Perfect;
            else if (distance <= goodThreshold)
                scoreType = ScoreType.Good;
            else
                scoreType = ScoreType.Miss;

            // Create feedback text
            CreateFeedbackText(scoreType, new Vector3(0, 1, 0));

            gameManager.AddScore(scoreType);
            Destroy(gameObject);
        }
    }

    private void CreateFeedbackText(ScoreType type, Vector3 offset)
    {
        string feedbackText = type.ToString();
        Debug.Log(feedbackText);
        Debug.Log(transform.position + offset);
        GameObject feedbackInstance =
            Instantiate(feedbackTextPrefab, transform.position + offset, Quaternion.identity);
        feedbackInstance.GetComponentInChildren<TextMesh>().text = feedbackText;
        feedbackInstance.GetComponentInChildren<TextMesh>().color = type == ScoreType.Perfect ? Color.gold :
            type == ScoreType.Good ? Color.green : Color.red;
        Destroy(feedbackInstance, 1f); // Destroy after 1 second
    }
}