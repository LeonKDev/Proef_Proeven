using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallOwner
{
    None,
    Player,
    Boss
}

public class BallOwnerHandler : MonoBehaviour
{
    public BallOwner ballOwner;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public BallOwner GetCurrentBallOwner()
    {
        return ballOwner;
    }
}
