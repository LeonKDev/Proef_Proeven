using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    [SerializeField] private int _health;
    private bool _hasBall = true;

    public int Health
    {
        get => _health;
        set => _health = value;
    }
    public void DamageBoss(int damageAmount)
    {
        Health -= damageAmount;
        
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public bool HasBall
    {
        get => _hasBall;
        set => _hasBall = value;
    }
}
