using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class DimensionHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool left;
    public bool right;
    public bool up;
    public bool down;

    public Transform handle;
    Vector3 HandleInitialPosition;
    Vector3 newLocalPosition;

    bool dragged;

    const float ratioFineTranslation = 0.25f;

    void Start()
    {
        ResetPosition();
    }

    public void ResetPosition()
    {
        if (!dragged)
            transform.localPosition = handle.GetComponent<OptionWall>().GetHandleLocalPosition(left, right, up, down);
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

    Vector3 offset;

    public void OnPointerDown(PointerEventData ev)
    {
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        HandleInitialPosition = handle.position;
        positionMouse0 = transform.localPosition;



        dragged = true;
    }

    public void OnPointerUp(PointerEventData ev)
    {
        StartCoroutine("BackInPlace");
        dragged = false;
    }

    IEnumerator BackInPlace()
    {
        Vector3 v = transform.localPosition;
        Vector2 finalPosition = handle.GetComponent<OptionWall>().GetHandleLocalPosition(left, right, up, down);

        const float time = 0.1f;
        float timeInit = Time.time;
        while (Time.time < timeInit + time)
        {
            transform.localPosition = Vector3.Lerp(v, finalPosition, (Time.time - timeInit) / time);
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = finalPosition;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 direction;

        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
        direction.z = 0;

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;


        if (!left && !right) //only up and down
        {
            transform.localPosition = new Vector2(0, transform.localPosition.y);
        }
        if (!up && !down) //only up and down
        {
            transform.localPosition = new Vector2(transform.localPosition.x, 0);
        }


        Vector3 delta = transform.localPosition - positionMouse0;
        if (left)
            delta.x = -delta.x;
        if (down)
            delta.y = -delta.y;

        handle.GetComponent<OptionWall>().ChangeSize(new Vector2(delta.x, delta.y), right, up);
        positionMouse0 = transform.localPosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
