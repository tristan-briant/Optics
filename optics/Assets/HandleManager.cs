using UnityEngine;

public class HandleManager : MonoBehaviour
{
    public GameObject Target;


    public float angleSet;
    public Vector3 PositionSet;
    float angleAct, angleMouse0, angleMouse1;

    public void SetTargetDeltaAngle(float deltaAngle)
    {
        Target.GetComponent<DragAndDrop>().angleSet += deltaAngle;
    }

    public void SetTargetDeltaPosition(Vector3 deltaPosition)
    {
        Target.GetComponent<DragAndDrop>().PositionSet += deltaPosition;
    }

    public void ConstrainTarget(bool translation, bool rotation)
    {
        Target.GetComponent<DragAndDrop>().Constrain(translation, rotation);
    }

    void Update()
    {
        transform.position = Target.transform.position;
        transform.rotation = Quaternion.identity;
    }



}
