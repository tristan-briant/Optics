using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public ChessPiece CP;

    public Slider slider;

    public Color EnableColor;
    public Color DisableColor;

    public GameObject RotationSwitch;
    public GameObject TranslationSwitch;

    void Start()
    {
        transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;

        ChangeColorButton(CP.CanRotate, RotationSwitch);
        ChangeColorButton(CP.CanTranslate, TranslationSwitch);

        ShowParameters();
    }

    void ChangeColorButton(bool enable, GameObject button)
    {
        if (enable)
            button.GetComponent<Image>().color = EnableColor;
        else
            button.GetComponent<Image>().color = DisableColor;
    }


    void Update()
    {
        transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;
    }


    public void ToggleRotation()
    {
        CP.CanRotate = !CP.CanRotate;
        ChangeColorButton(CP.CanRotate, RotationSwitch);
    }

    public void ToggleRTranslation()
    {
        CP.CanTranslate = !CP.CanTranslate;
        ChangeColorButton(CP.CanTranslate, TranslationSwitch);
    }

    public void DeleteComposant()
    {
        CP.GetComponent<OpticalComponent>().Delete();
        GameObject.DestroyImmediate(CP.gameObject);
        GameObject.DestroyImmediate(gameObject);
        FindObjectOfType<GameEngine>().UpdateComponentList();
    }


    public void ShowParameters()
    {
        if (CP.GetComponent<LightSource>())
        {
            //CP.GetComponent<LightSource>().Invoke("Div", 0);
            slider.onValueChanged.AddListener(delegate { CP.GetComponent<LightSource>().Div = slider.value; });
        }
    }

}
