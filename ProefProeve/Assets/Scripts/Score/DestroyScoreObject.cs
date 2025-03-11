using UnityEngine;

public class DestroyScoreObject : MonoBehaviour
{
    public float lifetime;

    private void Awake()
    {
        Destroy(this.gameObject, lifetime);
    }
}
