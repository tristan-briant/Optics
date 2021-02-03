using UnityEngine;
using UnityEngine.EventSystems;


public class PanZoom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector3 touchStart;
    float CamSizeMax;
    float CamSizeMin = 2f;
    RectTransform rt;

    private float screenWidth;
    private float screenHeight;

    const float SizeOffset = 3.75f - 1f; // Camera field bigger of SizeOffset than playground (size of options)


    void Start()
    {
        screenWidth = (float)Screen.width / 2.0f;
        screenHeight = (float)Screen.height / 2.0f;

        SetupCameraAndPlayground();
    }

    void SetupCameraAndPlayground()
    {
        GameObject PG = GameObject.Find("Playground");
        rt = (RectTransform)PG.transform;
        CamSizeMax = (rt.rect.height + SizeOffset) / 2;
        Camera.main.orthographicSize = rt.rect.height / 2;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        GetComponent<BoxCollider2D>().size = new Vector2(rt.rect.width, rt.rect.height);
    }

    public void SetPlaygroundSize(float width, float height)
    {
        GameObject PG = GameObject.Find("Playground");
        PG.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        SetupCameraAndPlayground();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ChessPiece.UnSelectAll();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + increment, CamSizeMin, CamSizeMax);
        ClampCamera();
    }
    
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
            Vector3 deltaPos;
            deltaPos.x = 0.5f * (T0.deltaPosition.x + T1.deltaPosition.x);
            deltaPos.y = 0.5f * (T0.deltaPosition.y + T1.deltaPosition.y);
            deltaPos.z = 0;
            Camera.main.transform.position -= deltaPos / screenWidth * Camera.main.orthographicSize;

            zoom(deltaMagnitude / screenWidth * Camera.main.orthographicSize);
        }
        
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

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 1 || Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction.z = 0;
            Camera.main.transform.position += direction;
            ClampCamera();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
