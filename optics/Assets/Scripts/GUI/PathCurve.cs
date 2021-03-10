using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCurve : MonoBehaviour
{
    public AnimationCurve curveX;
    public AnimationCurve curveY;

    public void Start()
    {
        ComputeLine();
    }

    public void ComputeLine()
    {
        curveX = new AnimationCurve();
        curveY = new AnimationCurve();

        int Npoint = transform.childCount;

        if (Npoint >= 2)
        {
            for (int i = 0; i < Npoint; i++)
            {
                Vector3 pos = transform.GetChild(i).transform.position;
                float t = (float)i / (Npoint - 1);

                curveX.AddKey(t, pos.x);
                curveY.AddKey(t, pos.y);
            }
        }
    }


    public void OnDrawGizmos()
    {
        ComputeLine();

        int N = 100;
        for (float i = 0; i < N; i++)
        {
            Vector2 pos1 = new Vector2(curveX.Evaluate(i / N), curveY.Evaluate(i / N));
            Vector2 pos2 = new Vector2(curveX.Evaluate((i + 1) / N), curveY.Evaluate((i + 1) / N));
            Debug.DrawLine(pos1, pos2);
        }
    }
}