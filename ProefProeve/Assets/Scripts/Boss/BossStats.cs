using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossStats : MonoBehaviour
{
    [SerializeField] private Slider HealthUI;
    [SerializeField] private int _health;
    
    private bool _hasBall = true;

    public int Health
    {
        get => _health;
        set => _health = value;
    }
    public bool HasBall
    {
        get => _hasBall;
        set => _hasBall = value;
    }
    public void DamageBoss(int damageAmount)
    {
        Health -= damageAmount;
        HealthUI.value = Health;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
