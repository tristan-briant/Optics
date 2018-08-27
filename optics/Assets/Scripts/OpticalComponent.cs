using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpticalComponent : MonoBehaviour {

    public float x=0,y=0; // position
    public float angle = 0;
    public float radius=0.1f;
    public float focal;

    public GameObject cross;
    public GameObject other;
    public bool backSide = false;


    float xc, yc;
    public float Collision(float xr,float yr, float ar)
    {
        float cosr = Mathf.Cos(ar);
        float sinr = Mathf.Sin(ar);

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        float det = -cosr * sin + sinr * cos;

        if (det == 0) return -1;

        float b = -sin * x + cos * y;
        float br = -sinr * xr + cosr * yr;

        xc = (cosr * b - cos * br) / det;
        yc = (sinr * b - sin * br) / det;

        if ((cosr > 0 && xc > xr) || (cosr < 0 && xc < xr) || (sinr > 0 && yc > yr) || (sinr < 0 && yc < yr))
            if ((xc - x) * (xc - x) + (yc - y) * (yc - y) < radius * radius)
                return 1;

        return -1;
    }

    float xc1, yc1, xc2, yc2;
    public float Collision2(float xr1, float yr1, float ar1, float xr2, float yr2, float ar2)
    {
        float l1=Collision(xr1, yr1, ar1);
        xc1 = xc; yc1 = yc;
        if (l1 < 0) return -1;
        float l2=Collision(xr2, yr2, ar2);
        xc2 = xc; yc2 = yc;
        if (l2 < 0) return -1;

        return 1;
    }


    // Update is called once per frame

    Vector3 OldPosition;
    Quaternion OldRotation;
    void LateUpdate () {

        /*if (OldPosition == transform.localPosition && transform.localRotation == OldRotation) return;

        OldPosition = transform.localPosition;
        OldRotation = transform.localRotation;*/

       
        
    }

    public void Deflection() {
        foreach (Transform t in transform.parent.Find("Rays"))
        {
            LightRay r = t.GetComponent<LightRay>();
            RayCollision2(r);
        }
    }

    public void RayCollision2(LightRay r)
    {
        x = transform.localPosition.x;
        y = transform.localPosition.y;
        angle = (transform.localRotation.eulerAngles.z + 90) * Mathf.PI / 180f;

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        if (Collision2(xo1, yo1, ao1, xo2, yo2, ao2) > 0)
        {
            //cross.transform.GetComponent<Image>().color = Color.green;
            r.Length1 = Mathf.Sqrt((xc1 - xo1) * (xc1 - xo1) + (yc1 - yo1) * (yc1 - yo1));
            r.Length2 = Mathf.Sqrt((xc2 - xo2) * (xc2 - xo2) + (yc2 - yo2) * (yc2 - yo2));

            Transform nextRay = r.transform.GetChild(0);
            LightRay lr = nextRay.GetComponent<LightRay>();
            lr.isVisible = true;
            lr.gameObject.SetActive(true);

            lr.Col = r.Col;
            lr.Col.r = lr.Col.r * 0.2f;
            lr.StartPosition1 = new Vector3(xc1, yc1, 0);
            lr.StartPosition2 = new Vector3(xc2, yc2, 0);
            lr.Direction1 = ao1;
            lr.Direction2 = ao2;
            lr.Length1 = 15.0f;
            lr.Length2 = 15.0f;
  

            // Pour une lentille
            float zz1, theta1, thetaP1;
            float zz2, theta2, thetaP2;
            float cos = Mathf.Cos(angle);
            

            if (cos > 0.7f || cos < -0.7f)
            {
                zz1 = (xc1 - x) / cos;
                zz2 = (xc2 - x) / cos;
            }
            else
            {
                zz1 = (yc1 - y) / Mathf.Sin(angle);
                zz2 = (yc2 - y) / Mathf.Sin(angle);
            }
            theta1 = ao1 - (angle - Mathf.PI / 2);
            theta2 = ao2 - (angle - Mathf.PI / 2);

            if (Mathf.Cos(theta1) < 0) backSide = true;
            else backSide = false;

            if (backSide) {
                thetaP1 = Mathf.Atan(zz1 / focal + Mathf.Tan(theta1)) + Mathf.PI; // le nouvel angle
                thetaP2 = Mathf.Atan(zz2 / focal + Mathf.Tan(theta2)) + Mathf.PI; // le nouvel angle
            }
            else
            {
                thetaP1 = Mathf.Atan(-zz1 / focal + Mathf.Tan(theta1)); // le nouvel angle
                thetaP2 = Mathf.Atan(-zz2 / focal + Mathf.Tan(theta2)); // le nouvel angle
            }

            lr.Direction1 = thetaP1 + (angle - Mathf.PI / 2);
            lr.Direction2 = thetaP2 + (angle - Mathf.PI / 2);
        }
        else
        {
            Transform nextRay = r.transform.GetChild(0);
            LightRay lr = nextRay.GetComponent<LightRay>();
            lr.isVisible = false;
            lr.gameObject.SetActive(false);
        }
    }



    public void RayCollision(LightRay r) {
        x = transform.localPosition.x;
        y = transform.localPosition.y;
        angle = (transform.localRotation.eulerAngles.z + 90) * Mathf.PI / 180f;

        float xo = r.StartPosition1.x;
        float yo = r.StartPosition1.y;
        float ao = r.Direction1;

        if (Collision(xo, yo, ao) > 0)
        {
            //cross.transform.GetComponent<Image>().color = Color.green;
            r.Length1 = Mathf.Sqrt((xc - xo) * (xc - xo) + (yc - yo) * (yc - yo));
            r.Length2 = Mathf.Sqrt((xc - xo) * (xc - xo) + (yc - yo) * (yc - yo));

            Transform nextRay=r.transform.GetChild(0);
            LightRay lr = nextRay.GetComponent<LightRay>();
            lr.isVisible = true;
            lr.gameObject.SetActive(true);

            lr.Col = r.Col;
            lr.Col.r = lr.Col.r * 0.2f;
            lr.StartPosition1 = new Vector3(xc,yc,0);
            lr.Direction1 = ao;
            lr.Length1 = 5.0f;
            lr.Divergence = r.Divergence;
            lr.Width = 2f * Mathf.Tan(0.5f * r.Divergence) * r.Length1 + r.Width;


            // Pour une lentille
            float zz, theta,thetaP;

            float cos = Mathf.Cos(angle);
            if (cos > 0.7f || cos < -0.7f) 
            {
                zz = (xc - x) / cos;
            }
                else
                {
                    zz = (yc - y) / Mathf.Sin(angle);
                }
            theta = ao - (angle - Mathf.PI / 2);
            
            thetaP = zz / focal + theta; // le nouvel angle

            lr.Direction1 = thetaP+ (angle - Mathf.PI / 2);

            //lr.Divergence =2*Mathf.Atan( Mathf.Cos(thetaP) * ( lr.Width/2.0f  / focal - Mathf.Tan(r.Divergence/2.0f)/Mathf.Cos(theta)));
            lr.Divergence = 0f;
            lr.Width = 0.01f;


        }
        else
        {
            Transform nextRay = r.transform.GetChild(0);
            LightRay lr = nextRay.GetComponent<LightRay>();
            lr.isVisible = false;
            lr.gameObject.SetActive(false);
            //cross.transform.GetComponent<Image>().color = Color.red;
        }


        /*if (cross != null)
            cross.transform.localPosition = new Vector3(xc, yc, 0);*/


    }

}
