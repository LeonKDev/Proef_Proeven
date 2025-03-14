using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
        [Tooltip("Optional intro segment that plays once before the main loop")]
        public AudioClip introClip;
    }

    [Header("Music Tracks")]
    [SerializeField] private MusicTrack menuMusic;
    [SerializeField] private MusicTrack gameplayMusic; 
    [SerializeField] private MusicTrack bossMusic;
    
    [Header("Transition Settings")]
    [SerializeField] private float crossFadeDuration = 1.5f;
    [SerializeField] private float bossTransitionTime = 60f;
    [SerializeField] private bool enableAutoBossTransition = true;
    
    private AudioSource _activeSource;
    private AudioSource _inactiveSource;
    private GameState _currentState = GameState.Menu;
    private bool _gameStarted = false;
    private bool _bossEncountered = false;
    private float _gameplayTimer = 0f;
    private bool _isPlayingIntro = false;
    
    private enum GameState { Menu, Gameplay, Boss }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // Initialize audio sources
            _activeSource = gameObject.AddComponent<AudioSource>();
            _inactiveSource = gameObject.AddComponent<AudioSource>();
            
            // Register for scene load events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            // If there's already an instance, destroy this one
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        PlayMenuMusic();
    }
    
    private void OnEnable()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnGameStarted += HandleGameStart;
            gm.OnReturnToMenu += PlayMenuMusic;
        }
    }

    private void OnDisable()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnGameStarted -= HandleGameStart;
            gm.OnReturnToMenu -= PlayMenuMusic;
        }
    }
    
    private void Update()
    {
        // Only check for auto boss transition if enabled
        if (enableAutoBossTransition && _gameStarted && !_bossEncountered)
        {
            _gameplayTimer += Time.deltaTime;
            
            if (_gameplayTimer >= bossTransitionTime)
            {
                PlayBossMusic();
                _bossEncountered = true;
            }
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void HandleGameStart()
    {
        PlayGameplayMusic();
        _gameStarted = true;
        _bossEncountered = false;
        _gameplayTimer = 0f;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Handle scene-specific music
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
    
    public void PlayMenuMusic()
    {
        if (_currentState != GameState.Menu && menuMusic.clip != null)
        {
            StartCoroutine(CrossFadeMusic(menuMusic.clip, menuMusic.volume, menuMusic.loop));
            _currentState = GameState.Menu;
        }
    }
    
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
    
    public void TriggerBossMusicManually()
    {
        PlayBossMusic();
        _bossEncountered = true;
    }
    
    private IEnumerator CrossFadeMusic(AudioClip newClip, float volume, bool loop)
    {
        if (_isPlayingIntro)
        {
            // Wait for intro to finish if one is playing
            yield return new WaitUntil(() => !_isPlayingIntro);
        }

        // Swap sources
        AudioSource temp = _activeSource;
        _activeSource = _inactiveSource;
        _inactiveSource = temp;
        
        _activeSource.clip = newClip;
        _activeSource.loop = loop;
        _activeSource.volume = 0;
        _activeSource.Play();
        
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
        
        _activeSource.volume = volume;
        
        if (_inactiveSource.isPlaying)
        {
            _inactiveSource.Stop();
            _inactiveSource.clip = null;
        }
    }
    
    private IEnumerator PlayMusicWithIntro(AudioClip introClip, AudioClip loopClip, float volume)
    {
        _isPlayingIntro = true;

        // Start with the intro
        _activeSource.clip = introClip;
        _activeSource.loop = false;
        _activeSource.volume = volume;
        _activeSource.Play();

        // Wait for the intro to complete
        float introDuration = introClip.length;
        yield return new WaitForSeconds(introDuration);

        _isPlayingIntro = false;
        
        // Transition to the main loop
        StartCoroutine(CrossFadeMusic(loopClip, volume, true));
    }
}