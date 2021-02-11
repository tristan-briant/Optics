using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour
{
    public static int rayNumber;
    public const int rayNumberMax = 1000;
    private static bool newRaysAvailable;
    public Vector3 StartPosition1, StartPosition2;
    public float Direction1, Direction2;
    public float Length1, Length2;
    public Color Col;
    public float Intensity = 0.05f;
    public bool isVisible;
    public bool HasWaist;
    public float cos1, sin1, cos2, sin2, proj1, proj2, param1, param2, div; // vecteur directeur, proj et parametre
    public OpticalComponent Origin;
    public OpticalComponent End;
    public int depth;

    public List<LightRay> Children;

    static public Transform RaysReserve;
    static public Transform Rays;
    static public int DepthMax;

    const float EPSILON = 0.00001f; // pour les erreurs d'arrondis

    public static bool NewRaysAvailable { get { bool value = newRaysAvailable; newRaysAvailable = false; return value; } set => newRaysAvailable = value; }

    Vector3[] vertices;
    Vector2[] uv;

    Mesh mesh;
    MeshRenderer meshRenderer;

    void InitializeMesh()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        Material mat = Resources.Load("Materials/RayDiv", typeof(Material)) as Material;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mat;
        meshRenderer.material.color = Col;
        meshRenderer.sortingLayerName = "Rays";

        mesh = new Mesh
        {
            vertices = new Vector3[6],
            triangles = new int[6] { 0, 1, 2, 3, 4, 5 }
        };

        mesh.normals = new Vector3[6] {
                -Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward
            };
        mesh.uv = new Vector2[6] {
                 new Vector2(0f,0f),new Vector2(0f,0f),new Vector2(1f,0f),new Vector2(1f,0f),new Vector2(0f,0f),new Vector2(1f,0f)
            };

        mf.mesh = mesh;

        vertices = new Vector3[6];
        uv = new Vector2[6];

    }

    public void Draw()
    {
        // Draw the rays recursively;
        DrawMesh();
        foreach (Transform child in transform)
        {
            child.GetComponent<LightRay>().Draw();
        }
    }

    public void ComputeDir()
    { //Calcule les vecteurs directeurs et paramètres
        cos1 = Mathf.Cos(Direction1);
        sin1 = Mathf.Sin(Direction1);
        proj1 = StartPosition1.x * cos1 + StartPosition1.y * sin1;
        param1 = -StartPosition1.x * sin1 + StartPosition1.y * cos1;
        cos2 = Mathf.Cos(Direction2);
        sin2 = Mathf.Sin(Direction2);
        proj2 = StartPosition2.x * cos2 + StartPosition2.y * sin2;
        param2 = -StartPosition2.x * sin2 + StartPosition2.y * cos2;

        div = Direction2 - Direction1;
        if (div < 0) div = -div;
        if (div > 2 * Mathf.PI) div -= 2 * Mathf.PI;
    }

    public void DrawMesh()
    {
        //Test if a waist exists
        Vector3 EndPosition1 = StartPosition1 + Length1 * new Vector3(cos1, sin1, 0);
        Vector3 EndPosition2 = StartPosition2 + Length2 * new Vector3(cos2, sin2, 0);

        float p1 = param1;
        float p2start = -StartPosition2.x * sin1 + StartPosition2.y * cos1;
        float p2end = -EndPosition2.x * sin1 + EndPosition2.y * cos1;

        if (p2start < p1 && p2end > p1 || p2start > p1 && p2end < p1)
        {
            Vector3 WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * StartPosition2) / (p2start - p2end);

            float cs1, cs2, ce1, ce2; //Couleur des vertex ( Shader uv en 1/u )

            float Ilum = div / Intensity;

            cs1 = (cos1 * (WaistPos.x - StartPosition1.x) + sin1 * (WaistPos.y - StartPosition1.y)) * Ilum;
            if (cs1 < 0) cs1 = -cs1;
            cs2 = (cos2 * (WaistPos.x - StartPosition2.x) + sin2 * (WaistPos.y - StartPosition2.y)) * Ilum;
            if (cs2 < 0) cs2 = -cs2;
            ce1 = (cos1 * (WaistPos.x - EndPosition1.x) + sin1 * (WaistPos.y - EndPosition1.y)) * Ilum;
            if (ce1 < 0) ce1 = -ce1;
            ce2 = (cos2 * (WaistPos.x - EndPosition2.x) + sin2 * (WaistPos.y - EndPosition2.y)) * Ilum;
            if (ce2 < 0) ce2 = -ce2;

            vertices[0] = StartPosition1;
            vertices[1] = StartPosition2;
            vertices[2] = WaistPos;
            vertices[3] = WaistPos;
            vertices[4] = EndPosition1;
            vertices[5] = EndPosition2;

            uv[0].x = cs1;
            uv[1].x = cs2;
            uv[2].x = 0f;
            uv[3].x = 0f;
            uv[4].x = ce1;
            uv[5].x = ce2;

            uv[0].y = uv[1].y = uv[2].y = uv[3].y = uv[4].y = uv[5].y = 0f;
        }
        else
        {
            float cs1, cs2, ce1, ce2; //Couleur des vertex ( Shader uv en 1/u )

            float Ilum = div / Intensity;

            float p = p2start - p2end;
            if (p < 0) p = -p;

            if (p > EPSILON)
            {
                Vector3 WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * StartPosition2) / (p2start - p2end);

                cs1 = (cos1 * (WaistPos.x - StartPosition1.x) + sin1 * (WaistPos.y - StartPosition1.y)) * Ilum;
                if (cs1 < 0) cs1 = -cs1;
                cs2 = (cos2 * (WaistPos.x - StartPosition2.x) + sin2 * (WaistPos.y - StartPosition2.y)) * Ilum;
                if (cs2 < 0) cs2 = -cs2;
                ce1 = (cos1 * (WaistPos.x - EndPosition1.x) + sin1 * (WaistPos.y - EndPosition1.y)) * Ilum;
                if (ce1 < 0) ce1 = -ce1;
                ce2 = (cos2 * (WaistPos.x - EndPosition2.x) + sin2 * (WaistPos.y - EndPosition2.y)) * Ilum;
                if (ce2 < 0) ce2 = -ce2;
            }
            else
            {   // Faisceau collimaté
                float cc = (p2start - p1);
                if (cc < 0) cc = -cc;
                cs1 = cs2 = ce1 = ce2 = cc / Intensity;
            }

            vertices[0] = StartPosition1;
            vertices[1] = StartPosition2;
            vertices[2] = EndPosition1;
            vertices[3] = EndPosition1;
            vertices[4] = StartPosition2;
            vertices[5] = EndPosition2;

            uv[0].x = cs1;
            uv[1].x = cs2;
            uv[2].x = ce1;
            uv[3].x = ce1;
            uv[4].x = cs2;
            uv[5].x = ce2;
            uv[0].y = uv[1].y = uv[2].y = uv[3].y = uv[4].y = uv[5].y = 0f;
        }

        Vector3 OffsetPosition = Camera.main.transform.position;
        for (int i = 0; i < 6; i++)
            vertices[i] -= OffsetPosition;

        mesh.vertices = vertices;
        mesh.uv = uv;
        meshRenderer.material.color = Col;

    }

    public void FreeLightRay() // remove child recursively
    {
        if (RaysReserve.transform.childCount == 0)
            newRaysAvailable = true;

        foreach (LightRay r in GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = RaysReserve;
            r.End = null;
            r.Origin = null;
        }

        /*foreach (LightRay r in Children)
        {
            r.FreeLightRay();
        }

        Children.Clear();
        gameObject.SetActive(false);*/

    }

    static public LightRay NewLightRayChild(LightRay lr = null)
    {
        if (lr && lr.depth >= DepthMax) return null; // profondeur max atteinte !!
        if (RaysReserve == null || RaysReserve.childCount == 0)
        {
            return null; // Plus de rayons disponible !!
        }

        // Preparation du rayon
        LightRay r = RaysReserve.GetChild(0).GetComponent<LightRay>();
        if (lr)
        {
            r.transform.parent = lr.transform;
            r.depth = lr.depth + 1;
        }
        else
        {
            r.transform.parent = Rays;
            r.depth = 0;
        }

        r.transform.localScale = Vector3.one;
        r.transform.localPosition = Vector3.zero;
        return r;
    }

    static public LightRay InstantiateLightRay()
    {
        if (RaysReserve.transform.childCount == 0)
            newRaysAvailable = true;

        GameObject ray = new GameObject("Ray");
        ray.transform.parent = (RaysReserve);
        ray.transform.localScale = Vector3.one;
        ray.transform.localPosition = Vector3.zero;

        LightRay r = ray.AddComponent<LightRay>();
        r.InitializeMesh();
        rayNumber++;
        return r;
    }

}
