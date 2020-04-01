using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeveManager : MonoBehaviour
{

    void Start() // Lance le GameEngine quand le niveau est prêt
    {
        GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
    }


    IEnumerator StartGameEngine()
    {
        yield return new WaitForSeconds(1);
        GameEngine GE = GameObject.Find("GameEngine").GetComponent<GameEngine>();
        GE.StartGameEngine();
    }

}
