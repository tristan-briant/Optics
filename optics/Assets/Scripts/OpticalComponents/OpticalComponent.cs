using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpticalComponent : MonoBehaviour {

    public float x=0,y=0; // position
    public float angle = 0;
    public float radius=0.5f;
    public float cos, sin, param; // vecteur directeur
    public bool hasChanged = true;

    private void Start()
    {
        ComputeDir();
        hasChanged = true;
    }

    Vector3 OldPosition;
    Quaternion OldRotation;
    void Update()
    {
        if (OldPosition == transform.localPosition && transform.localRotation == OldRotation)
            return;
      
        ComputeDir();
        OldPosition = transform.localPosition;
        OldRotation = transform.localRotation;
        hasChanged = true;
 
    }

    public void ComputeDir()
    {
        x = transform.localPosition.x;
        y = transform.localPosition.y;

        angle = (transform.localRotation.eulerAngles.z + 90) * Mathf.PI / 180f;
        cos = Mathf.Cos(angle);
        sin = Mathf.Sin(angle);
        param = -sin * x + cos * y;
    }


    float xc, yc;
    public float Collision(LightRay lr, int i)
    {
        float cosr, sinr, xr, yr, br;
        if (i == 1)
        {
            cosr = lr.cos1;
            sinr = lr.sin1;
            xr = lr.StartPosition1.x;
            yr = lr.StartPosition1.y;
            br = lr.param1;
        }
        else
        {
            cosr = lr.cos2;
            sinr = lr.sin2;
            xr = lr.StartPosition2.x;
            yr = lr.StartPosition2.y;
            br = lr.param2;
        }

        float b = param;

        float det = -cosr * sin + sinr * cos;

        if (det == 0) return -1;


        xc = (cosr * b - cos * br) / det;
        yc = (sinr * b - sin * br) / det;

 
        if ((cosr > 0 && xc > xr) || (cosr < 0 && xc < xr) || (sinr > 0 && yc > yr) || (sinr < 0 && yc < yr))
            if ((xc - x) * (xc - x) + (yc - y) * (yc - y) < radius * radius)
                return (xc - xr) * (xc - xr) + (yc - yr) * (yc - yr);
        
        return -1;
    }
    
    protected float xc1, yc1, xc2, yc2;
    public float Collision2(LightRay lr)
    {
        float l1 = Collision(lr, 1);
        xc1 = xc; yc1 = yc;
        if (l1 < 0) return -1;
        float l2 = Collision(lr, 2);
        xc2 = xc; yc2 = yc;
        if (l2 < 0) return -1;

        return l1;
    }
    
    public virtual void Deflect(LightRay r)
    {
        
    }

}
