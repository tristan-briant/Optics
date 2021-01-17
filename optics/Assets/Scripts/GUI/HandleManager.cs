using UnityEngine;

public class HandleManager : MonoBehaviour
{
    public ChessPiece CP;


    public float angleSet;
    public Vector3 PositionSet;
    float angleAct, angleMouse0, angleMouse1;

    void Start()
    {
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

    public void SetTargetDeltaAngle(float deltaAngle)
    {
        CP.GetComponent<ChessPiece>().angleSet += deltaAngle;
    }

    public void SetTargetDeltaPosition(Vector3 deltaPosition)
    {
        CP.GetComponent<ChessPiece>().positionSet += deltaPosition;
    }

    public void ConstrainTarget(bool translation, bool rotation)
    {
        CP.GetComponent<ChessPiece>().Constrain(translation, rotation);
         Debug.Log(translation + "  " + rotation);
    }

    void Update()
    {
        transform.position = CP.transform.position;
        transform.rotation = Quaternion.identity;
    }



}
