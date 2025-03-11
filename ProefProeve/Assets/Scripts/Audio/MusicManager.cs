using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    // Singleton pattern
    public static MusicManager Instance { get; private set; }
    
    [System.Serializable]
    public class MusicTrack
    {
        public string trackName;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool loop = true;
        [Tooltip("For tracks with intro segment")]
        public AudioClip introClip;
    }

    [Header("Music Tracks")]
    [SerializeField] private MusicTrack menuMusic;
    [SerializeField] private MusicTrack gameplayMusic; 
    [SerializeField] private MusicTrack bossMusic;
    
    [Header("Transition Settings")]
    [SerializeField] private float crossFadeDuration = 1.5f;
    [SerializeField] private float bossTransitionTime = 60f; // Time in seconds before boss music starts (adjust based on gameplay)
    
    // Audio sources for crossfading
    private AudioSource _activeSource;
    private AudioSource _inactiveSource;
    
    // Game state tracking
    private enum GameState { Menu, Gameplay, Boss }
    private GameState _currentState = GameState.Menu;
    private bool _gameStarted = false;
    private bool _bossEncountered = false;
    private float _gameplayTimer = 0f;
    
    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create audio sources for crossfading
            _activeSource = gameObject.AddComponent<AudioSource>();
            _inactiveSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Add scene load event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
        // Start with menu music
        PlayMenuMusic();
    }
    
    private void Update()
    {
        // Track gameplay time for boss music transition
        if (_gameStarted && !_bossEncountered)
        {
            _gameplayTimer += Time.deltaTime;
            
            // Check if it's time for boss music
            if (_gameplayTimer >= bossTransitionTime)
            {
                PlayBossMusic();
                _bossEncountered = true;
            }
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Detect scene changes to adjust music
        // You may need to modify this logic based on your scene names/indexes
        if (scene.name == "MainMenu")
        {
            PlayMenuMusic();
            _gameStarted = false;
            _bossEncountered = false;
            _gameplayTimer = 0f;
        }
        else if (scene.name != "MainMenu" && !_gameStarted)
        {
            PlayGameplayMusic();
            _gameStarted = true;
        }
    }
    
    /// <summary>
    /// Switches to menu music with crossfade
    /// </summary>
    public void PlayMenuMusic()
    {
        if (_currentState != GameState.Menu && menuMusic.clip != null)
        {
            StartCoroutine(CrossFadeMusic(menuMusic.clip, menuMusic.volume, menuMusic.loop));
            _currentState = GameState.Menu;
        }
    }
    
    /// <summary>
    /// Switches to gameplay music with crossfade, handles intro if available
    /// </summary>
    public void PlayGameplayMusic()
    {
        if (_currentState != GameState.Gameplay && gameplayMusic.clip != null)
        {
            if (gameplayMusic.introClip != null)
            {
                StartCoroutine(PlayMusicWithIntro(gameplayMusic.introClip, gameplayMusic.clip, gameplayMusic.volume));
            }
            else
            {
                StartCoroutine(CrossFadeMusic(gameplayMusic.clip, gameplayMusic.volume, gameplayMusic.loop));
            }
            _currentState = GameState.Gameplay;
        }
    }
    
    /// <summary>
    /// Switches to boss music with crossfade, handles intro if available
    /// </summary>
    public void PlayBossMusic()
    {
        if (_currentState != GameState.Boss && bossMusic.clip != null)
        {
            if (bossMusic.introClip != null)
            {
                StartCoroutine(PlayMusicWithIntro(bossMusic.introClip, bossMusic.clip, bossMusic.volume));
            }
            else
            {
                StartCoroutine(CrossFadeMusic(bossMusic.clip, bossMusic.volume, bossMusic.loop));
            }
            _currentState = GameState.Boss;
        }
    }
    
    /// <summary>
    /// Manually trigger boss music (can be called from boss encounter scripts)
    /// </summary>
    public void TriggerBossMusicManually()
    {
        PlayBossMusic();
        _bossEncountered = true;
    }
    
    /// <summary>
    /// Handles crossfading between music tracks
    /// </summary>
    private IEnumerator CrossFadeMusic(AudioClip newClip, float volume, bool loop)
    {
        // Swap sources so active becomes inactive and vice versa
        AudioSource temp = _activeSource;
        _activeSource = _inactiveSource;
        _inactiveSource = temp;
        
        // Set up new clip
        _activeSource.clip = newClip;
        _activeSource.loop = loop;
        _activeSource.volume = 0;
        _activeSource.Play();
        
        // Fade out old music while fading in new music
        float timeElapsed = 0;
        
        while (timeElapsed < crossFadeDuration)
        {
            float normalizedTime = timeElapsed / crossFadeDuration;
            
            _activeSource.volume = Mathf.Lerp(0, volume, normalizedTime);
            
            if (_inactiveSource.isPlaying)
            {
                _inactiveSource.volume = Mathf.Lerp(_inactiveSource.volume, 0, normalizedTime);
            }
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        // Ensure final volumes are set correctly
        _activeSource.volume = volume;
        
        // Stop the old music
        _inactiveSource.Stop();
        _inactiveSource.clip = null;
    }
    
    /// <summary>
    /// Plays an intro segment and then transitions to the looping main track
    /// </summary>
    private IEnumerator PlayMusicWithIntro(AudioClip introClip, AudioClip loopClip, float volume)
    {
        // Fade out any currently playing music
        if (_activeSource.isPlaying)
        {
            StartCoroutine(FadeOut(_activeSource, crossFadeDuration));
        }
        
        if (_inactiveSource.isPlaying)
        {
            StartCoroutine(FadeOut(_inactiveSource, crossFadeDuration));
        }
        
        // Wait for fade out
        yield return new WaitForSeconds(crossFadeDuration);
        
        // Play intro
        _activeSource.clip = introClip;
        _activeSource.loop = false;
        _activeSource.volume = volume;
        _activeSource.Play();
        
        // Wait for intro to finish
        while (_activeSource.isPlaying)
        {
            yield return null;
        }
        
        // Transition to looping part
        _activeSource.clip = loopClip;
        _activeSource.loop = true;
        _activeSource.volume = volume;
        _activeSource.Play();
    }
    
    /// <summary>
    /// Helper method to fade out an audio source
    /// </summary>
    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            source.volume = Mathf.Lerp(startVolume, 0, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        source.Stop();
    }
}