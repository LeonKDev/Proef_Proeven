using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerData : MonoBehaviour
{
    private bool coroutineCalled = false;
    private bool damagebreak;
    private bool colorchange;
    [SerializeField] private float DamageBreakTimer = 3;
        
    //todo; check if health works 
    public static PlayerData Instance;
    [SerializeField] private int _health;
    [SerializeField] private Renderer renderer;
    private Color _color;
    public int Health
    {
        get => _health;
        set => _health = value;
    }
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _color = renderer.material.color;
    }
    
    public void DamagePlayer(int damageAmount)
    {
        if (damagebreak)
            return;
        
        Health -= damageAmount;
        damagebreak = true;
        
        if (Health <= 0)
        {
            Death();
        }
        
        DamageBreakTimer -= Time.deltaTime;
        StartCoroutine(HitFlash());
        if (DamageBreakTimer <= 0)
        {
            damagebreak = false;
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    public IEnumerator HitFlash()
    {
        Debug.Log("flash");
        _color = Color.red;     
        yield return new WaitForSeconds(0.5f); 
        _color = Color.white;     
        yield return new WaitForSeconds(0.5f); 
        _color = Color.red;     
        yield return new WaitForSeconds(0.5f); 
        _color = Color.white;     
        yield return new WaitForSeconds(0.5f);
    }
}
