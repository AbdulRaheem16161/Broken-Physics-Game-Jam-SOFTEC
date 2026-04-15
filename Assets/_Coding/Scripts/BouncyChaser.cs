using UnityEngine;

public class BouncyChaser : MonoBehaviour
{
    public Rigidbody rb;
    public Transform target;

    [Header("Movement")]
    public float moveForce = 8f;
    public float maxSpeed = 12f;

    [Header("Bounce")]
    public float bounceForce = 6f;
    public float upwardBias = 1.5f;

    [Header("Control")]
    public float airControl = 0.5f;

    private void Awake()
    {
        FindTarget();
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            FindTarget();
        }

        if (!target) return;

        // Direction towards target
        Vector3 dir = (target.position - transform.position).normalized;

        // Small air control so it doesn't go dumb mid-air
        rb.AddForce(dir * moveForce * airControl, ForceMode.Acceleration);

        // Clamp speed so it doesn't become a missile
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!target)
        {
            FindTarget();
            if (!target) return;
        }

        ContactPoint contact = collision.contacts[0];

        // Direction towards target
        Vector3 toTarget = (target.position - transform.position).normalized;

        // Bounce direction based on surface
        Vector3 bounceDir = Vector3.Reflect(rb.linearVelocity.normalized, contact.normal);

        // Mix bounce + target direction
        Vector3 finalDir = (bounceDir + toTarget + Vector3.up * upwardBias).normalized;

        // Apply bounce impulse
        rb.AddForce(finalDir * bounceForce, ForceMode.Impulse);
    }

    private void FindTarget()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }
}