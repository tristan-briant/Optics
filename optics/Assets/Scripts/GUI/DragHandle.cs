using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Animator animator;
    Vector3 startPos;

    public bool translation;
    public bool rotation;
    public bool clamped;

    public Transform handle;
    LineRenderer lr;
    float startAngle;
    Vector3 HandleInitialPosition;
    Vector3 newLocalPosition;

    const float ratioFineTranslation = 0.1f;

    void Start()
    {
        //animator = GetComponentInParent<Animator>();
        startPos = transform.localPosition;
        startPos.z = 0;
        startAngle = AngleFromXY(startPos.x, startPos.y);
        lr = GetComponent<LineRenderer>();
    }

    float angleMouse0;
    Vector3 positionMouse0;

    void OnMouseDown()
    {
        OnPointerDown(null);
    }

    void OnMouseUp()
    {
        OnPointerUp(null);
    }

    bool dragged = false;
    Vector3 offset;

    public void OnPointerDown(PointerEventData ev)
    {
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //if not clicked exactly in the center

        angleMouse0 = AngleFromXY(transform.position.x - handle.position.x,
                        transform.position.y - handle.position.y, -startAngle);

        HandleInitialPosition = handle.position;
        positionMouse0 = ratioFineTranslation * transform.position
                        + (1 - ratioFineTranslation) * HandleInitialPosition;


        //animator.SetBool("controled", true);
        lr.enabled = true;
        dragged = true;

        handle.GetComponent<HandleManager>().ConstrainTarget(translation, rotation);
    }

    public void OnPointerUp(PointerEventData ev)
    {
        handle.GetComponent<HandleManager>().ConstrainTarget(false, false);
        //animator.SetBool("controled", false);
        StartCoroutine("BackInPlace");
        dragged = false;
        handle.GetComponent<HandleManager>().ConstrainTarget(false, false);
    }

    IEnumerator BackInPlace()
    {
        Vector3 v = transform.localPosition;

        const float time = 0.1f;
        float timeInit = Time.time;
        while (Time.time < timeInit + time)
        {
            transform.localPosition = Vector3.Lerp(v, startPos, (Time.time - timeInit) / time);
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = startPos;
        lr.enabled = false;
    }

    float AngleFromXY(float x, float y, float offsetAngle = 0)
    {
        // return angle + offset entre 0 et 360°
        float angle = (Mathf.Atan2(y, x) * Mathf.Rad2Deg + offsetAngle) % 360;
        if (angle < 0) angle += 360;
        return angle;
    }

    void Update()
    {
        if (dragged)
        {

            /*if (rotation)
                animator.SetFloat("rotation", angleMouse1 / 360.0f);*/
        }

        lr.SetPosition(0, handle.position);
        lr.SetPosition(1, transform.position);

        transform.rotation = Quaternion.identity;
    }

    void XYLocalPosition(Vector3 newPos)
    {
        // Modifie uniquement le X et Y
        Vector3 pos = newPos;
        pos.z = transform.localPosition.z;
        transform.localPosition = pos;
    }

    void XYPosition(Vector3 newPos)
    {
        // Modifie uniquement le X et Y
        Vector3 pos = newPos;
        pos.z = transform.position.z;
        transform.position = pos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 direction;
        if (rotation)
            direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - handle.position - offset;
        else
            direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
        direction.z = 0;

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition)- offset;

        /*if (rotation)
            XYLocalPosition(startPos / startPos.magnitude * direction.magnitude);
        else
            XYPosition(direction);*/

        float angleMouse1 = AngleFromXY(direction.x, direction.y, -startAngle);

        if (rotation)
        {
            handle.GetComponent<HandleManager>().SetTargetDeltaAngle(angleMouse1 - angleMouse0, clamped);
            angleMouse0 = angleMouse1;
        }

        if (translation)
        {
            Vector3 positionMouse1 = ratioFineTranslation * transform.position
                                    + (1 - ratioFineTranslation) * HandleInitialPosition;

            handle.GetComponent<HandleManager>().SetTargetDeltaPosition(positionMouse1 - positionMouse0);
            positionMouse0 = positionMouse1;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
