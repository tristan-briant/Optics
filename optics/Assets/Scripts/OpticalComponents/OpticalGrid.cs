using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpticalGrid : OpticalComponent
{
    const float angleTolerance = 0.5f * Mathf.Deg2Rad;

    override public void Deflect(LightRay r)
    {
        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        //float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        //float ao2 = r.Direction2;

        r.Length1 = (xc1 - xo1) * r.cos1 + (yc1 - yo1) * r.sin1;
        r.Length2 = (xc2 - xo2) * r.cos2 + (yc2 - yo2) * r.sin2;

        float deltaAngle1 = (angle - r.Direction1 + 0.5f * Mathf.PI) % Mathf.PI;
        if (deltaAngle1 > 0.5f * Mathf.PI) deltaAngle1 -= Mathf.PI;

        float deltaAngle2 = (angle - r.Direction2 + 0.5f * Mathf.PI) % Mathf.PI;
        if (deltaAngle2 > 0.5f * Mathf.PI) deltaAngle2 -= Mathf.PI;


        if (Mathf.Abs(deltaAngle1) > angleTolerance
         || Mathf.Abs(deltaAngle2) > angleTolerance)
        {
            r.ClearChildren();  // nothing goes throw
            return;
        }

        LightRay lr = null;
        switch (r.Children.Count)
        {
            case 0:
                lr = LightRay.NewLightRayChild(r);
                break;
            case 1:
                lr = r.Children[0];
                break;
            default:
                r.ClearChildren(1);
                lr = r.Children[0];
                break;
        }

        if (lr == null) return;

        lr.Col = r.Col;
        lr.Intensity = r.Intensity;
        lr.StartPosition1 = new Vector3(xc1, yc1, 0);
        lr.StartPosition2 = new Vector3(xc2, yc2, 0);
        lr.Direction1 = r.Direction1;
        lr.Direction2 = r.Direction2;
        lr.Length1 = lr.Length2 = 15.0f;
        lr.Origin = this;

        lr.ComputeDir();
    }

    override public void ChangeVisual() { }
}
