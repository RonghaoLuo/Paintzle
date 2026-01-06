using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private int mainMenuSceneIndex = 1;
    [SerializeField] private int levelSceneIndex = 2;
    [SerializeField] private int testSceneIndex = 3;

    public static BootstrapManager Instance;

    private int currentSceneIndex = -1;

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

    private void Start()
    {
        StartCoroutine(LoadSceneFlow(mainMenuSceneIndex, music: "bgm_mainmenu"));
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadSceneFlow(mainMenuSceneIndex, music: "bgm_mainmenu"));
    }

    public void StartGame()
    {
        StartCoroutine(LoadSceneFlow(levelSceneIndex, music: "bgm_level_positive"));
    }

    public void StartTest()
    {
        StartCoroutine(LoadSceneFlow(testSceneIndex, music: "bgm_level_positive"));
    }

    public void RestartGame()
    {
        // Reload whatever is currently active as gameplay
        if (currentSceneIndex < 0) return;
        StartCoroutine(LoadSceneFlow(currentSceneIndex, music: "bgm_level_positive"));
    }

    private IEnumerator LoadSceneFlow(int targetBuildIndex, string music)
    {
        // 1) Unload current (if any)
        if (currentSceneIndex >= 0)
        {
            var unloadOp = SceneManager.UnloadSceneAsync(currentSceneIndex);
            if (unloadOp != null)
                yield return unloadOp;
        }

        // 2) Load target additively
        var loadOp = SceneManager.LoadSceneAsync(targetBuildIndex, LoadSceneMode.Additive);
        // Optional: ensure it activates when ready (default is true)
        loadOp.allowSceneActivation = true;
        yield return loadOp;

        // 3) Set active scene to the newly loaded one (IMPORTANT for baked data consistency)
        var loadedScene = SceneManager.GetSceneByBuildIndex(targetBuildIndex);
        if (loadedScene.IsValid() && loadedScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        currentSceneIndex = targetBuildIndex;

        // 4) Music
        AudioManager.Instance.PlayMusic(music, true);
    }
}
