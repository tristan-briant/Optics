using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LameSemi : OpticalComponent {

    public float ReflectionCoef = 0.5f;

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

        LightRay lt = NewRayLightChild(r); // Rayon transmis
        if (lt == null) return;
        lt.isVisible = true;
        lt.gameObject.SetActive(true);

        lt.Col = r.Col;
        lt.Intensity = r.Intensity * (1 - ReflectionCoef);
        lt.StartPosition1 = new Vector3(xc1, yc1, 0);
        lt.StartPosition2 = new Vector3(xc2, yc2, 0);
        lt.Direction1 = r.Direction1;
        lt.Direction2 = r.Direction2;
        lt.Length1 = 15.0f;
        lt.Length2 = 15.0f;
        lt.Origin = this;
        lt.ComputeDir();


        LightRay lr = NewRayLightChild(r); //Rayon reflechi
        if (lr == null) return;

        lr.isVisible = true;
        lr.gameObject.SetActive(true);

        lr.Col = r.Col;
        lr.Intensity = r.Intensity * ReflectionCoef;
        lr.StartPosition1 = new Vector3(xc1, yc1, 0);
        lr.StartPosition2 = new Vector3(xc2, yc2, 0);
        lr.Direction1 = ao1;
        lr.Direction2 = ao2;
        lr.Length1 = 15.0f;
        lr.Length2 = 15.0f;
        lr.Origin = this;

        // Pour un miroir

        lr.Direction1 = -ao1 + 2 * angle;
        lr.Direction2 = -ao2 + 2 * angle;
        lr.ComputeDir();
    }
}
