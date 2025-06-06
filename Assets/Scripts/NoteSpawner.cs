using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab; // Prefab for the note to spawn
    [SerializeField] private float spawnInterval = 1f; // Time interval between spawns
    [SerializeField] private float timer = 0f;
    [SerializeField] private GameObject[] spawnArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= spawnInterval)
        {
            int randomIndex = Random.Range(0, spawnArea.Length);
            SpawnNote(randomIndex);
            timer = 0f; // Reset the timer after spawning
        }
        else
        {
            timer += Time.deltaTime; // Increment the timer
        }
    }

    void SpawnNote(int index)
    {
        Vector2 position = spawnArea[index].transform.position;
        GameObject note = Instantiate(notePrefab, position, Quaternion.identity);
        note.transform.SetParent(transform);
    }
}
