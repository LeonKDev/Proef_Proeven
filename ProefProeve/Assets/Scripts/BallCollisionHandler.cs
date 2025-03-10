using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    private BallController _controller;
    private BallMovementHandler _movementHandler;
    private GameObject _lastCollidedObject;
    

    public void Initialize(BallController controller, BallMovementHandler movementHandler)
    {
        _controller = controller;
        _movementHandler = movementHandler;
        _lastCollidedObject = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            // Store the last collided GameObject
            _lastCollidedObject = collision.gameObject;
            
            if (_movementHandler.CurrentSpeed > 40f)
            {
                // calculate point multiplier and add points accordingly
                var multiplier = _movementHandler.CurrentSpeed / 40;
                var pointsToAdd = (int)Mathf.Round(100 * multiplier);
                ScoreManager.Instance.AddPoints(pointsToAdd);
                
                ScoreManager.Instance.InstantiateScoreObject(collision, pointsToAdd);
            }
            
            // Reflect the current direction based on collision normal
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_movementHandler.Direction, normal);
            
            // Apply the new reflected direction
            _movementHandler.SetDirection(reflectedDirection);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Store last triggered GameObject
        _lastCollidedObject = other.gameObject;
    }
    
    /// <summary>
    /// Returns the last GameObject that this object collided or triggered with.
    /// </summary>
    /// <returns>The GameObject of the last collision or trigger, or null if none has occurred yet.</returns>
    public GameObject GetLastCollidedObject()
    {
        return _lastCollidedObject;
    }
}