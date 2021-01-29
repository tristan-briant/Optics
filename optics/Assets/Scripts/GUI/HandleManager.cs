using UnityEngine;

public class HandleManager : MonoBehaviour
{
    public ChessPiece CP;


    public float angleSet;
    public Vector3 PositionSet;
    float angleAct, angleMouse0, angleMouse1;
    const float incrementRotation = 11.25f;


    void Start()
    {

        transform.SetParent(CP.transform);

        Transform fineHandle = transform.Find("ItemHolder");
        if (fineHandle)
            foreach (Canvas c in fineHandle.GetComponentsInChildren<Canvas>())
                c.sortingLayerName = "Handle";

        transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;

        if (!CP.CanTranslate)
            foreach (DragHandle dh in transform.GetComponentsInChildren<DragHandle>())
                if (dh.translation)
                    dh.gameObject.SetActive(false);

        if (!CP.CanRotate)
            foreach (DragHandle dh in transform.GetComponentsInChildren<DragHandle>())
                if (dh.rotation)
                    dh.gameObject.SetActive(false);

    }

    float DeltaAngleSet;
    public void SetTargetDeltaAngle(float deltaAngle, bool clamped = false)
    {

        if (!clamped)
            CP.GetComponent<ChessPiece>().angleSet += deltaAngle;

        if (clamped)
        {
            float a, b;

            //CP.GetComponent<ChessPiece>().angleSet = Mathf.Round(CP.GetComponent<ChessPiece>().angleSet / incrementRotation) * incrementRotation;
            a = CP.GetComponent<ChessPiece>().angleSet;
            DeltaAngleSet += deltaAngle;

            a += DeltaAngleSet;
            b = Mathf.Round(a / incrementRotation) * incrementRotation;
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

        Debug.Log(translation + "  " + rotation);
    }

    void Update()
    {
        //transform.position = CP.transform.position;
        //transform.rotation = Quaternion.identity;
    }



}
