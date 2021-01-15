using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class DragAndDrop : MonoBehaviour
{
    static DragAndDrop itemSelected = null;

    GameObject Handle;   // Handle  or Options

    public bool CanTranslate;
    public bool CanRotate;
    

    private bool selected = false;
    public Vector3 positionSet;
    public float angleSet;

    float PressedTime;
    const float ClickDuration = 0.2f; // maximum click duration 
    const float LongClickDuration = 1.0f; // long click duration 

    private bool longClicking;

    Rigidbody2D rb;

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

    void EnableHandle(bool enable)
    {
        if (enable && !Handle)
        {
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Handle"));
            Handle.GetComponent<HandleManager>().Target = gameObject;
        }
        if (!enable && Handle)
            GameObject.Destroy(Handle);
    }

    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        positionSet = transform.position;
        angleSet = transform.eulerAngles.z;
    }

    void OnMouseDown()
    {
        offsetTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        PressedTime = Time.time;
    }

    void OnShortClick()
    {
        if (Selected)
            Selected = false;
        else
        {
            Selected = true;
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Handle"));
            Handle.GetComponent<HandleManager>().Target = gameObject;
        }
    }


    void OnLongClick()
    {
        if (Selected)
            Selected = false;
        else
        {
            Selected = true;
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Option"));
            Handle.GetComponent<OptionManager>().Target = gameObject;
        }
    }


    void OnMouseDrag()
    {
        Constrain(true, false);
        positionSet = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offsetTouch;
    }

    public void Constrain(bool translation, bool rotation)
    {
        //if (!rb) return;

        if (rotation && translation)
            rb.constraints = RigidbodyConstraints2D.None;

        if (!rotation && translation)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (rotation && !translation)
            rb.constraints = RigidbodyConstraints2D.FreezePosition;

        if (!rotation && !translation)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        float angleAct = transform.localEulerAngles.z;

        float deltaAngle = Mathf.DeltaAngle(angleAct, angleSet);
        Vector3 deltaPosition = positionSet - transform.position;

        rb.AddTorque(deltaAngle * 0.1f);
        rb.AddForce(deltaPosition * 100.0f);
    }

    public void LetFindPlace()
    { // if droped on other component find the good place
        StartCoroutine("FixPlace");
    }

    IEnumerator FixPlace()
    {
        yield return 0; // Wait for next frame to let collisions happen
        positionSet = transform.position;
        angleSet = transform.eulerAngles.z;
    }


}
