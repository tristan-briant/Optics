using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateComponent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,
IPointerDownHandler, IPointerUpHandler
{
    public GameObject prefab;

    const float longClickTime = .50f;
    bool longClicking = false;
    float clickStartTime;
    Vector3 clickStartPos;

    GameObject item;

    public void OnPointerDown(PointerEventData eventData)
    {
        longClicking = false;
        item = null;

        StartCoroutine(OnLongClick());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine(OnLongClick());
    }

    IEnumerator OnLongClick()
    {
        yield return new WaitForSeconds(longClickTime); // Time before deciding if it is a long click
        SpawnItem();
        longClicking = true;
        //StartCoroutine(WelcomOnBoard());
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
            //StopCoroutine("OnLongClick");
            StopAllCoroutines();
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

        
        item.transform.SetParent(GameObject.Find("DragLayer").transform);
        Activate(false); // after setparen otherwise canvas.overrideSorting won't work
    }

    void Activate(bool active)
    {
        item.GetComponent<OpticalComponent>().enabled = active;
        item.GetComponent<ChessPiece>().enabled = active;

        item.gameObject.SetActive(true);
        foreach (Canvas cv in item.GetComponentsInChildren<Canvas>()){
            cv.overrideSorting = active;
            Debug.Log("canvas found");
        }

        foreach (Collider2D co in item.GetComponentsInChildren<Collider2D>())
            co.enabled = active;
    }

    IEnumerator WelcomOnBoard()
    {
        const float AnimTime = 0.5f;
        float time = 0;
        //item.transform.SetParent(GameObject.Find("DragLayer").transform);
        //item.transform.localScale = Vector3.one;
        Activate(false);
        Vector3 FinalPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        while (time < AnimTime)
        {
            time += Time.deltaTime;
            item.transform.localScale = Mathf.Lerp(2f, 1f, time / AnimTime) * Vector3.one;
            item.transform.position = FinalPosition + new Vector3(0, Mathf.Lerp(1f, 0f, time / AnimTime), 0);
            yield return 0;
        }

       
        item.transform.SetParent(GameObject.Find("Playground/Components").transform, false); 
        Activate(true);
        item.transform.localScale = Vector3.one;

        item.GetComponent<ChessPiece>().positionSet = FinalPosition;
        item.GetComponent<ChessPiece>().LetFindPlace();

        GameEngine.instance.UpdateComponentList();
    }

}
