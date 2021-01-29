using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : OpticalComponent
{
    public int N = 10;
    public float vergence = 0;
    public float Length = 15;
    LightRay[] LightRays;
    public Color Color = new Color(1, 1, 0.8f, 0.5f);
    public float Intensity = 1;
    //private float lightRadius = 0;

    public float Vergence { get => vergence; set { vergence = value; hasChanged = true; } }

    override public void Update()
    {
        base.Update();
        LaunchStar();
    }

    public void InitializeSource()
    {
        LightRays = new LightRay[N];
        for (int i = 0; i < N; i++)
        {
            LightRay r = LightRay.NewLightRayChild();
            if (r != null)
            {
                r.Origin = this;
                LightRays[i] = r;
            }
        }
    }

    override public void Deflect(LightRay r)
    { // Simple obstacle
        while (r.transform.childCount > 0)
            r.transform.GetChild(0).GetComponent<LightRay>().FreeLightRay();

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;

        r.Length1 = Mathf.Sqrt((x - xo1) * (x - xo1) + (y - yo1) * (y - yo1));
        r.Length2 = Mathf.Sqrt((x - xo2) * (x - xo2) + (y - yo2) * (y - yo2));

    }

    public void EmitLight()
    {
        float angle = transform.rotation.eulerAngles.z * 2 * Mathf.PI / 360;
       
        Vector3 pos = new Vector3(x, y, 0);

        if (LightRays == null)
            InitializeSource();

        for (int i = 0; i < N; i++)
        {
            LightRay r = LightRays[i];
            if (r == null) return;

            // Couleur et intensité
            Color c = Color;
            c.a = c.a * (1 - (i + 0.5f - N / 2f) * (i + 0.5f - N / 2f) / (float)N / N * 4.0f);
            r.Col = c;
            r.Intensity = Intensity / N;

            // Calculs des positions et directions
            float l1 = -Radius * (-0.5f + i / (float)N);
            float l2 = -Radius * (-0.5f + (i + 1) / (float)N);

            r.StartPosition1 = pos + new Vector3(Mathf.Sin(angle) * l1, -Mathf.Cos(angle) * l1, 0);
            r.StartPosition2 = pos + new Vector3(Mathf.Sin(angle) * l2, -Mathf.Cos(angle) * l2, 0);

            r.Direction1 = Vergence * Radius * (-0.5f + i / (float)N) + angle;
            r.Direction2 = Vergence * Radius * (-0.5f + (i + 1) / (float)N) + angle;

            // Précalcul de paramètres géométriques utiles pour le calcul de collision
            r.ComputeDir();
        }
    }

    /*public void EmitLight2()
    {
        float angle = transform.localRotation.eulerAngles.z * 2 * Mathf.PI / 360;
        Vector3 pos = transform.localPosition;//+Random.Range(0,0.001f)*Vector3.one;

        int i = 0;
        foreach (LightRay r in LightRays)
        {
            float l1 = -radius * (-0.5f + i / (float)N);
            float l2 = -radius * (-0.5f + (i + 1) / (float)N);

            r.StartPosition1 = pos + new Vector3(Mathf.Sin(angle) * l1, -Mathf.Cos(angle) * l1, 0);
            r.StartPosition2 = pos + new Vector3(Mathf.Sin(angle) * l2, -Mathf.Cos(angle) * l2, 0);

            r.Direction1 = Div * (-0.5f + i / (float)N) + angle;
            r.Direction2 = Div * (-0.5f + (i + 1) / (float)N) + angle;

            r.Length1 = r.Length2 = Length;

            Color c = Color;
            c.a = c.a * (1 - (i + 0.5f - N / 2f) * (i + 0.5f - N / 2f) / (float)N / N * 4.0f);
            r.Col = c;
            r.Intensity = Intensity / N;

            r.ComputeDir();
            i++;
        }
    }*/

    void LaunchStar()
    {
        const float proba = 0.01f;

        if (LightRays != null && LightRays[N - 1] != null)
        {

            if (Random.Range(0.0f, 1.0f) < proba)
            {
                GameObject Star = Instantiate(Resources.Load("Star", typeof(GameObject)) as GameObject);
                StarFollowRay sf = Star.GetComponent<StarFollowRay>();
                sf.Initialize(LightRays[Random.Range(0, N)]);
            }
        }

    }

    public override void Delete()
    {
        foreach (LightRay lr in LightRays)
        {
            lr.FreeLightRay();
        }
    }
}
