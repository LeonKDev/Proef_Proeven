using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    
    [Header("Health Settings")]
    [SerializeField] private int _health;
    [SerializeField] private float damageBreakTimer = 3;
    private float oldDamageBreakTimer;
    
    [Header("Health References")]
    [SerializeField] private Renderer renderer;
    
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
        _damageBreak = true;
        
        if (Health <= 0)
        {
            Death();
        }
        
        StartCoroutine(HitFlash()); ;
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    public IEnumerator HitFlash()
    {
        Debug.Log("flash");
        var material = renderer.material;
        material.color = Color.red;     
        yield return new WaitForSeconds(0.5f); 
        material.color = Color.white;     
        yield return new WaitForSeconds(0.5f); 
        material.color = Color.red;     
        yield return new WaitForSeconds(0.5f); 
        material.color = Color.white;     
        yield return new WaitForSeconds(0.5f);
    }
}
