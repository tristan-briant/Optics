using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Component : MonoBehaviour {

    public float x=0,y=0; // position
    public float angle = 0;
    public float radius=0.1f;

    public GameObject cross;
    public GameObject other;


    // Use this for initialization
    public void Start () {
		
	}

    public float xc, yc;
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

    // Update is called once per frame
    void LateUpdate () {


        /* x = transform.localPosition.x;
         y = transform.localPosition.y;
         angle = (transform.localRotation.eulerAngles.z + 90) * Mathf.PI / 180f;

         float xo = other.transform.localPosition.x;
         float yo = other.transform.localPosition.y;
         float ao = (other.transform.localRotation.eulerAngles.z) * Mathf.PI / 180f;

         if(Collision(xo, yo, ao)>0)
             cross.transform.GetComponent<Image>().color=Color.green;
         else
             cross.transform.GetComponent<Image>().color = Color.red;


         if (cross != null)
             cross.transform.localPosition = new Vector3(xc, yc, 0);*/

        foreach (Transform t in transform.parent.Find("Rays"))
        {
            //Transform t = transform.parent.Find("Rays").GetChild(0);
            LightRay r = t.GetComponent<LightRay>();
            RayCollision(r);
            r.Draw();
        }
        
    }


    public void RayCollision(LightRay r) {
        x = transform.localPosition.x;
        y = transform.localPosition.y;
        angle = (transform.localRotation.eulerAngles.z + 90) * Mathf.PI / 180f;

        float xo = r.StartPosition.x;
        float yo = r.StartPosition.y;
        float ao = r.Direction;

        if (Collision(xo, yo, ao) > 0)
        {
            cross.transform.GetComponent<Image>().color = Color.green;
            r.Length = Mathf.Sqrt((xc - xo) * (xc - xo) + (yc - yo) * (yc - yo));
        }
        else
            cross.transform.GetComponent<Image>().color = Color.red;


        if (cross != null)
            cross.transform.localPosition = new Vector3(xc, yc, 0);


    }

}
