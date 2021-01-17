using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const float ShortClickTime = 0.2f;
    const float LongClickTime = 0.8f;

    float BeginClickTime;

    public void OnPointerDown(PointerEventData eventData)
    {
        BeginClickTime = Time.time;
        StartCoroutine("LongClickTimer");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine("LongClickTimer");
        if (Time.time - BeginClickTime < ShortClickTime)
            SendMessage("OnShortClick", SendMessageOptions.DontRequireReceiver);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StopCoroutine("LongClickTimer");
        SendMessage("OnMouseBeginDrag", SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator LongClickTimer()
    {
        yield return new WaitForSeconds(LongClickTime); // Time before deciding if it is a long click
        SendMessage("OnLongClick", SendMessageOptions.DontRequireReceiver);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SendMessage("OnMouseDrag", SendMessageOptions.DontRequireReceiver);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SendMessage("OnMouseEndDrag", SendMessageOptions.DontRequireReceiver);
    }
}
