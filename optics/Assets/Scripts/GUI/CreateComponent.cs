using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateComponent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{

    const float longClickTime = 0.3f;
    bool longClicking = false;
    float clickStartTime;
    Vector3 clickStartPos;
    public GameObject prefab;

    GameObject item;

    public void OnPointerDown(PointerEventData eventData)
    {
        longClicking = false;
        item = null;
        StartCoroutine("OnLongClick");
        Debug.Log("click");
    }

    public void OnPointerUp(PointerEventData eventData)
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
        Debug.Log("toto");
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
            item.transform.SetParent(GameObject.Find("Playground").transform);
            item.transform.localScale = Vector3.one;
            item.GetComponent<ChessPiece>().LetFindPlace();

            FindObjectOfType<GameEngine>().UpdateComponentList();
        }
    }

    void SpawnItem()
    {
        item = Instantiate(prefab, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        item.GetComponent<ChessPiece>().enabled = false;
        item.transform.SetParent(GameObject.Find("DragedLayer").transform);
        item.transform.localScale = Vector3.one;
        Activate(false);

    }

    void Activate(bool active)
    {
        item.GetComponent<OpticalComponent>().enabled=active;
        item.GetComponent<ChessPiece>().enabled = active;

        foreach (Canvas cv in item.GetComponentsInChildren<Canvas>())
            cv.overrideSorting = active;

        foreach (Collider2D co in item.GetComponentsInChildren<Collider2D>())
            co.enabled = active;
    }

}
