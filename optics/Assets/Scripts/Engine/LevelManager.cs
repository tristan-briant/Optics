using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int Level = 0;

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

        GameEngine.instance.StartGameEngine();
        //StartCoroutine("StartWithDelay", 0.1f);
    }

    /*IEnumerator StartWithDelay(float second)
    {
        while (GameObject.Find("GameEngine") == null)
            yield return 0; // wait next frame
        //yield return new WaitForSeconds(second);
        //GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        GameEngine.instance.StartGameEngine();
        Debug.Log("Game engine Started!");

    }*/

}
