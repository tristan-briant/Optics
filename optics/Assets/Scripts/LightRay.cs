using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour {

    public Vector3 StartPosition;
    public Vector3 Direction;
    public float Length;
    public Color Col;
    public float Width;
    public float Divergence;
    public float intensity=1;

    LineRenderer lr;

    public void Draw() {
        lr = gameObject.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Particles/Additive"));

        lr.SetPosition(0, StartPosition);
        lr.SetPosition(1, StartPosition + Length * Direction);

        lr.startWidth = Width;
        lr.endWidth = Width + Divergence*Length;

        Color c = Col;
        c.a = intensity / Width;
        lr.startColor = c;
       
        c.a = intensity / (Width + Divergence * Length);
        lr.endColor = c;
        
    }
	


}
