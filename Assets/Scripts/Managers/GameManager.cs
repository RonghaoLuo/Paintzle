using System;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action OnIntroFinish;
    public Action OnInitializePostStart, OnGameOver;
    public Action<float> OnCountdownChange;
    public Action OnTogglePauseGame;
    public Action OnResetManagers;
    public Action<RoomKeeper, Transform[], int, float> OnChallengeBegin;
    public Action<RoomKeeper> OnChallengeEnd;
    public Action OnChallengeComplete, OnChallengeFail;

    [Header("Challenge Settings")]
    [SerializeField] private float challengeCountdown = 120f;
    
    [Space(10)]
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private bool gameRunning;
    [SerializeField] private bool countdownRunning;
    [SerializeField] private bool debugStartGameOnStart = false;
    [SerializeField] private bool levelCompleted = false;
    [SerializeField] private PlayableDirector introDirector;
    [SerializeField] private PlayerInput playerInput;

    [ContextMenu("Debug Start Game")] private void DebugStartGame()
    {
        FinishIntro();
    }

    private float gameCountdownInitialNumber;
    private RoomKeeper roomOfChallenge;

    #region Settings
    private float mouseSensitivity = 1f;
    private float masterVolume = 1f;
    private float uiVolume = 1f;
    private float sfxVolume = 1f;
    private float musicVolume = 1f;
    #endregion

    public bool IsGameRunning
    {
        get { return gameRunning; }
    }

    public float MouseSensitivity
    {
        get { return mouseSensitivity; }
        set { mouseSensitivity = value; }
    }
    public float MasterVolume
    {
        get { return masterVolume; }
        set { masterVolume = value; }
    }
    public float UiVolume
    {
        get { return uiVolume; }
        set { uiVolume = value; }
    }
    public float SfxVolume
    {
        get { return sfxVolume; }
        set { sfxVolume = value; }
    }
    public float MusicVolume
    {
        get { return musicVolume; }
        set { musicVolume = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        gameCountdownInitialNumber = challengeCountdown;

        // For Testing
        if (debugStartGameOnStart) DebugStartGame();
    }

    private void OnDestroy()
    {
    }

    void Update()
    {
        // For Challenge
        if (challengeCountdown >= 0 && gameRunning && countdownRunning)
        {
            challengeCountdown -= Time.deltaTime;
            OnCountdownChange?.Invoke(challengeCountdown);

            if (challengeCountdown < 0)
            {
                ChallengeFail();
            }
        }

        #region User Inputs
        if (introDirector != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                introDirector.time = introDirector.duration;
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab) && gameStarted)
        {
            TogglePauseGame();
        }
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    countdownRunning = !countdownRunning;
        //}

        #endregion
    }

    public void InitializePostStart()
    {
        OnInitializePostStart?.Invoke();
    }

    public void FinishIntro()
    {
        OnIntroFinish?.Invoke();
        gameRunning = true;
    }

    public void TogglePauseGame()
    {
        gameRunning = !gameRunning;
        Time.timeScale = gameRunning ? 1f : 0f;  // Pause/unpause gameplay
        Cursor.visible = !gameRunning;
        Cursor.lockState = gameRunning ? CursorLockMode.Locked : CursorLockMode.None;

        playerInput.enabled = gameRunning;

        OnTogglePauseGame?.Invoke();
    }

    public void StartChallenge(RoomKeeper room, Transform[] spawnPoints, int maxNumOfPreys, 
        float spawnInterval)
    {
        // do some effects
        Debug.Log("Starting Challenge...");

        roomOfChallenge = room;
        OnChallengeBegin?.Invoke(room, spawnPoints, maxNumOfPreys, spawnInterval);
        countdownRunning = true;
    }

    public void StopChallenge(RoomKeeper room)
    {
        OnChallengeEnd?.Invoke(room);
        countdownRunning = false;
    }

    public void ChallengeComplete()
    {
        // To prevent duplicate calls
        if (levelCompleted) return;

        StopChallenge(roomOfChallenge);
        levelCompleted = true;
        // do some effect
        // win cutscene
        //win screen
        Debug.Log("You Won");
        OnChallengeComplete?.Invoke();
    }

    public void ChallengeFail()
    {
        if (levelCompleted) return;

        StopChallenge(roomOfChallenge);
        levelCompleted = true;
        // do some effect
        // challenge lost cutscene
        // challenge lost screen
        Debug.Log("You Lost");
        OnChallengeFail?.Invoke();
    }

    public void StartGame()
    {
        gameStarted = true;
        BootstrapManager.Instance.StartGame();
        InitializePostStart();
    }

    public void StartTest()
    {
        gameStarted = true;
        BootstrapManager.Instance.StartTest();
        Invoke(nameof(InitializePostStart), 1f);
    }

    public void ReturnToMainMenu()
    {
        gameStarted = false;
        BootstrapManager.Instance.ReturnToMainMenu();
        ResetAllManagers();
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        BootstrapManager.Instance.RestartGame();
        ResetAllManagers();
        Time.timeScale = 1f;
    }

    //public void SetSettingValues(float newSensitivity, float masterVol, float uiVol,
    //    float sfxVol, float musicVol)
    //{
    //    mouseSensitivity = newSensitivity;
    //    masterVolume = masterVol;
    //    uiVolume = uiVol;
    //    sfxVolume = sfxVol;
    //    musicVolume = musicVol;
    //}

    private void ResetAllManagers()
    {
        OnResetManagers?.Invoke();

        challengeCountdown = gameCountdownInitialNumber;
        countdownRunning = false;
        levelCompleted = false;

        playerInput = null;
        introDirector = null;
    }

    #region Register
    public void RegisterIntroDirector(PlayableDirector director)
    {
        introDirector = director;
    }

    public void RegisterPlayerInput(PlayerInput input)
    {
        playerInput = input;
    }

    #endregion
    
}
