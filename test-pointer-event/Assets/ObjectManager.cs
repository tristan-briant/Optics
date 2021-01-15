using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ObjectManager : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        transform.position = pos;
    }

    void LongClick()
    {
        Debug.Log("Receive a long click");
    }

    void ShortClick()
    {
        Debug.Log("Receive a short click");

    }
}
