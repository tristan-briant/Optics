﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LameSemi : OpticalComponent
{
    private const float Imin = 0.001f;
    public float reflectionCoef = 0.5f;

    public float ReflectionCoef { get => reflectionCoef; set { reflectionCoef = value; hasChanged = true; } }
    public float ReflectionCoefMax { get => 0.9f; }
    public float ReflectionCoefMin { get => 0.1f; }


    override public void Deflect(LightRay r)
    {

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        //r.Length1 = Mathf.Sqrt((xc1 - xo1) * (xc1 - xo1) + (yc1 - yo1) * (yc1 - yo1));
        //r.Length2 = Mathf.Sqrt((xc2 - xo2) * (xc2 - xo2) + (yc2 - yo2) * (yc2 - yo2));
        r.Length1 = (xc1 - xo1) * r.cos1 + (yc1 - yo1) * r.sin1;
        r.Length2 = (xc2 - xo2) * r.cos2 + (yc2 - yo2) * r.sin2;

        LightRay lr = null;
        LightRay lt = null;
        if (r.transform.childCount == 0)
        {
            lr = LightRay.NewLightRayChild(r);
            lt = LightRay.NewLightRayChild(r);
        }
        else if (r.transform.childCount == 1)
        {
            lt = r.transform.GetChild(0).GetComponent<LightRay>();
            lr = LightRay.NewLightRayChild(r);
        }
        else if (r.transform.childCount == 2)
        {
            lt = r.transform.GetChild(0).GetComponent<LightRay>();
            lr = r.transform.GetChild(1).GetComponent<LightRay>();
        }
        else
        {
            while (r.transform.childCount > 2)
                //FreeLightRay(r.transform.GetChild(0).GetComponent<LightRay>());
                r.transform.GetChild(0).GetComponent<LightRay>().FreeLightRay();

            lt = r.transform.GetChild(0).GetComponent<LightRay>();
            lr = r.transform.GetChild(1).GetComponent<LightRay>();
        }

        // Rayon transmis
        if (lt)
        {
            lt.Col = r.Col;
            lt.Intensity = r.Intensity * (1 - ReflectionCoef);
            lt.StartPosition1 = new Vector3(xc1, yc1, 0);
            lt.StartPosition2 = new Vector3(xc2, yc2, 0);
            lt.Direction1 = r.Direction1;
            lt.Direction2 = r.Direction2;
            lt.Origin = this;
            lt.ComputeDir();
        }

        //Rayon reflechi
        if (lr)
        {
            lr.Col = r.Col;
            lr.Intensity = r.Intensity * ReflectionCoef;
            lr.StartPosition1 = new Vector3(xc1, yc1, 0);
            lr.StartPosition2 = new Vector3(xc2, yc2, 0);
            lr.Direction1 = -ao1 + 2 * angle;
            lr.Direction2 = -ao2 + 2 * angle;
            lr.Origin = this;
            lr.ComputeDir();
        }
    }
}
