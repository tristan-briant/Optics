using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int CurrentLevel = 0;

    public static LevelManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        if (SceneManager.sceneCount == 1)
            SceneManager.LoadScene("MiniMap", LoadSceneMode.Additive);
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
                    CurrentLevel = int.Parse(scene.name.Substring(5));
            }
        }
        else
        {
            output = "No open Scenes.";
        }

        Debug.Log(output);

        //StartCoroutine("StartWithDelay", 0.1f);
    }

    public void SelectLevel(int n)
    {
        Debug.Log("Loading: " + "Level" + n);
        CurrentLevel = n;
        StartCoroutine(Transition("MiniMap", "Level" + CurrentLevel));
    }

    IEnumerator LoadAsync(int levelNumber)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Level" + levelNumber, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        GameEngine.instance.StartGameEngine();
    }

    IEnumerator Transition(string scene1, string scene2)
    {
        LevelLoader lvloader = FindObjectOfType<LevelLoader>();
        if (lvloader)
        {
            lvloader.StartTransition();
            yield return new WaitForSeconds(lvloader.animationTime);
        }
        if (GameEngine.instance.running)
            GameEngine.instance.StopGameEngine();

        SceneManager.UnloadSceneAsync(scene1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene2, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (GameObject.Find("Playground"))
            GameEngine.instance.StartGameEngine();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameEngine.instance.running)
            {
                StartCoroutine(Transition("Level" + CurrentLevel, "MiniMap"));
            }
            else
            {
                Application.Quit();
            }
        }

    }

}
