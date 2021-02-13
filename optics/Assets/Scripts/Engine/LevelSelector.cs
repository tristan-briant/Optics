using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public int CurrentLevel = 0;

    void Start()
    {
        if (SceneManager.sceneCount == 1)
            SceneManager.LoadScene("MiniMap", LoadSceneMode.Additive);
        else
            GameEngine.instance.StartGameEngine();
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
