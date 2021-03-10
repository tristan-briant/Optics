using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditCurve : MonoBehaviour
{
    public PathCurve path;

    void Update()
    {
        path = GetComponent<PathCurve>();
        ComputeLine();
        Debug.Log("toto");
    }


    public void ComputeLine()
    {
        path.curveX = new AnimationCurve();
        path.curveY = new AnimationCurve();

        int Npoint = transform.childCount;

        if (Npoint >= 2)
        {
            for (int i = 0; i < Npoint; i++)
            {
                Vector3 pos = transform.GetChild(i).transform.position;
                float t = (float)i / (Npoint - 1);

                path.curveX.AddKey(t, pos.x);
                path.curveY.AddKey(t, pos.y);
            }
        }
    }
}
