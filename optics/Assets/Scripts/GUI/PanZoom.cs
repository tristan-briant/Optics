using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class PanZoom : MonoBehaviour, IDragHandler, IPointerClickHandler
//IPointerDownHandler, IPointerUpHandler,  //IBeginDragHandler, 
//IEndDragHandler
{
    Vector3 touchStart;
    float CamSizeMax;
    float CamSizeMin = 2f;
    RectTransform rt;

    public Vector2 Size = new Vector2(4, 4);
    private float screenWidth;
    private float screenHeight;

    const float SizeOffset = 3.75f - 1f; // Camera field bigger of SizeOffset than playground (size of options)
    const float PerspectiveEffect = 0.5f;  // ratio for panning / moving the grass and give perspecive
    public GameObject Grass;

    public static PanZoom instance;

    #region SINGLETON
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }
    #endregion

    void Start()
    {
        screenWidth = (float)Screen.width / 2.0f;
        screenHeight = (float)Screen.height / 2.0f;

        GameObject PG = GameObject.Find("Playground");
        Size = ((RectTransform)PG.transform).rect.size;
        SetupCameraAndPlayground();
    }

    void SetupCameraAndPlayground()
    {
        GameObject PG = GameObject.Find("Playground");
        rt = (RectTransform)PG.transform;
        rt.sizeDelta = Size;
        GetComponent<BoxCollider2D>().size = Size;

        foreach (BoxCollider2D wall in transform.Find("Base").GetComponentsInChildren<BoxCollider2D>())
            wall.size = ((RectTransform)wall.transform).rect.size;

        CamSizeMax = (Size.y + SizeOffset) / 2;

        Camera cam = Camera.main;
        if (cam)
        {
            cam.orthographicSize = Size.y / 2;
            cam.transform.position = new Vector3(0, 0, -10);
        }
    }

    public void SetPlaygroundSize(float width, float height)
    {
        //GameObject PG = GameObject.Find("Playground");
        //PG.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        Size.x = width;
        Size.y = height;

        SetupCameraAndPlayground();
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + increment, CamSizeMin, CamSizeMax);
        ClampCamera();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            zoom(-Input.GetAxis("Mouse ScrollWheel"));
    }

    void ClampCamera()
    { // Clamp the camera in the rectangle of the PG

        Vector3 CamPos = Camera.main.transform.position;
        float camsize = Camera.main.orthographicSize;
        float ratio = Camera.main.aspect * 0.5f;

        CamPos.x = Mathf.Clamp(CamPos.x, -rt.rect.width / 2, +rt.rect.width / 2);
        CamPos.y = Mathf.Clamp(CamPos.y, -rt.rect.height / 2, +rt.rect.height / 2);

        CamPos.z = -10f;

        Camera.main.transform.position = CamPos;


        Vector3 GrassPos = CamPos * PerspectiveEffect; // To give a perspective effect
        GrassPos.z = 2;
        Grass.transform.position = GrassPos;

        Grass.transform.localScale = Vector3.one * Mathf.Sqrt(camsize);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 2)
        {
            Touch T0 = Input.GetTouch(0);
            Touch T1 = Input.GetTouch(1);

            Vector2 posOld0 = T0.position - T0.deltaPosition;
            Vector2 posOld1 = T1.position - T1.deltaPosition;

            float deltaMagnitude = (posOld1 - posOld0).magnitude - (T1.position - T0.position).magnitude;
            Vector3 deltaPos = T0.deltaPosition * 0.5f;
            //deltaPos.x = 0.5f * (T0.deltaPosition.x + T1.deltaPosition.x);
            //deltaPos.y = 0.5f * (T0.deltaPosition.y + T1.deltaPosition.y);
            deltaPos.z = 0;
            //Vector3 direction = 0.5f*-eventData.delta * Camera.main.orthographicSize / screenHeight;
            //direction.z = 0;
            //Camera.main.transform.position += direction;
            Camera.main.transform.position -= deltaPos / screenHeight * Camera.main.orthographicSize;

            zoom(deltaMagnitude / screenWidth * Camera.main.orthographicSize);
        }
        else if (Input.touchCount == 1 || Input.GetMouseButton(0))
        {
            //Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = -eventData.delta * Camera.main.orthographicSize / screenHeight;
            //Vector3 direction = -T0.deltaPosition * Camera.main.orthographicSize / screenHeight;
            direction.z = 0;
            Camera.main.transform.position += direction;
            ClampCamera();
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (Input.touchCount == 1)
        {
            Touch T0 = Input.GetTouch(0);
            clickCount = T0.tapCount;
        }

        if (clickCount == 1)
        {
            ChessPiece.UnSelectAll();
        }
        if (clickCount == 2)
        {
            StartCoroutine(ResetCamera());
        }
    }

    IEnumerator ResetCamera()
    {
        float finalOrthSize = rt.rect.height / 2;
        Vector3 finalCamPos = new Vector3(0, 0, -10);

        const float animTime = 0.5f;
        float t = 0;
        while (t < animTime)
        {
            t += Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, finalOrthSize, t / animTime);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, finalCamPos, t / animTime);
            yield return null;
        }

        Camera.main.orthographicSize = finalOrthSize;
        Camera.main.transform.position = finalCamPos;

    }

    public void IncrementSize(int deltaW, int deltaH)
    {


    }

}
