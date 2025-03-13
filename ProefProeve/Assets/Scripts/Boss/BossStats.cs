using UnityEngine;
using UnityEngine.UI;
using System;

public class BossStats : MonoBehaviour
{
    [SerializeField] private Slider HealthUI;
    [SerializeField] private int _health;
    
    private bool _hasBall = true;

    // Event that will be triggered when the boss is defeated
    public static event Action OnBossDefeated;

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
            OnBossDefeated?.Invoke();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.HandleBossDefeated();
            }
            Destroy(gameObject);
        }
    }
}
