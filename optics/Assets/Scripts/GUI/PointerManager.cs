using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const float ShortClickTime = 0.2f;
    const float LongClickTime = 0.5f;
    bool PhaseDragging;
    bool PhaseTouching;

    float BeginClickTime;

    public void Start()
    {
        //Input.simulateMouseWithTouches=false;
    }

    public void Update()
    {
        if (Input.touchCount > 1)  // Only interested in 1 touch moves otherwise cancel everything
        {
            StopAllCoroutines();
            if (PhaseDragging)
            {
                OnEndDrag(null);
            }


            PhaseTouching = false;

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.touchCount <= 1)
        {
            BeginClickTime = Time.time;
            StartCoroutine("LongClickTimer");
            PhaseTouching = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine("LongClickTimer");
        if (PhaseTouching)
            if (Time.time - BeginClickTime < ShortClickTime)
                SendMessage("OnShortClick", SendMessageOptions.DontRequireReceiver);

        PhaseTouching = false;
    }

    IEnumerator LongClickTimer()
    {
        yield return new WaitForSeconds(LongClickTime); // Time before deciding if it is a long click
        SendMessage("OnLongClick", SendMessageOptions.DontRequireReceiver);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StopCoroutine("LongClickTimer");
        if (Input.touchCount <= 1)
        {
            SendMessage("OnMouseBeginDrag", SendMessageOptions.DontRequireReceiver);
            PhaseDragging = true;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (PhaseDragging)
            SendMessage("OnMouseDragging", SendMessageOptions.DontRequireReceiver);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PhaseDragging)
            SendMessage("OnMouseEndDrag", SendMessageOptions.DontRequireReceiver);
        PhaseDragging = false;
    }
}
