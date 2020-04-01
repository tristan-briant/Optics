using UnityEngine;

public class DragAndDrop : MonoBehaviour
{


    public bool dragging = false;
    public bool moving = false;
    public bool rotating = false;

    private float distance;
    public GameObject RotationCircle;
    public GameObject Handle;
    public bool selected = false;

    float PressedTime;
    const float ClickDuration = 0.2f; // maximum click duration 
    Rigidbody2D rb;

    public Vector3 PositionSet;

    Vector3 PositionOffset;
    Vector3 InitialPos;
    public float angleMouse0;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        PositionSet = transform.position;
        angleSet = transform.eulerAngles.z;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        InitialPos = transform.position;
        PositionOffset = rayPoint - transform.position;
        angleMouse0 = Mathf.Atan2(PositionOffset.y, PositionOffset.x) * Mathf.Rad2Deg;
        angleSet = transform.localEulerAngles.z;

        PressedTime = Time.time;
    }

    void OnMouseUp()
    {
        if (Time.time < PressedTime + ClickDuration) // C'est un click !
        {
            OnMouseClick();
        }

        Constrain(false,false);

    }


    void OnMouseClick()
    {
        selected = !selected;
        if (selected && !Handle)
        {
            Handle = Instantiate(Resources.Load<GameObject>("GUI/Handle"));

            Handle.transform.SetParent(transform);
            Handle.GetComponent<HandleManager>().Target = gameObject;
        }
        if (!selected && Handle)
            GameObject.Destroy(Handle);
    }


    void OnMouseDrag()
    {
        Constrain(true, false);
        PositionSet = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    public void Constrain(bool translation, bool rotation)
    {
        if (rotation && translation)
            rb.constraints = RigidbodyConstraints2D.None;

        if (!rotation && translation)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (rotation && !translation)
            rb.constraints = RigidbodyConstraints2D.FreezePosition;

        if (!rotation && !translation)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }


    public float angleAct;
    public float angleSet;
   
    void Update()
    {
        angleAct = transform.localEulerAngles.z;

        float deltaAngle = Mathf.DeltaAngle(angleAct, angleSet);
        Vector3 deltaPosition = PositionSet - transform.position;

        rb.AddTorque(deltaAngle * 0.1f);
        rb.AddForce(deltaPosition * 100.0f);
    }

}
