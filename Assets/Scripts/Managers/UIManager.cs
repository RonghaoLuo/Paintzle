using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Action<Color> OnSelectPaint;
    public Action<int> OnSelectPaintWithIndex;
    public Action<int, Color> OnCollectPaint;
    public Action<bool> OnToggleInteractionPrompt;
    public Action<float> OnPowerUpCountdown;
    public Action<string, Color> OnUpdateHotbarText;
    public Action OnPowerUpStart;
    public Action OnPowerUpEnd;
    public Action<string> OnPopup;

    [SerializeField] private UIPlayerHUD playerHUD;
    [SerializeField] private UIPauseMenu pauseMenu;

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
        GameManager.Instance.OnCountdownChange += UpdateCountdown;
        GameManager.Instance.OnIntroFinish += HandleIntroFinish;
        GameManager.Instance.OnTogglePauseGame += TogglePauseMenu;
        GameManager.Instance.OnResetManagers += ResetManager;
        GameManager.Instance.OnChallengeComplete += HandleChallengeComplete;
        GameManager.Instance.OnChallengeFail += HandleChallengeFail;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnCountdownChange -= UpdateCountdown;
        GameManager.Instance.OnIntroFinish -= HandleIntroFinish;
        GameManager.Instance.OnTogglePauseGame -= TogglePauseMenu;
        GameManager.Instance.OnResetManagers -= ResetManager;
        GameManager.Instance.OnChallengeComplete -= HandleChallengeComplete;
        GameManager.Instance.OnChallengeFail -= HandleChallengeFail;
    }

    private void UpdateCountdown(float timesLeft)
    {
        if (playerHUD == null)
        {
            return;
        }

        playerHUD.SetCountdown(timesLeft);
    }

    private void HandleIntroFinish()
    {
        if (playerHUD == null) return;

        playerHUD.gameObject.SetActive(true);
    }

    private void TogglePauseMenu()
    {
        if (pauseMenu == null) return;

        if (GameManager.Instance.IsGameRunning)
        {
            pauseMenu.gameObject.SetActive(false);
        }
        else
        {
            pauseMenu.gameObject.SetActive(true);
        }
    }

    private void ResetManager()
    {
        playerHUD = null;
        pauseMenu = null;
    }

    private void HandleChallengeComplete()
    {
        OnUpdateHotbarText?.Invoke("Challenge Complete!\nPress Tab for Menu", Color.gold);
    }

    private void HandleChallengeFail()
    {
        OnUpdateHotbarText?.Invoke("Challenge Failed!\nPress Tab for Menu", Color.red);
    }

    public void EnableInteractionPrompt()
    {
        OnToggleInteractionPrompt?.Invoke(true);
    }

    public void DisableInteractionPrompt()
    {
        OnToggleInteractionPrompt?.Invoke(false);
    }

    public void StartPowerUpCountdown(float duration)
    {
        OnPowerUpCountdown?.Invoke(duration);
    }

    #region Register Methods
    public void RegisterPlayerHUD(UIPlayerHUD hud)
    {
        playerHUD = hud;
    }

    public void RegisterPauseMenu(UIPauseMenu menu)
    {
        pauseMenu = menu;
    }

    #endregion
}
