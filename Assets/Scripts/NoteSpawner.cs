using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab; // Prefab for the note to spawn
    [SerializeField] private float spawnInterval = 1f; // Time interval between spawns
    [SerializeField] private GameObject[] spawnArea;

    public void SpawnNote(int index)
    {
        Vector2 position = spawnArea[index].transform.position;
        GameObject note = Instantiate(notePrefab, position, Quaternion.identity);
    }
}
