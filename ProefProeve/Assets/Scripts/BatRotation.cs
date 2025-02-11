using UnityEngine;

public class BatRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    private float currentRotation = 0f;

    void Start()
    {
        // Lock cursor to game window and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        // Use Raw Mouse Input for direct movement detection
        float mouseDelta = Input.GetAxisRaw("Mouse X");

        // Update rotation based on mouse movement
        currentRotation += mouseDelta * rotationSpeed;
        currentRotation = (currentRotation % 360f + 360f) % 360f;
        transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);

        // Allow unlocking cursor with Escape key for testing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnDisable()
    {
        // Ensure cursor is unlocked when script is disabled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
