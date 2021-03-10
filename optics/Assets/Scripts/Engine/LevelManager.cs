using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string CurrentLevel = "";
    public int CurrentLevelNumber = -1;
    //public List<LevelDescriptor> Levels = new List<LevelDescriptor>();
    public List<string> Levels = new List<string>();
    public GameObject ScoreBoard;
    public GameObject PauseMenu;

    #region SINGLETON
    public static LevelManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }
    #endregion

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        if (SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene("MiniMap", LoadSceneMode.Additive);
            CurrentLevel = "MiniMap";
        }
        else
            GameEngine.instance.StartGameEngine();

        string output = "";
        if (SceneManager.sceneCount > 0)
        {
            for (int n = 0; n < SceneManager.sceneCount; ++n)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                output += scene.name;
                output += scene.isLoaded ? " (Loaded, " : " (Not Loaded, ";
                output += scene.isDirty ? "Dirty, " : "Clean, ";
                output += scene.buildIndex >= 0 ? " in build)\n" : " NOT in build)\n";




                if (scene.name.Contains("Level"))
                    CurrentLevelNumber = Levels.FindIndex(a => a == scene.name);
                if (scene.name == "MiniMap")
                    CurrentLevel = "MiniMap";

            }
        }
        else
        {
            output = "No open Scenes.";
        }

        Debug.Log(output);

        //StartCoroutine("StartWithDelay", 0.1f);
    }

    public void SelectNextLevel(int i = +1)  // -1 for prev level
    {
        SelectLevel(CurrentLevelNumber + i);
    }

    public void SelectLevel(int n)
    {
        if (n >= 0 && n < Levels.Count)
        {
            Debug.Log("Loading: " + Levels[n]);
            StartCoroutine(Transition(CurrentLevel, Levels[n]));

            CurrentLevel = Levels[n];
            CurrentLevelNumber = n;
        }
    }

    public void BackToMenu()
    {
        StartCoroutine(Transition(CurrentLevel, "MiniMap"));
        CurrentLevel = "MiniMap";
        CurrentLevelNumber = -1;
    }

    public bool isNextLevelAccessible { get => (CurrentLevelNumber >= 0 && CurrentLevelNumber < Levels.Count - 1); }
    public bool isPrevLevelAccessible { get => CurrentLevelNumber > 0; }


    IEnumerator LoadAsync(string LevelName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(LevelName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        GameEngine.instance.StartGameEngine();
    }

    IEnumerator Transition(string scene1, string scene2)
    {
        LevelLoader lvloader;

        Time.timeScale = 0.0f;

        lvloader = FindObjectOfType<LevelLoader>();
        if (lvloader)
        {
            lvloader.TransitionOut();
            yield return new WaitForSecondsRealtime(lvloader.animationTime);
        }
        if (GameEngine.instance.PlayMode != GameEngine.Mode.Inactive)
            GameEngine.instance.StopGameEngine();


        SceneManager.UnloadSceneAsync(scene1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene2, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (lvloader)
        {
            lvloader.TransitionIn();
            yield return new WaitForSecondsRealtime(lvloader.animationTime);
        }
        Time.timeScale = 1.0f;


        if (scene2.Contains("Level"))
            GameEngine.instance.StartGameEngine(GameEngine.Mode.Play);
        else if (scene2.Contains("SandBox"))
            GameEngine.instance.StartGameEngine(GameEngine.Mode.Edit);
        else
            GameEngine.instance.StopGameEngine();

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameEngine.instance.isInInactiveMode)
            {
                if (PauseMenu.activeSelf)
                    Resume();
                else
                    Pause();
            }
            else
            {
                Application.Quit();
            }
        }

    }

    public void ShowsScoreBoard()
    {
        ScoreBoard.SetActive(true);
        ScoreBoard.GetComponent<Animator>().Update(0);

    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
