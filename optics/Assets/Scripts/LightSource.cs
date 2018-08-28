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
        //InitializeSource();
    }

    public void InitializeSource()
    {
        Vector3 pos = transform.localPosition;

        Rays = transform.parent.Find("Rays");
        LightRays = new LightRay[N];

        for (int i = 0; i < N; i++) {

            GameObject ray = new GameObject("Ray");
            ray.transform.SetParent(Rays);
            ray.transform.localScale = Vector3.one;
            ray.transform.localPosition = Vector3.zero;

            LightRay r =ray.AddComponent<LightRay>();
            
            r.Col = new Color(1, 1, 0.8f, 0.5f * (1 - (i + 0.5f - N / 2f) * (i + 0.5f - N / 2f) / N / N * 4.0f));
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
            r.Origin = null;
            LightRays[i] = r;


        }
    }
	
    public void EmitLight()
    {
        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Vector3 pos = transform.localPosition;//+Random.Range(0,0.001f)*Vector3.one;

        int i = 0;
        foreach (LightRay r in LightRays)
        {
            float l1 = - radius * (-0.5f + 0.5f / N + i / (float)N);
            float l2 = - radius * (-0.5f + 0.5f / N + (i + 1) / (float)N);

            r.StartPosition1 = pos + new Vector3(Mathf.Sin(angle) * l1, -Mathf.Cos(angle) * l1, 0);
            r.StartPosition2 = pos + new Vector3(Mathf.Sin(angle) * l2, -Mathf.Cos(angle) * l2, 0);

            r.Direction1 = Div * (-0.5f + 0.5f / N + i / (float)N) + angle;
            r.Direction2 = Div * (-0.5f + 0.5f / N + (i + 1) / (float)N) + angle;

            r.Length1 = r.Length2 = Length;

            i++;
            r.Width = 0;
            
            r.ComputeDir();

        }
    }


    void Update () {
        //EmitLight();
    }
}
