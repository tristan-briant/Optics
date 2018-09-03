using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour {

    
    public bool dragging = false;
    public bool moving = false;
    public bool rotating = false;

    private float distance;
    public GameObject RotationCircle;
    public GameObject Handle;
    public bool selected=false;

    float PressedTime;
    const float ClickDuration = 0.1f; // maximum click duration 
    Rigidbody2D rb;


    Vector3 PositionOffset;
    Vector3 InitialPos;
    float angleOffset;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        InitialPos = transform.position;
        PositionOffset = rayPoint - transform.position;
        angleOffset = AngleFromXY(PositionOffset.x, PositionOffset.y)- transform.localEulerAngles.z / 180.0f * Mathf.PI;


        //transform.GetComponent<Rigidbody2D>().mass = transform.GetComponent<Rigidbody2D>().mass / 10;

        PressedTime = Time.time;
    }

    void OnMouseUp()
    {
        if (Time.time < PressedTime + ClickDuration) // C'est un click !
        {
            OnMouseClick();
        }
        else
        {
           OnMouseEndDrag();
        }

    }

    void OnMouseClick()
    {
        selected = !selected;
        if (Handle) Handle.SetActive(selected);
    }

    private void OnMouseDrag()
    {
        if (!dragging && Time.time > PressedTime + ClickDuration)
        {
            OnMouseBeginDrag();
        }
    }

    private void OnMouseBeginDrag()
    {
        dragging = true;
        
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == RotationCircle)
        {
            rotating = true;
            rb.constraints = RigidbodyConstraints2D.None;
        }
        else
        {
            moving = true;
            transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void OnMouseEndDrag()
    {
        moving = false;
        rotating = false;
        dragging = false;
        //transform.GetComponent<Rigidbody2D>().mass = transform.GetComponent<Rigidbody2D>().mass * 10;

        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }



    float AngleFromXY(float x, float y)
    {
        float angle= Mathf.Atan2(y, x);
        return angle;
    }


    public float angleAct;
    public float angleSet;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);

        Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();


        if (rotating)
        {
            Vector2 f;
            f.x = rayPoint.x - transform.position.x;
            f.y = rayPoint.y - transform.position.y;

            angleAct = transform.localEulerAngles.z / 180.0f * Mathf.PI;
            //angleAct = GetComponent<OpticalComponent>().angle;
            angleSet = (AngleFromXY(f.x, f.y)-angleOffset);
            if (angleSet > Mathf.PI) angleSet -= 2 * Mathf.PI;
            if (angleSet < -Mathf.PI) angleSet += 2 * Mathf.PI;

            //angleSet = angleSet * 0.2f;
            float angle = angleSet-angleAct;
            if (angle > Mathf.PI) angle -= 2 * Mathf.PI;
            if (angle < -Mathf.PI) angle += 2 * Mathf.PI;

            rb.AddTorque((angle)*1.0f);
           
        }

        if (moving)
        {
            
            if (rb)
            {
                Vector2 f;
                if (!selected)
                {
                    f.x = rayPoint.x - PositionOffset.x - transform.position.x;
                    f.y = rayPoint.y - PositionOffset.y - transform.position.y;
                }
                else
                {
                    const float r = 0.3f;
                    f.x = (r * (rayPoint.x - PositionOffset.x) + (1 - r) * InitialPos.x) - transform.position.x;
                    f.y = (r * (rayPoint.y - PositionOffset.y) + (1 - r) * InitialPos.y) - transform.position.y;
                }

                transform.GetComponent<Rigidbody2D>().AddForce(10*f);

            }
            
        }
    }
}
