using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostMultiplier = 3f;  // How much faster it goes when boosted
    [SerializeField] private float boostDuration = 0.5f;  // How long the boost lasts

    private Rigidbody rb;
    private Vector3 lastValidDirection;
    private float currentSpeed;
    private float boostTimeRemaining;
    private float initialBoostSpeed;
    private bool isBoosting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;

        // Initial random direction
        lastValidDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        // Set initial velocity
        rb.velocity = lastValidDirection * currentSpeed;
    }

    void Update()
    {
        // Check for boost input
        if (Input.GetKeyDown(KeyCode.E) && !isBoosting)
        {
            ApplyBoost();
        }
    }

    void FixedUpdate()
    {
        // Handle boost duration and speed
        if (isBoosting)
        {
            boostTimeRemaining -= Time.fixedDeltaTime;
            if (boostTimeRemaining > 0)
            {
                // Smoothly interpolate between boost speed and base speed
                float progress = 1 - (boostTimeRemaining / boostDuration);
                currentSpeed = Mathf.Lerp(initialBoostSpeed, baseSpeed, EaseOut(progress));
            }
            else
            {
                currentSpeed = baseSpeed;
                isBoosting = false;
            }
        }

        // Maintain constant speed
        if (rb.velocity.magnitude != 0)
        {
            lastValidDirection = rb.velocity.normalized;
            rb.velocity = lastValidDirection * currentSpeed;
        }
        else
        {
            // If velocity somehow becomes zero, use last known direction
            rb.velocity = lastValidDirection * currentSpeed;
        }

        // Keep the ball moving on the horizontal plane
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Calculate reflection direction
        if (collision.contacts.Length > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            lastValidDirection = Vector3.Reflect(lastValidDirection, normal);
            rb.velocity = lastValidDirection * currentSpeed;
        }
    }

    private void ApplyBoost()
    {
        isBoosting = true;
        initialBoostSpeed = baseSpeed * boostMultiplier;
        currentSpeed = initialBoostSpeed;
        boostTimeRemaining = boostDuration;

        // Add a small random angle variation for more dynamic feel (-15 to 15 degrees)
        float randomAngle = Random.Range(-15f, 15f);
        lastValidDirection = Quaternion.Euler(0, randomAngle, 0) * lastValidDirection;
        rb.velocity = lastValidDirection * currentSpeed;
    }

    private float EaseOut(float t)
    {
        // Cubic ease-out function for smoother deceleration
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
