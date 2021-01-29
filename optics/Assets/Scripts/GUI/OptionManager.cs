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

    public virtual void Start()
    {
        transform.SetParent(CP.transform);

        Transform itemHolder = transform.Find("ItemHolder");
        itemHolder.GetComponent<Canvas>().sortingLayerName = "Handle";

        transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;

        if (RotationSwitch)
            ChangeColorButton(CP.CanRotate, RotationSwitch);
        if (TranslationSwitch)
            ChangeColorButton(CP.CanTranslate, TranslationSwitch);
    }

    void ChangeColorButton(bool enable, GameObject button)
    {
        if (enable)
            button.GetComponent<Image>().color = EnableColor;
        else
            button.GetComponent<Image>().color = DisableColor;
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
        GameObject.DestroyImmediate(gameObject);
        GameObject.DestroyImmediate(CP.gameObject);
        FindObjectOfType<GameEngine>().UpdateComponentList();
    }

    void Update()
    {
        //transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;
    }

}
