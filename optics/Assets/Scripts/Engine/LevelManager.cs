using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int Level = 0;

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        if (SceneManager.sceneCount == 1)
            SceneManager.LoadScene("MiniMap", LoadSceneMode.Additive);


        //GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        //GE.StartGameEngine();
        StartCoroutine("StartWithDelay", 0.1f);
    }

    IEnumerator StartWithDelay(float second)
    {
        while (GameObject.Find("GameEngine") == null)
            yield return 0; // wait next frame
        //yield return new WaitForSeconds(second);
        GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        GE.StartGameEngine();
        Debug.Log("Game engine Started!");

    }

}
