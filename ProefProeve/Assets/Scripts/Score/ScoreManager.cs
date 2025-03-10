using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    private int _score = 0;
    private int _highScore = 0;
    
    //References for UI
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighscoreText;
    
    //Property for accessing score safely 
    public int Score
    {
        get => _score;
        private set => _score = value;
    }
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _highScore = PlayerPrefs.GetInt("highscore", 0);
        
        ScoreText.text = Score.ToString("00000");
        HighscoreText.text = _highScore.ToString("00000");
    }
    
    /// <summary>
    /// Class <c>AddPoints</c> Adds amount to the current score.
    /// </summary>
    public void AddPoints(int amount)
    {
        Score += amount;
        ScoreText.text = Score.ToString("00000");
        
        if (_highScore > Score)
            return;
        
        // saves the highest score outside the game loop
        PlayerPrefs.SetInt("highscore", Score);
    }
}