using UnityEngine;

public abstract class State : MonoBehaviour
{
    /// <summary>
    /// Gets called when entering the current state
    /// </summary>
    public virtual void Enter()
    {
        
    }

    /// <summary>
    /// Gets called every frame in the current state
    /// </summary>
    public virtual void Tick()
    {
        
    }

    /// <summary>
    /// Gets called when exiting the current state
    /// </summary>
    public virtual void Exit()
    {
        
    }
}
