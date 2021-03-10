using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSetup : MonoBehaviour
{
    public GameObject PrevButton;
    public GameObject NextButton;


    void OnEnable()
    {
        LevelManager levelManager = LevelManager.instance;
        if (NextButton)
            NextButton.SetActive(levelManager.isNextLevelAccessible);
        if (PrevButton)
            PrevButton.SetActive(levelManager.isPrevLevelAccessible);
    }


}
