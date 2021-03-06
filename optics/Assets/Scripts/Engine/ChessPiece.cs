﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class ChessPiece : MonoBehaviour
{
    public static ChessPiece itemSelected = null;
    public static ChessPiece manipulated = null;

    public GameObject Options;
    [System.NonSerialized]
    public GameObject PG;
    [System.NonSerialized]
    public bool SnapToGrid = true;

    public GameObject RotatingPart;
    Rigidbody2D rigidbodyPart;

    GameObject Handle;   // Handle  or Options

    public bool CanTranslate { get => GetComponent<GenericComponent>().CanTranslate; set => GetComponent<GenericComponent>().CanTranslate = value; }
    public bool CanRotate { get => GetComponent<GenericComponent>().CanRotate; set => GetComponent<GenericComponent>().CanRotate = value; }

    public bool moving;
    private bool rotating;
    bool clamped;

    private bool selected = false;
    public Vector3 positionSet;
    public float angleSet;

    const float ClickDuration = 0.2f; // maximum click duration 
    const float LongClickDuration = 1.0f; // long click duration 

    //private bool longClicking;
    public float SnapIncrement = 0.25f;


    Vector3 offsetTouch;

    public bool Selected
    {
        get => selected;
        set
        {
            if (value == true)
            {
                if (itemSelected)// on deslectionne les autres
                    itemSelected.Selected = false;
                itemSelected = this;
                selected = true;
            }
            else
            {
                itemSelected = null;
                selected = false;
                if (Handle)
                    GameObject.Destroy(Handle);
            }
        }
    }

    public Vector3 PositionSet { get { return positionSet; } set => positionSet = value; }
    public static ChessPiece Manipulated { get => manipulated; set => manipulated = value; }

    static public void UnSelectAll()
    {
        if (itemSelected)// on deslectionne 
            itemSelected.Selected = false;

    }

    void EnableHandle(bool enable)
    {
        if (enable && !Handle)
        {
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Handle"));
            Handle.GetComponent<HandleManager>().CP = this;
        }
        if (!enable && Handle)
            GameObject.Destroy(Handle);
    }

    void Awake()
    {
        if (RotatingPart == null)
            RotatingPart = gameObject;

        Constrain(false, false);
        PositionSet = transform.position;
        angleSet = RotatingPart.transform.eulerAngles.z;
        PG = GameObject.FindGameObjectWithTag("Playground");
    }

    void OnMouseDown()
    {
        offsetTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    void OnShortClick()
    {
        if (Selected)
            Selected = false;
        else
        {
            if (CanRotate || CanTranslate || GameEngine.instance.isInEditMode)
            {
                Selected = true;
                Handle = Instantiate(Resources.Load<GameObject>("GUI/Handle"));
                Handle.GetComponent<HandleManager>().CP = this;
            }
            else
                Selected = false;

        }
    }

    void OnLongClick()
    {
        if (Handle)
            GameObject.Destroy(Handle);

        Selected = true;

        if (GameEngine.instance.isInEditMode && Options)
        {
            Handle = Instantiate(Options);
            Handle.GetComponent<OptionManager>().CP = this;
        }
    }

    void OnMouseBeginDrag()
    {
        offsetTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (!CanTranslate && !GameEngine.instance.isInEditMode) return;

        Manipulated = this;
        moving = true;
        Constrain(true, false);
    }

    void OnMouseDragging()
    {
        if (!CanTranslate && !GameEngine.instance.isInEditMode) return;

        positionSet = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offsetTouch;
        if (SnapToGrid)
            positionSet = MyMathf.Round(positionSet, GameEngine.instance.SnapIncrement);

    }

    void OnMouseEndDrag()
    {
        if (!CanTranslate && !GameEngine.instance.isInEditMode) return;

        PositionSet = MyMathf.Round(transform.position, SnapIncrement / 2);
        moving = false;
        LetFindPlace();
        Manipulated = null;
    }

    public void Constrain(bool translation, bool rotation)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) DestroyImmediate(rb);
        rb = RotatingPart.GetComponent<Rigidbody2D>();
        if (rb) DestroyImmediate(rb);

        if (translation || rotation)
        {
            positionSet = transform.position;
            angleSet = GetComponent<GenericComponent>().angle * Mathf.Rad2Deg;

            if (translation)
            {
                rigidbodyPart = gameObject.AddComponent<Rigidbody2D>();
                rigidbodyPart.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            if (rotation)
            {
                rigidbodyPart = RotatingPart.AddComponent<Rigidbody2D>();
                rigidbodyPart.constraints = RigidbodyConstraints2D.FreezePosition;
            }

            rigidbodyPart.gravityScale = 0;
            rigidbodyPart.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }


        clamped = !(rotation || translation);
    }

    public void LetFindPlace()
    { // if droped on other component find the good place
        StartCoroutine("FixPlace");
    }

    public void FixedUpdate()
    {
        if (!clamped)
        {
            float angleAct = RotatingPart.transform.localEulerAngles.z;

            float deltaAngle = Mathf.DeltaAngle(angleAct, angleSet);
            Vector3 deltaPosition = PositionSet - transform.position;

            //Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (PG)
            {
                RectTransform rt = (RectTransform)PG.transform;

                Vector3 pos = PositionSet;

                pos.x = Mathf.Clamp(PositionSet.x, rt.rect.xMin, rt.rect.xMax);
                pos.y = Mathf.Clamp(PositionSet.y, rt.rect.yMin, rt.rect.yMax);

                PositionSet = pos;
            }

            if (moving || ChessPiece.manipulated == this) // smooth moves
            {
                const float alpha = 0.5f;
                Vector2 goToPos = alpha * transform.position + (1 - alpha) * PositionSet;
                rigidbodyPart.MovePosition(goToPos);

                rigidbodyPart.MoveRotation(angleAct + alpha * deltaAngle);
            }
            else  // Go Final position
            {
                TeleportTo(new Vector2(PositionSet.x, PositionSet.y));
            }

            //UpdateCoordinates();  // Essentially for genericcomponent with no automatic update of trans and rotation


        }
    }

    IEnumerator FixPlace()
    {
        Constrain(true, false);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        Constrain(false, false);

        /*GenericComponent gc = GetComponent<GenericComponent>();
        if (gc)
            gc.UpdateCoordinates();*/
    }

    public void TeleportTo(Vector3 vector)
    {
        transform.position = vector;
        positionSet = vector;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
            rb.position = vector;
    }

    void UpdateCoordinates()
    {
        GenericComponent gc = GetComponent<GenericComponent>();
        gc?.UpdateCoordinates();
    }


}

