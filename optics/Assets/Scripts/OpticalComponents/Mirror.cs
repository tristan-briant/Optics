﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : OpticalComponent
{


    override public void Deflect(LightRay r)
    {

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        r.Length1 = Mathf.Sqrt((xc1 - xo1) * (xc1 - xo1) + (yc1 - yo1) * (yc1 - yo1));
        r.Length2 = Mathf.Sqrt((xc2 - xo2) * (xc2 - xo2) + (yc2 - yo2) * (yc2 - yo2));

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

        if (lr == null) { return; }

        lr.Col = r.Col;
        lr.Intensity = r.Intensity;
        lr.StartPosition1 = new Vector3(xc1, yc1, 0);
        lr.StartPosition2 = new Vector3(xc2, yc2, 0);
        lr.Length1 = lr.Length2 = 15.0f;
        lr.Origin = this;

        // Pour un miroir

        lr.Direction1 = -ao1 + 2 * angle;
        lr.Direction2 = -ao2 + 2 * angle;
        lr.ComputeDir();
    }

}
