using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour {

    public Vector3 StartPosition;
    public float Direction;
    public float Length;
    public Color Col;
    public float Width;
    public float Divergence;
    public float intensity=1;

    LineRenderer lr;


    public void Initiliaze()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.sortingLayerName="Rays";
    }

    public void Draw() {
     
        lr.SetPosition(0, StartPosition);
        lr.SetPosition(1, StartPosition + Length * new Vector3(Mathf.Cos(Direction), Mathf.Sin(Direction), 0));

        float s = transform.lossyScale.x;

        lr.startWidth = Width * s;
        lr.endWidth = Width + Divergence * Length * s;


        Color c = Col;
        //c.a = intensity / Width;
        lr.startColor = c;
       
        //c.a = intensity / Width * ( 1 - 10f*Divergence * Length);
        lr.endColor = c;
        
    }
	


}
