using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateComponent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    const float longClickTime = 0.3f;
    bool longClicking = false;
    float clickStartTime;
    Vector3 clickStartPos;
    public GameObject prefab;

    GameObject item;

    void OnMouseDown()
    {
        longClicking = false;
        item = null;
        StartCoroutine("OnLongClick");
    }

    void OnMouseUp()
    {
        StopCoroutine("OnLongClick");
    }

    IEnumerator OnLongClick()
    {
        yield return new WaitForSeconds(longClickTime); // Time before deciding if it is a long click
        longClicking = true;
        Debug.Log("Long click");

        SpawnItem();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!longClicking)
            transform.GetComponentInParent<ScrollRect>().OnDrag(eventData);
        else if (item)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            item.transform.position = pos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!longClicking)
        {
            StopCoroutine("OnLongClick");
            transform.GetComponentInParent<ScrollRect>().OnBeginDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!longClicking)
            transform.GetComponentInParent<ScrollRect>().OnEndDrag(eventData);
        else if (item)
        {
            Activate(true);
        }
    }

    void SpawnItem()
    {
        item = Instantiate(prefab, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        /*foreach (Collider2D c in item.GetComponentsInChildren<Collider2D>())
            c.enabled = false;*/
        item.GetComponent<DragAndDrop>().enabled = false;
        item.transform.parent = GameObject.Find("DragedLayer").transform;

        Activate(false);
    }

    void Activate(bool active)
    {
        item.GetComponent<OpticalComponent>().enabled=active;

        item.GetComponent<DragAndDrop>().enabled = active;

        foreach (Canvas cv in item.GetComponentsInChildren<Canvas>())
            cv.overrideSorting = active;

        foreach (Collider2D co in item.GetComponentsInChildren<Collider2D>())
            co.enabled = active;
    }

}
