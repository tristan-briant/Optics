using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public ChessPiece CP;

    public Color EnableColor;
    public Color DisableColor;

    public GameObject RotationSwitch;
    public GameObject TranslationSwitch;

    Transform itemHolder;

    public virtual void Start()
    {
        transform.SetParent(CP.transform);


        transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;

        if (RotationSwitch)
            ChangeColorButton(CP.CanRotate, RotationSwitch);
        if (TranslationSwitch)
            ChangeColorButton(CP.CanTranslate, TranslationSwitch);

        itemHolder = transform.Find("ItemHolder");
        if (itemHolder)
        {
            itemHolder.GetComponent<Canvas>().sortingLayerName = "Handle";

            if (CP.transform.position.x >= Camera.main.transform.position.x && CP.transform.position.y >= Camera.main.transform.position.y)
            {
                ((RectTransform)itemHolder.transform).anchoredPosition = new Vector2(-0.5f, -0.5f);
                ((RectTransform)itemHolder.transform).pivot = new Vector2(1, 1);
            }
            if (CP.transform.position.x < Camera.main.transform.position.x && CP.transform.position.y >= Camera.main.transform.position.y)
            {
                ((RectTransform)itemHolder.transform).anchoredPosition = new Vector2(0.5f, -0.5f);
                ((RectTransform)itemHolder.transform).pivot = new Vector2(0, 1);
            }
            if (CP.transform.position.x >= Camera.main.transform.position.x && CP.transform.position.y < Camera.main.transform.position.y)
            {
                ((RectTransform)itemHolder.transform).anchoredPosition = new Vector2(-0.5f, 0.5f);
                ((RectTransform)itemHolder.transform).pivot = new Vector2(1, 0);
            }
            if (CP.transform.position.x < Camera.main.transform.position.x && CP.transform.position.y < Camera.main.transform.position.y)
            {
                ((RectTransform)itemHolder.transform).anchoredPosition = new Vector2(0.5f, 0.5f);
                ((RectTransform)itemHolder.transform).pivot = new Vector2(0, 0);
            }

            SetSize();
        }


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

    public void ToggleTranslation()
    {
        CP.CanTranslate = !CP.CanTranslate;
        ChangeColorButton(CP.CanTranslate, TranslationSwitch);
    }

    public void DeleteComposant()
    {
        CP.GetComponent<GenericComponent>().Delete();
        GameEngine.instance.UpdateComponentList();
    }

    void Update()
    {
        //transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;
        SetSize();

    }

    void SetSize()
    {
        if (itemHolder)
            itemHolder.localScale = Mathf.Clamp(Camera.main.orthographicSize / 4f, 0.1f, 10f) * Vector2.one;
    }

}
