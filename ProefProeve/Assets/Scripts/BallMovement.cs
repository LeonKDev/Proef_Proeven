using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostMultiplier = 3f;
    [SerializeField] private float boostDuration = 0.5f;
    [SerializeField] private GameObject batObject;
    [SerializeField] private float normalBounceMultiplier = 2f; // For 1-2 unit range
    [SerializeField] private float closeRangeBounceMultiplier = 3.5f; // For 0-1 unit range
    // New variables for curving behavior
    [SerializeField] private float curveStrength = 0.02f; // How much to adjust the direction
    [SerializeField] private float curveResponse = 2f;    // How fast the adjustment is applied

    private Rigidbody rb;
    private Vector3 lastValidDirection;
    public float currentSpeed;
    // Reintroduced boost state variables
    private float boostTimeRemaining;
    private float initialBoostSpeed;

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
        rb.velocity = lastValidDirection * currentSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && batObject != null)
        {
            float distanceToBat = Vector3.Distance(transform.position, batObject.transform.position);
            if (distanceToBat <= 4f)
            {
                Vector3 batDirection = batObject.transform.forward;
                float bounceMultiplier = distanceToBat <= 2f ? closeRangeBounceMultiplier : normalBounceMultiplier;
                // Apply boost effect on hit
                currentSpeed = baseSpeed * boostMultiplier * bounceMultiplier;
                initialBoostSpeed = currentSpeed;
                boostTimeRemaining = boostDuration;
                lastValidDirection = batDirection.normalized;
                rb.velocity = lastValidDirection * currentSpeed;
            }
        }
        Debug.Log(Vector3.Distance(transform.position, batObject.transform.position));
    }

    void FixedUpdate()
    {
        // Decay boost if active
        if (boostTimeRemaining > 0)
        {
            boostTimeRemaining -= Time.fixedDeltaTime;
            float progress = 1 - Mathf.Clamp01(boostTimeRemaining / boostDuration);
            currentSpeed = Mathf.Lerp(initialBoostSpeed, baseSpeed, progress);
        }
        else
        {
            currentSpeed = baseSpeed;
        }

        // Maintain constant speed
        if (rb.velocity.magnitude != 0)
        {
            lastValidDirection = rb.velocity.normalized;
            rb.velocity = lastValidDirection * currentSpeed;
        }
        else
        {
            rb.velocity = lastValidDirection * currentSpeed;
        }

        // Keep the ball moving on the horizontal plane
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Apply a subtle curving effect toward the player (bat)
        if (batObject != null)
        {
            Vector3 toPlayer = (batObject.transform.position - transform.position).normalized;
            // Reduce curve effect when currentSpeed is higher than baseSpeed.
            float effectiveCurve = (curveStrength * curveResponse * Time.fixedDeltaTime) * (baseSpeed / currentSpeed);
            Vector3 newDir = Vector3.Slerp(rb.velocity.normalized, toPlayer, effectiveCurve).normalized;
            rb.velocity = newDir * currentSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            // Reflect the current direction based on collision normal
            Vector3 normal = collision.contacts[0].normal;
            lastValidDirection = Vector3.Reflect(lastValidDirection, normal);
            rb.velocity = lastValidDirection * currentSpeed;
        }
    }

    private float EaseOut(float t)
    {
        // Unused now, but kept for reference
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
