using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour {

    // Use this for initialization

    const int MaxDepth = 10;

    Transform Rays;
    public int N;
    public float Div;
    public float Length;
    public float radius;
    public GameObject cross;
    LightRay[] LightRays;

    void Start () {

        Vector3 pos = transform.localPosition;

        Rays = transform.parent.Find("Rays");
        LightRays = new LightRay[N];

        for (int i = 0; i < N; i++) {

            GameObject ray = new GameObject("Ray");
            ray.transform.SetParent(Rays);
            ray.transform.localScale = Vector3.one;
            ray.transform.localPosition = Vector3.zero;

            LightRay r =ray.AddComponent<LightRay>();
            
            r.Col = new Color(1, 1, 0.8f, 0.5f * (1 - (i - N / 2f) * (i - N / 2f) / N / N * 4.0f));
            r.Initiliaze();
            r.isVisible = true;
            r.cross = cross;

            GameObject prev = ray;
            for (int k = 0; k < MaxDepth; k++)
            {
                GameObject rr = new GameObject("Ray");
                rr.transform.SetParent(prev.transform);
                rr.transform.localScale = Vector3.one;
                rr.transform.localPosition = Vector3.zero;

                LightRay lr = rr.AddComponent<LightRay>();
                lr.Initiliaze();
                lr.Col = new Color(1, 1, 0.8f, 0.5f);
                lr.isVisible = false;
                lr.gameObject.SetActive(false);
                lr.cross = cross;
                prev = rr;
            }

            LightRays[i] = r;


        }
    }
	
	// Update is called once per frame
	void Update () {

        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Vector3 pos = transform.localPosition;//+Random.Range(0,0.001f)*Vector3.one;

        int i = 0;
        //foreach(Transform t in Rays)
        foreach(LightRay r in LightRays)
        {
            //LightRay r = t.GetComponent<LightRay>();
            r.StartPosition1 =  pos;
            r.StartPosition2 = pos;
            r.Length1 = r.Length2 = Length;
            r.Divergence = Div / N;
            float a = Div * (-0.5f +0.5f/N + i / (float)N) + angle;//float a = 2 * Mathf.PI * (-1.0f / 20.0f + 1 / 100.0f * i) + angle;
            r.Direction1 = a;
            r.Direction2 = Div * (-0.5f + 0.5f / N + (i + 1) / (float)N) + angle;
            i++;
            r.Width = 0;
            //r.Draw();
        }

        /*foreach (Transform t in Rays)
        {
            LightRay r = t.GetComponent<LightRay>();
            float l = radius*(-0.5f + 0.5f / N + i / (float)N);
            r.StartPosition = pos + new Vector3(Mathf.Sin(angle)*l, -Mathf.Cos(angle) * l,0);
            r.Length = Length;
            //r.Divergence =0.001f;
            r.Direction = angle;
            i++;
            r.Width = radius  / (float)N;
            r.Draw();
        }*/

        /* r.StartPosition = pos;

         r.Direction = new Vector3(Mathf.Cos(2 * Mathf.PI / 100 * i), Mathf.Sin(2 * Mathf.PI / 100 * i), 0);

     */
    }
}
