using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Animator animator;
    Vector3 startPos;

    public bool translation;
    public bool rotation;

    public Transform handle;
    LineRenderer lr;
    float startAngle;
    Vector3 HandleInitialPosition;
    Vector3 newLocalPosition;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        startPos = transform.localPosition;
        startAngle = AngleFromXY(startPos.x, startPos.y);
        lr = GetComponent<LineRenderer>();
    }

    float angleMouse0;
    Vector3 positionMouse0;


    bool dragged = false;
    Vector3 offset;
    public void OnPointerDown(PointerEventData ev)
    {
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //if not clicked exactly in the center

        angleMouse0 = AngleFromXY(transform.position.x - handle.position.x,
                        transform.position.y - handle.position.y, -startAngle);

        HandleInitialPosition = handle.position;
        positionMouse0 = 0.3f * transform.position + 0.7f * HandleInitialPosition;


        animator.SetBool("controled", true);
        lr.enabled = true;
        dragged = true;

        handle.GetComponent<HandleManager>().ConstrainTarget(translation, rotation);
    }

    public void OnPointerUp(PointerEventData ev)
    {
        animator.SetBool("controled", false);
        StartCoroutine("BackInPlace");
        dragged = false;
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
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - handle.position - offset;
            direction.z = 0;

            transform.localPosition = startPos / startPos.magnitude * direction.magnitude;

            float angleMouse1 = AngleFromXY(direction.x, direction.y, -startAngle);

            if (rotation)
            {
                handle.GetComponent<HandleManager>().SetTargetDeltaAngle(angleMouse1 - angleMouse0);
                angleMouse0 = angleMouse1;
            }

            if (translation)
            {
                Vector3 positionMouse1 = 0.3f * transform.position + 0.7f * HandleInitialPosition;

                handle.GetComponent<HandleManager>().SetTargetDeltaPosition(positionMouse1 - positionMouse0);
                positionMouse0 = positionMouse1;
            }

            animator.SetFloat("rotation", angleMouse1 / 360.0f);
        }

        lr.SetPosition(0, handle.position);
        lr.SetPosition(1, transform.position);
    }

}
