using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    private int _score = 0;
    private int _highScore = 0;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _highScore = PlayerPrefs.GetInt("highscore", 0);
    }

    public void AddPoints(int amount)
    {
        _score += amount;
        if (_highScore < _score)
        {
            // saves the highest score outside the game loop
            PlayerPrefs.SetInt("highscore", _score);
        }
    }
}

