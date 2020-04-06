using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        Application.targetFrameRate = 30;
        GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        StartCoroutine("StartWithDelay", 0.1f);
    }

    IEnumerator StartWithDelay(float second)
    {
        yield return new WaitForSeconds(second);
        GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        GE.StartGameEngine();
        Debug.Log("Game engine Started!");

    }

}
