using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostMultiplier = 3f;
    [SerializeField] private float boostDuration = 0.5f;
    [SerializeField] private GameObject batObject;
    [SerializeField] private float normalBounceMultiplier = 2f; // For 1-2 unit range
    [SerializeField] private float closeRangeBounceMultiplier = 3.5f; // For 0-1 unit range

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

        rb.velocity = lastValidDirection * currentSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isBoosting && batObject != null)
        {
            float distanceToBat = Vector3.Distance(transform.position, batObject.transform.position);

            if (distanceToBat <= 4f)
            {
                Vector3 batDirection = batObject.transform.forward;
                float bounceMultiplier = distanceToBat <= 2f ? closeRangeBounceMultiplier : normalBounceMultiplier;

                isBoosting = true;
                initialBoostSpeed = baseSpeed * boostMultiplier * bounceMultiplier;
                currentSpeed = initialBoostSpeed;
                boostTimeRemaining = boostDuration;

                lastValidDirection = batDirection.normalized;
                rb.velocity = lastValidDirection * currentSpeed;
            }
        }
        Debug.Log(Vector3.Distance(transform.position, batObject.transform.position));
    }

    void FixedUpdate()
    {
        if (isBoosting)
        {
            boostTimeRemaining -= Time.fixedDeltaTime;
            if (boostTimeRemaining > 0)
            {
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
            rb.velocity = lastValidDirection * currentSpeed;
        }

        // Keep the ball moving on the horizontal plane
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            lastValidDirection = Vector3.Reflect(lastValidDirection, normal);
            rb.velocity = lastValidDirection * currentSpeed;
        }
    }

    private float EaseOut(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
