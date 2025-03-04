using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    // Score storage
    private int _score = 0;
    private int _highScore = 0;
    
    //References for UI
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighscoreText;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _highScore = PlayerPrefs.GetInt("highscore", 0);
        
        ScoreText.text = _score.ToString("00000");
        HighscoreText.text = _highScore.ToString("00000");
    }

    public void AddPoints(int amount)
    {
        _score += amount;
        ScoreText.text = _score.ToString("00000");
        
        if (_highScore > _score)
            return;
        
        // saves the highest score outside the game loop
        PlayerPrefs.SetInt("highscore", _score);
    }
}