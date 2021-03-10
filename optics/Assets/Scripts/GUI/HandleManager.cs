using UnityEngine;
using UnityEngine.UI;

public class HandleManager : MonoBehaviour
{
    public Color enableColor;
    public Color disableColor;

    public ChessPiece CP;
    public float angleSet;
    public Vector3 PositionSet;
    float angleAct, angleMouse0, angleMouse1;
    const float incrementRotation = 11.25f;
    Transform itemHolder;

    float startAngle;

    void Start()
    {
        transform.SetParent(CP.transform);

        itemHolder = transform.Find("ItemHolder");
        if (itemHolder)
            foreach (Canvas c in itemHolder.GetComponentsInChildren<Canvas>())
                c.sortingLayerName = "Handle";

        transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;


        foreach (DragHandle dh in transform.GetComponentsInChildren<DragHandle>())
        {
            if (dh.translation)
            {
                if (CP.CanTranslate)
                {
                    dh.gameObject.SetActive(true);
                    dh.GetComponent<Image>().color = enableColor;
                }
                else if (GameEngine.instance.isInEditMode)
                {
                    dh.gameObject.SetActive(true);
                    dh.GetComponent<Image>().color = disableColor;
                }
                else
                    dh.gameObject.SetActive(false);
            }

            if (dh.rotation)
            {
                if (CP.CanRotate)
                {
                    dh.gameObject.SetActive(true);
                    dh.GetComponent<Image>().color = enableColor;
                }
                else if (GameEngine.instance.isInEditMode)
                {
                    dh.gameObject.SetActive(true);
                    dh.GetComponent<Image>().color = disableColor;
                }
                else
                    dh.gameObject.SetActive(false);
            }
        }

        startAngle = CP.GetComponent<ChessPiece>().angleSet;

    }

    float DeltaAngleSet;
    public void SetTargetDeltaAngle(float deltaAngle, bool clamped = false)
    {

        if (!clamped)
            CP.GetComponent<ChessPiece>().angleSet += deltaAngle;

        if (clamped)
        {
            float a, b;
            a = CP.GetComponent<ChessPiece>().angleSet;
            DeltaAngleSet += deltaAngle;

            a += DeltaAngleSet;
            b = MyMathf.Round(a, incrementRotation);
            DeltaAngleSet = a - b;

            CP.GetComponent<ChessPiece>().angleSet = b;

        }
    }

    public void SetTargetDeltaPosition(Vector3 deltaPosition)
    {
        CP.GetComponent<ChessPiece>().PositionSet += deltaPosition;
    }

    public void ConstrainTarget(bool translation, bool rotation)
    {
        CP.GetComponent<ChessPiece>().Constrain(translation, rotation);
        if (translation || rotation)
            ChessPiece.Manipulated = CP;
        else ChessPiece.Manipulated = null;
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
        itemHolder.transform.rotation = Quaternion.Euler(0, 0, CP.GetComponent<ChessPiece>().angleSet - startAngle);
    }

}
