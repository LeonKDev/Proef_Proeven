using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    //References for UI
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
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
        
        scoreText.text = Score.ToString("00000");
        highScoreText.text = _highScore.ToString("00000");
    }
    
    /// <summary>
    /// <c>AddPoints</c> Adds the amount to the current score.
    /// </summary>
    public void AddPoints(int amount)
    {
        // adds points to the score
        Score += amount;
        scoreText.text = Score.ToString("00000");
        
        if (_highScore > Score)
            return;

        // saves the highest score outside the game loop
        PlayerPrefs.SetInt("highscore", Score);
    }
    
    /// <summary>
    /// <c>InstantiateScoreObject</c> instantiates a score object at the current collision point 
    /// </summary> 
    public void InstantiateScoreObject(Collision col, int pointsOnHit)
    {
        ContactPoint contact = col.GetContact(0);
        Vector3 pos = contact.point;
        
        Canvas pointObjectClone = Instantiate(collisionHitPrefab, new Vector3(pos.x, 3 ,pos.z),Quaternion.Euler(90,0,-90));
        pointObjectClone.GetComponentInChildren<TextMeshProUGUI>().text = pointsOnHit.ToString();
    }
}