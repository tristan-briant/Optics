using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float animationTime;

    public void StartTransition()
    {
        transition.SetTrigger("Start");
    }
}
