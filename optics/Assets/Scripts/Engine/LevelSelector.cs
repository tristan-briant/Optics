using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

    GameEngine GE;
    int CurrentLevel = 0;

    private void Start()
    {
        GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
    }

    public void SelectLevel(int n)
    {
        Debug.Log("Loading: " + "Level" + n);
        SceneManager.LoadScene("Level" + n, LoadSceneMode.Additive);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level" + n));
        SceneManager.UnloadSceneAsync("MiniMap");
        CurrentLevel = n;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GE.running)
            {
                GE.ResetLightRay();
                SceneManager.UnloadSceneAsync("Level" + CurrentLevel);
                SceneManager.LoadScene("MiniMap", LoadSceneMode.Additive);
                GE.running = false;
            }
            else
            {
                Application.Quit();
            }
        }
        
    }
}
