using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSnapMode : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(0).GetComponent<Toggle>().isOn
        = GameEngine.instance.snapMode == GameEngine.GridSnapMode.Fine;

        transform.GetChild(1).GetComponent<Toggle>().isOn
        = GameEngine.instance.snapMode == GameEngine.GridSnapMode.Gross;
    }


    public void SetSnapModeFine(bool newValue)
    {
        if (newValue) GameEngine.instance.snapMode = GameEngine.GridSnapMode.Fine;
        else GameEngine.instance.snapMode = GameEngine.GridSnapMode.None;
    }

    public void SetSnapModeGross(bool newValue)
    {
        if (newValue) GameEngine.instance.snapMode = GameEngine.GridSnapMode.Gross;
        else GameEngine.instance.snapMode = GameEngine.GridSnapMode.None;
    }

}
