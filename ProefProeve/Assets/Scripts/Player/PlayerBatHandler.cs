using UnityEngine;
using BallGame.Shared;

public class PlayerBatHandler : MonoBehaviour
{
    [Header("Bat Configuration")]
    [SerializeField] private GameObject batObject;
    [SerializeField] private Animator batAnimator;
    [SerializeField] private string swingAnimationTrigger = "Swing";
    
    [Header("Hit Properties")]
    [SerializeField] private float hitCooldown = 0.5f;
    
    private float _lastHitTime = 0f;
    
    private void Start()
    {
        // If no animator assigned, try to get from bat object
        if (batAnimator == null && batObject != null)
        {
            batAnimator = batObject.GetComponent<Animator>();
            Debug.Log("Bat animator not assigned, automatically found from bat object");
        }
        
        if (batObject == null)
        {
            Debug.LogWarning("Bat object reference missing in PlayerBatHandler!");
        }
    }
    
    public bool CanSwing()
    {
        return Time.time >= _lastHitTime + hitCooldown;
    }
    
    public void SwingBat()
    {
        if (CanSwing())
        {
            _lastHitTime = Time.time;
            
            // Play bat swing animation
            if (batAnimator != null)
            {
                batAnimator.SetTrigger(swingAnimationTrigger);
            }
        }
    }
    
    public GameObject GetBatObject()
    {
        return batObject;
    }
}