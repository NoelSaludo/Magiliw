using UnityEngine;

public class MagiliwAnimScript : MonoBehaviour
{
    public float delay = 2.88f;          // Time between each step
    public float rotationAngle = -90f;   // Rotation per step
    public int maxSteps = 4;             // Total steps
    public float moveAmount = 0.92f;     // Distance to move each step

    private float timer = 0f;
    private int steps = 0;

    private Quaternion targetRotation;
    private Vector3 targetPosition;

    void Start()
    {
        targetRotation = transform.rotation;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (steps >= maxSteps)
            return;

        timer += Time.deltaTime;

        if (timer >= delay)
        {
            // Step 1: rotate
            targetRotation *= Quaternion.Euler(0, rotationAngle, 0);

            // Step 2: move in the *new forward* direction
            Vector3 forwardAfterRotation = targetRotation * Vector3.forward;
            targetPosition += forwardAfterRotation.normalized * moveAmount;

            steps++;
            timer = 0f;
        }

        // Smoothly rotate and move
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 90 * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1.5f * Time.deltaTime);
    }
}
