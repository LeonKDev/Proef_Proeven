using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    
    [Header("Health Settings")]
    [SerializeField] private int _health;
    [SerializeField] private float damageBreakTimer = 3;
    [SerializeField] private Color colorOnDamage;
    private float oldDamageBreakTimer;

    [Header("Health References")] 
    [SerializeField] private Slider HealthUI;
    [SerializeField] private new Renderer renderer;
    
    private bool _damageBreak;
    private bool _colorChange;
    public int Health
    {
        get => _health;
        set => _health = value;
    }

    private void Start()
    {
        oldDamageBreakTimer = damageBreakTimer;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!_damageBreak)
            return;
        
        damageBreakTimer -= Time.deltaTime;
        
        if (damageBreakTimer <= 0)
        {
            _damageBreak = false;
            damageBreakTimer = oldDamageBreakTimer;
        }
    }

    public void DamagePlayer(int damageAmount)
    {
        if (_damageBreak)
            return;
        
        Health -= damageAmount;
        HealthUI.value = Health;
        _damageBreak = true;
        
        // Play hit flash through GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayHitFlash(renderer, colorOnDamage);
        }
        
        if (Health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        // Let the GameManager handle the game over screen
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandlePlayerDeath();
        }

        // Destroy the player object
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            DamagePlayer(1);
        }
    }
}
