using UnityEngine;
using UnityEngine.EventSystems;


public class PanZoom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    Vector3 touchStart;
    float zoomMax;
    RectTransform rt;

    private float width;
    private float height;

    const float SizeOffset = 3.75f - 1f; // Camera field bigger of SizeOffset than playground (size of options)

    void Awake()
    {
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
        Input.simulateMouseWithTouches = false;
    }


    void Start()
    {
        GameObject PG = GameObject.Find("Playground");
        rt = (RectTransform)PG.transform;
        zoomMax = (rt.rect.height + SizeOffset) / 2;
        Camera.main.orthographicSize = rt.rect.height / 2;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        //Debug.Log("Camera Size : " + zoomInit + "Pg size : " + x + "   -  " + y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ChessPiece.UnSelectAll();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Up" + Input.touchCount);
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }



    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + increment, 0.5f * zoomMax, zoomMax);
        ClampCamera();
    }

    PointerManager closest;
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
            //zoom(deltaMagnitude * 0.002f);
            Vector3 deltaPos;
            deltaPos.x = 0.5f * (T0.deltaPosition.x + T1.deltaPosition.x);
            deltaPos.y = 0.5f * (T0.deltaPosition.y + T1.deltaPosition.y);
            deltaPos.z = 0;
            Camera.main.transform.position -= deltaPos/ width * Camera.main.orthographicSize; 

            zoom(deltaMagnitude / width * Camera.main.orthographicSize);
        }
        /*
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:

                            closest = FindClosestComponent(Camera.main.ScreenToWorldPoint(touch.position));
                            if (closest)
                                closest.OnPointerDown(null);
                            Debug.Log(closest);

                            break;

                        //Determine if the touch is a moving touch
                        case TouchPhase.Moved:
                            //if (closest)
                                closest.OnDrag(null);//
                            //touch.position - startPos;

                            break;

                        case TouchPhase.Ended:
                            if (closest)
                                closest.OnPointerUp(null);
                            break;
                    }


                }*/


        /*
        else if (Input.touchCount == 1 && two2oneTouch == true)
        {
            // evite un décalage brusque quand on retire un doigt mais ne marche pas
            //touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Touch T0 = Input.GetTouch(0);
            //touchStart = T0.position;
            two2oneTouch = false;
        }*/
    }

    void ClampCamera()
    {// Clamp the camera in the rectangle of the PG
        Vector3 CamPos = Camera.main.transform.position;
        float camsize = Camera.main.orthographicSize;
        float ratio = Camera.main.aspect;

        if (2 * camsize * ratio > rt.rect.width + SizeOffset)
            CamPos.x = 0;
        else
            CamPos.x = Mathf.Clamp(CamPos.x, -(rt.rect.width + SizeOffset) / 2 + camsize * ratio, (rt.rect.width + SizeOffset) / 2 - camsize * ratio);

        if (2 * camsize > rt.rect.height + SizeOffset)
            CamPos.y = 0;
        else
            CamPos.y = Mathf.Clamp(CamPos.y, -(rt.rect.height + SizeOffset) / 2 + camsize, (rt.rect.height + SizeOffset) / 2 - camsize);

        CamPos.z = -10f;

        Camera.main.transform.position = CamPos;
    }

    PointerManager FindClosestComponent(Vector2 position)
    {

        float distance = 1;  // If nothing under 1 return null
        PointerManager closest = null;
        foreach (PointerManager gc in FindObjectsOfType<PointerManager>())
        {

            Vector3 gcPos = gc.transform.position;
            Vector2 gcPos2D = new Vector2(gcPos.x, gcPos.y);
            float d = (gcPos2D - position).magnitude;
            if (distance < 0 || d < distance)
            {
                distance = d;
                closest = gc;
            }
        }
        return closest;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 1|| Input.GetMouseButtonDown(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction.z = 0;
            Camera.main.transform.position += direction;
            ClampCamera();
        }
    }
}
