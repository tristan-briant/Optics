using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour {

	// Use this for initialization
	void Start () {

        for (int i = 0; i < 10; i++) {

            GameObject ray = new GameObject("Ray");

            LightRay r =ray.AddComponent<LightRay>();
            r.StartPosition = new Vector3(0.0f, 0, 0);
            r.Length = 5.0f;
            r.Width = 0.0f;
            r.Divergence = 2 * Mathf.PI / 100;
            r.Direction = new Vector3(Mathf.Cos(2*Mathf.PI / 100 * i), Mathf.Sin(2 * Mathf.PI / 100 * i), 0);
            r.Col = new Color(1, 1, 1, 0.5f);
            r.intensity = 0.01f;

            r.Draw();

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
