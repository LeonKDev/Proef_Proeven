using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    private int _score = 0;
    private int _highScore = 0;

    //References for UI
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private Canvas collisionHitPefab;
    
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
        
        scoreText.text = Score.ToString("00000");
        highscoreText.text = _highScore.ToString("00000");
    }
    
    /// <summary>
    /// Class <c>AddPoints</c> Adds amount to the current score.
    /// </summary>
    public void AddPoints(int amount)
    {
        Score += amount;
        scoreText.text = Score.ToString("00000");
        
        if (_highScore > Score)
            return;
        
        // saves the highest score outside the game loop
        PlayerPrefs.SetInt("highscore", Score);
    }
    public void InstantiateScoreObject(Collision col, int pointsOnHit)
    {
        ContactPoint contact = col.GetContact(0);
        Vector3 pos = contact.point;
        
        Canvas pointObjectClone = Instantiate(collisionHitPefab, new Vector3(pos.x, 3 ,pos.z),Quaternion.Euler(90,0,-90));
        pointObjectClone.GetComponentInChildren<TextMeshProUGUI>().text = pointsOnHit.ToString();
    }
}