using System;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private float speed; // Speed at which the note moves

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (speed * Time.deltaTime));

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Note hit by player");
            Destroy(gameObject);
        }
    }
}
