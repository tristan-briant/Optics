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

    public bool CanTranslate { set { canTranslate = value; ChangeVisual(); } get => canTranslate; }
    public bool CanRotate { set { canRotate = value; ChangeVisual(); } get => canRotate; }

    [System.NonSerialized]
    public float cos, sin, param; // vecteur directeur
    [System.NonSerialized]
    public bool hasChanged = true;



    virtual public void Start()
    {
        ChangeVisual();
    }

    public virtual void Delete()
    {
        DestroyImmediate(gameObject);
    }

    public virtual void ChangeVisual()
    {
        Animator animator = null;
        Transform baseGo = transform.Find("Base");
        if (baseGo)
            animator = baseGo.GetComponent<Animator>();
        if (animator)
            animator.SetBool("Movable", CanTranslate || CanRotate);

    }

    public virtual void ClampParameters() // Used to clamp all params between min and max, used in editor mode
    {

    }

    public virtual void OnInstantiate() //Setup position, rotation and many mmore when instantiated (from designer)
    {
        GetComponent<ChessPiece>().positionSet = new Vector2(x, y);
        transform.position = new Vector2(x, y);
        transform.localScale = Vector3.one;
        GameObject RP = GetComponent<ChessPiece>().RotatingPart;
        RP.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90f);

        hasChanged = true;

        //Animator anim = GetComponent<Animator>();
        //if (anim) anim.Play(0);
    }

}
