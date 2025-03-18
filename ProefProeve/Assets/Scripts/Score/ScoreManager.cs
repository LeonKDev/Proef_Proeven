using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    //References for UI
    [Header("Primary UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    [Header("Additional Score Displays")]
    [SerializeField] private TextMeshProUGUI[] additionalScoreTexts;
    [SerializeField] private TextMeshProUGUI[] additionalHighScoreTexts;
    
    [Header("Score Visual Effects")]
    [SerializeField] private Canvas collisionHitPrefab;
    
    private int _score;
    private int _highScore;
    
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
        UpdateAllScoreDisplays();
    }

    private void UpdateAllScoreDisplays()
    {
        string scoreString = Score.ToString("00000");
        string highScoreString = _highScore.ToString("00000");
        
        // Update main displays
        if (scoreText != null) scoreText.text = scoreString;
        if (highScoreText != null) highScoreText.text = highScoreString;
        
        // Update additional displays
        if (additionalScoreTexts != null)
        {
            foreach (var text in additionalScoreTexts)
            {
                if (text != null) text.text = scoreString;
            }
        }
        
        if (additionalHighScoreTexts != null)
        {
            foreach (var text in additionalHighScoreTexts)
            {
                if (text != null) text.text = highScoreString;
            }
        }
    }
    
    public void AddPoints(int amount)
    {
        // adds points to the score
        Score += amount;
        
        // Update high score if needed
        if (Score > _highScore)
        {
            _highScore = Score;
            PlayerPrefs.SetInt("highscore", _highScore);
        }
        
        // Update all displays
        UpdateAllScoreDisplays();
    }
    
    public void InstantiateScoreObject(Collision col, int pointsOnHit)
    {
        ContactPoint contact = col.GetContact(0);
        Vector3 pos = contact.point;
        
        Canvas pointObjectClone = Instantiate(collisionHitPrefab, new Vector3(pos.x, 3 ,pos.z),Quaternion.Euler(90,0,-90));
        pointObjectClone.GetComponentInChildren<TextMeshProUGUI>().text = pointsOnHit.ToString();
    }
}