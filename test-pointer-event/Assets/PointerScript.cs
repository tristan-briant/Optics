using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PointerScript : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IBeginDragHandler
{

    float ClickTime;


    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        ClickTime = Time.time;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        ClickTime = Time.time;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {

        if (Time.time - ClickTime > 0.5f)
            gameObject.SendMessage("LongClick", SendMessageOptions.DontRequireReceiver);

        if (Time.time - ClickTime < 0.2f)
            gameObject.SendMessage("ShortClick", SendMessageOptions.DontRequireReceiver);
    }



    void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }

    // Update is called once per frame
    void Update()
    {

    }


}
