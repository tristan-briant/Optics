using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
       if (SceneManager.sceneCount == 1)
            SceneManager.LoadScene("Engine", LoadSceneMode.Additive); 
    }

}
