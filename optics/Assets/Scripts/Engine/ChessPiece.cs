using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class ChessPiece : MonoBehaviour
{
    public static ChessPiece itemSelected = null;
    public static ChessPiece manipulated = null;
    public GameObject Options;
    public GameObject PG;

    GameObject Handle;   // Handle  or Options

    public bool CanTranslate = true;
    public bool CanRotate = true;

    public bool moving;
    bool clamped;

    private bool selected = false;
    public Vector3 positionSet;
    public float angleSet;

    const float ClickDuration = 0.2f; // maximum click duration 
    const float LongClickDuration = 1.0f; // long click duration 

    private bool longClicking;


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

    void Start()
    {
        Constrain(false, false);
        PositionSet = transform.position;
        angleSet = transform.eulerAngles.z;
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
            Selected = true;
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Handle"));
            Handle.GetComponent<HandleManager>().CP = this;
        }
    }

    void OnLongClick()
    {
        if (Handle)
            GameObject.Destroy(Handle);

        Selected = true;
        //Handle = Instantiate(Resources.Load<GameObject>("GUI/Option"));
        if (Options)
            Handle = Instantiate(Options);
        else
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Option"));

        Handle.GetComponent<OptionManager>().CP = this;
    }

    void OnMouseBeginDrag()
    {
        //offsetTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Manipulated = this;
        moving = true;
        Constrain(true, false);
    }

    void OnMouseDragging()
    {
        positionSet = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offsetTouch;
    }

    void OnMouseEndDrag()
    {
        moving = false;
        LetFindPlace();
        Manipulated = null;
        //Constrain(false, false);
    }

    public void Constrain(bool translation, bool rotation)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rotation && translation)
            rb.constraints = RigidbodyConstraints2D.None;

        if (!rotation && translation)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (rotation && !translation)
            rb.constraints = RigidbodyConstraints2D.FreezePosition;

        if (!rotation && !translation)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (!translation)
            PositionSet = transform.position;
        if (!rotation)
            angleSet = transform.eulerAngles.z;

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
            float angleAct = transform.localEulerAngles.z;

            float deltaAngle = Mathf.DeltaAngle(angleAct, angleSet);
            Vector3 deltaPosition = PositionSet - transform.position;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (PG)
            {
                RectTransform rt = (RectTransform)PG.transform;

                Vector3 pos = PositionSet;

                pos.x = Mathf.Clamp(PositionSet.x, rt.rect.xMin, rt.rect.xMax);
                pos.y = Mathf.Clamp(PositionSet.y, rt.rect.yMin, rt.rect.yMax);

                PositionSet = pos;
            }

            rb.angularVelocity = deltaAngle / Time.fixedDeltaTime;
            rb.MovePosition(new Vector2(PositionSet.x, PositionSet.y));
        }

    }

    IEnumerator FixPlace()
    {
        Constrain(true, false);
        yield return new WaitForFixedUpdate();
        Constrain(false, false);
    }



}

