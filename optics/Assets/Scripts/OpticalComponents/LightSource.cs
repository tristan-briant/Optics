using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : OpticalComponent
{
    public int N = 10;
    public float vergence = 0;
    public float Length = 15;

    private Color Color = new Color(1, 1, 0.8f, 0.5f);
    public float intensity = 1;
    public float lightRadius = 0.5f;

    public bool mini = false;

    public float Vergence { get => vergence; set { vergence = value; hasChanged = true; } }

    public float LightRadius { get => lightRadius; set { lightRadius = value; hasChanged = true; } }
    public float LightRadiusMax { get => mini ? 0.35f : 0.7f; }
    public float LightRadiusMin { get => 0.1f; }

    public bool red = true;
    public bool Red { get => red; set { red = value; hasChanged = true; } }

    public bool green = true;
    public bool Green { get => green; set { green = value; hasChanged = true; } }

    public bool blue = true;
    public bool Blue { get => blue; set { blue = value; hasChanged = true; } }


    public float Intensity { get => intensity; set { intensity = value; hasChanged = true; } }
    public float IntensityMax { get => 1f; }
    public float IntensityMin { get => 0.1f; }

    [System.NonSerialized]
    public List<LightRay> LightRays = new List<LightRay>();

    Color LightColor()
    {
        return new Color(red ? 1f : 0f, green ? 1f : 0f, blue ? 0.8f : 0f, 0.5f * Intensity);
    }

    override public void Update()
    {
        base.Update();
        LaunchStar();
    }

    public void InitializeSource()
    {
        //if (LightRays == null)
        //    LightRays = new LightRay[N];
        if (LightRays.Count < N)
            for (int i = LightRays.Count; i < N; i++)
            {
                LightRay r = LightRay.NewLightRayChild();
                if (r == null) break; // no more ray available !
                r.Origin = this;
                LightRays.Add(r);
            }

        /*
            for (int i = 0; i < N; i++)
            {
                if (LightRays[i] == null)
                {
                    LightRay r = LightRay.NewLightRayChild();
                    if (r != null)
                    {
                        r.Origin = this;
                        LightRays[i] = r;
                    }
                }
            }*/
        }

    override public void Deflect(LightRay r)
    { // Simple obstacle
       
        r.ClearChildren();
        
        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;

        r.Length1 = Mathf.Sqrt((x - xo1) * (x - xo1) + (y - yo1) * (y - yo1));
        r.Length2 = Mathf.Sqrt((x - xo2) * (x - xo2) + (y - yo2) * (y - yo2));
        //r.Length1 = Mathf.Sqrt((xc1 - xo1) * (xc1 - xo1) + (yc1 - yo1) * (yc1 - yo1));
        //r.Length2 = Mathf.Sqrt((xc2 - xo2) * (xc2 - xo2) + (yc2 - yo2) * (yc2 - yo2));

    }

    override public float Collision2(LightRay lr)
    {
        float l1 = CircularCollision(lr, 1);
        xc1 = xc; yc1 = yc;
        if (l1 < 0) return -1;
        float l2 = CircularCollision(lr, 2);
        xc2 = xc; yc2 = yc;
        if (l2 < 0) return -1;

        return l1;
    }

    public void EmitLight()
    {
        float EmitAngle = angle - Mathf.PI / 2;

        Vector3 pos = new Vector3(x, y, 0);

        //if (LightRays == null || LightRays[N - 1] == null)
        if(LightRays.Count<N)
            InitializeSource();

        Color SourceColor = LightColor();

        for (int i = 0; i < LightRays.Count; i++)
        {
            LightRay r = LightRays[i];
            if (r == null) return;

            // Couleur et intensité

            Color c = SourceColor;
            c.a = c.a * (1 - (i + 0.5f - N / 2f) * (i + 0.5f - N / 2f) / (float)N / N * 4.0f);
            r.Col = c;
            r.Intensity = Intensity / N;

            // Calculs des positions et directions
            float l1 = -lightRadius * (-0.5f + i / (float)N);
            float l2 = -lightRadius * (-0.5f + (i + 1) / (float)N);

            r.StartPosition1 = pos + new Vector3(Mathf.Sin(EmitAngle) * l1, -Mathf.Cos(EmitAngle) * l1, 0);
            r.StartPosition2 = pos + new Vector3(Mathf.Sin(EmitAngle) * l2, -Mathf.Cos(EmitAngle) * l2, 0);

            r.Direction1 = Vergence * lightRadius * (-0.5f + i / (float)N) + EmitAngle;
            r.Direction2 = Vergence * lightRadius * (-0.5f + (i + 1) / (float)N) + EmitAngle;

            // Précalcul de paramètres géométriques utiles pour le calcul de collision
            r.ComputeDir();
        }
    }

    void LaunchStar()
    {
        const float proba = 0.01f;

        //if (LightRays != null && LightRays[N - 1] != null)
        if(LightRays.Count>0)
        {

            if (Random.Range(0.0f, 1.0f) < proba)
            {
                GameObject Star = Instantiate(Resources.Load("Star", typeof(GameObject)) as GameObject);
                StarFollowRay sf = Star.GetComponent<StarFollowRay>();
                sf.Initialize(LightRays[Random.Range(0, LightRays.Count)]);
            }
        }

    }

    public override void Delete()
    {
        if (LightRays != null)
            foreach (LightRay lr in LightRays)
            {
                lr.FreeLightRay();
            }

        DestroyImmediate(gameObject);
    }
}
