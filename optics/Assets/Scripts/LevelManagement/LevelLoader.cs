using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float animationTime;

    public void Start()
    {
        if (SceneManager.sceneCount == 1)
            SceneManager.LoadScene("Engine", LoadSceneMode.Additive);

        transition.SetFloat("Speed", 1 / animationTime);
    }

    public void TransitionIn()
    {
        transition.SetTrigger("In");
    }

    public void TransitionOut()
    {
        transition.SetTrigger("Out");
    }

}
