using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GenericComponent : MonoBehaviour
{
    public string prefabPath;
    public float x = 0, y = 0; // position
    public float angle = 0;

    public bool canTranslate = true;
    public bool canRotate = true;

    public bool CanTranslate { set { canTranslate = value; UpdateChessPiecePosition(); ChangeVisual(); } get => canTranslate; }
    public bool CanRotate { set { canRotate = value; UpdateChessPiecePosition(); ChangeVisual(); } get => canRotate; }

    [System.NonSerialized]
    public float cos, sin, param; // vecteur directeur
    [System.NonSerialized]
    public bool hasChanged = true;


    virtual public void ComputeDir()
    {
        Vector3 pos = transform.position;
        x = pos.x;
        y = pos.y;

        angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        cos = Mathf.Cos(angle);
        sin = Mathf.Sin(angle);
        param = -sin * x + cos * y;
    }

    virtual public void Start()
    {
        UpdateCoordinates();
        ChangeVisual();
    }

    public virtual void Delete()
    {
        DestroyImmediate(gameObject);
    }

    //[ContextMenu("ChangeVisual")]
    public virtual void ChangeVisual()
    {
        Animator animator = null;
        Transform baseGo = transform.Find("Base");
        if (baseGo)
            animator = baseGo.GetComponent<Animator>();
        if (animator)
            animator.SetBool("Movable", CanTranslate || CanRotate);

    }

    public virtual void ClampParameters() { }// Used to clamp all params between min and max, used in editor mode

    virtual public void UpdateCoordinates()
    {
        Vector3 pos = transform.position;
        x = pos.x;
        y = pos.y;

        angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

    }

    public void UpdateChessPiecePosition()
    {
        ChessPiece CP = GetComponent<ChessPiece>();
        if (CP)
        {
            CP.positionSet = transform.position;
            CP.angleSet = angle * Mathf.Rad2Deg;
        }

    }

    virtual public string ToJson()
    {
        UpdateCoordinates(); // to be sure x,y,angle are up to date for non OpticalComponent
        return JsonUtility.ToJson(this);
    }

    virtual public void FromJson(string str)
    {
        JsonUtility.FromJsonOverwrite(str, this);

        GameObject PGComponents = GameObject.Find("Playground/Components");
        transform.SetParent(PGComponents.transform);

        OnInstantiate();

    }

    public virtual void OnInstantiate() //Setup position, rotation and many mmore when instantiated (from designer)
    {
        ChessPiece CP = GetComponent<ChessPiece>();

        CP.positionSet = new Vector2(x, y);
        transform.position = new Vector2(x, y);
        transform.localScale = Vector3.one;
        GameObject RP = CP.RotatingPart;
        if (RP)
            RP.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        CP.angleSet = angle * Mathf.Rad2Deg;

        hasChanged = true;
    }
}
