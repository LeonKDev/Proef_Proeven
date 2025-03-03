using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int _score;
    private int _highScore;

    
    private void Awake()
    {
        Instance = this;
    }

    public void AddPoints(int amount)
    {
        _score += amount;
    }
    
}

