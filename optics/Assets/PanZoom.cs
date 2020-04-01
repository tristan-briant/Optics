using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    float zoomInit;
    // Start is called before the first frame update
    void Start()
    {
        zoomInit = Camera.main.orthographicSize;
    }

    void OnMouseDown()
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void OnMouseDrag()
    {
        Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Camera.main.transform.position += direction;
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + increment, 0.5f * zoomInit, zoomInit);
    }

    bool two2oneTouch;
    void Update()
    {

        zoom(-Input.GetAxis("Mouse ScrollWheel"));

        if (Input.touchCount == 2)
        {
            Touch T0 = Input.GetTouch(0);
            Touch T1 = Input.GetTouch(1);

            Vector2 posOld0 = T0.position - T0.deltaPosition;
            Vector2 posOld1 = T1.position - T1.deltaPosition;

            float deltaMagnitude = (posOld1 - posOld0).magnitude - (T1.position - T0.position).magnitude;
            zoom(deltaMagnitude * 0.002f);

            two2oneTouch = true;
        }
        else if (two2oneTouch == true)
        {
            // evite un décalage brusque quand on retire un doigt mais ne marche pas
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            two2oneTouch = false;
        }

    }
}
