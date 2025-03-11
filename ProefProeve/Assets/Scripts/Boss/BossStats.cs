using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    [SerializeField] private int _health;
    private bool _hasBall;

    public bool HasBall
    {
        get => _hasBall;
        set => _hasBall = value;
    }
}
