using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour {

    // Use this for initialization

    Transform Rays;
    public int N;
    public float Div;

    void Start () {

        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Debug.Log(transform.localRotation.eulerAngles.z);
        Vector3 pos = transform.localPosition;

       Rays = transform.parent.Find("Rays");

        for (int i = 0; i < N; i++) {

            GameObject ray = new GameObject("Ray");
            ray.transform.SetParent(Rays);
            ray.transform.localScale = Vector3.one;
            ray.transform.localPosition = Vector3.zero;


            LightRay r =ray.AddComponent<LightRay>();
            r.StartPosition = pos; //new Vector3(0.0f, 0, 0);
            r.Length = 5.0f;
            r.Width = 0.0f;
            r.Divergence = Div/N ; // 2 * Mathf.PI / 100;
            float a = Div * (-0.5f + i / (float)N);// + angle;
            r.Direction = a;// new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
            r.Col = new Color(1, 1, 0.8f, 0.5f);
            r.intensity = 0.01f;

            r.Initiliaze();
            r.Draw();

        }
    }
	
	// Update is called once per frame
	void Update () {

        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Vector3 pos = transform.localPosition;

        int i = 0;
        foreach(Transform t in Rays)
        {
            LightRay r = t.GetComponent<LightRay>();
            r.StartPosition = pos;
            r.Length = 5.0f;
            r.Divergence = Div / N;
            float a = Div * (-0.5f + i / (float)N) + angle;//float a = 2 * Mathf.PI * (-1.0f / 20.0f + 1 / 100.0f * i) + angle;
            r.Direction = a;
            i++;
            r.Draw();
        }


       /* r.StartPosition = pos;

        r.Direction = new Vector3(Mathf.Cos(2 * Mathf.PI / 100 * i), Mathf.Sin(2 * Mathf.PI / 100 * i), 0);

    */
    }
}
