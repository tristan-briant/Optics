using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRay : MonoBehaviour {

    public Vector3 StartPosition1, StartPosition2;
    public float Direction1, Direction2;
    public float Length1,Length2;
    public Color Col;
    public float Width;
    public float Divergence;
    public float Intensity=1;
    public bool isVisible;
    public bool HasWaist;
    public GameObject cross;
    Vector3 WaistPos;
    LineRenderer lr;
    public float cos1, sin1, cos2, sin2, proj1, proj2, param1, param2; // vecteur directeur, proj et parametre
    public OpticalComponent Origin;


    Mesh mesh;
    void InitilizedMesh()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        
        Material mat = Resources.Load("Materials/Material Line", typeof(Material)) as Material;
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = mat;
        mr.material.color = Col;
        mr.sortingLayerName="Rays";

        Mesh m = new Mesh();

 

        m.vertices = new Vector3[6];
        m.triangles = new int[6] { 0, 1, 2, 3, 4, 5 }; ;
        m.normals = new Vector3[6] {
                -Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward
            };
        m.uv = new Vector2[6] {
                 new Vector2(0.45f,0.5f),new Vector2(0.55f,0.5f),new Vector2(0.55f,0.5f),new Vector2(0.45f,0.5f),new Vector2(0.45f,0.5f),new Vector2(0.45f,0.5f)
            }; 

        GetComponent<MeshFilter>().mesh=m;

    }

    void InitializeLinerenderer() {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        //lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        //Material mat = Resources.Load("Materials/Material Line", typeof(Material)) as Material;
        //lr.material = mat;
        lr.sortingLayerName = "Rays";
    }

    public void Initiliaze()
    {
        //InitializeLinerenderer();
        InitilizedMesh();
    }

    public void Draw() {
        // Draw the rays recursively;
        //if (!isVisible) return;
        DrawMesh();
        foreach(Transform child in transform)
        {
            child.GetComponent<LightRay>().Draw();
        }
    }

 
    void LateUpdate()
    {
        //Draw();
    }

    public void ComputeDir() { //Calcule les vecteurs directeurs
        cos1 = Mathf.Cos(Direction1);
        sin1 = Mathf.Sin(Direction1);
        proj1 = StartPosition1.x * cos1 + StartPosition1.y * sin1;
        param1 = -StartPosition1.x * sin1 + StartPosition1.y * cos1;
        cos2 = Mathf.Cos(Direction2);
        sin2 = Mathf.Sin(Direction2);
        proj2 = StartPosition2.x * cos2 + StartPosition2.y * sin2;
        param2 = -StartPosition2.x * sin2 + StartPosition2.y * cos2;


    }

    void DrawMesh() {
       
        Vector3[] vertices;
        
        //Test if a waist exists
        Vector3 EndPosition1 = StartPosition1 + Length1 * new Vector3(cos1, sin1, 0);
        Vector3 EndPosition2 = StartPosition2 + Length2 * new Vector3(cos2, sin2, 0);

        /*float cos1 = Mathf.Cos(Direction1);
        float sin1 = Mathf.Sin(Direction1);*/

        float p1 = param1;  //-StartPosition1.x * sin1 + StartPosition1.y * cos1;
        float p2start = -StartPosition2.x * sin1 + StartPosition2.y * cos1;
        float p2end = -EndPosition2.x * sin1 + EndPosition2.y * cos1;

        if (p2start<p1 && p2end> p1 || p2start > p1 && p2end<p1)
        {
            HasWaist = true;
            WaistPos = ((p2start - p1) * EndPosition2 - (p2end - p1) * StartPosition2) / (p2start - p2end);

           
            vertices = new Vector3[6] {
                StartPosition1,
                StartPosition2,
                WaistPos,
                WaistPos,
                EndPosition1,
                EndPosition2
            };

            /*tri = new int[6] { 0, 1, 2, 3, 4, 5 };

            normals = new Vector3[6] {
                -Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward,-Vector3.forward
            };

            //UV
            uv = new Vector2[6] {
                 new Vector2(0.45f,0.5f),new Vector2(0.55f,0.5f),new Vector2(0.55f,0.5f),new Vector2(0.45f,0.5f),new Vector2(0.45f,0.5f),new Vector2(0.45f,0.5f)
            };*/

        }
        else
        {
            HasWaist = false;

            vertices = new Vector3[6] {
                StartPosition1,
                StartPosition2,
                EndPosition1,
                EndPosition1,
                EndPosition2,
                StartPosition2
            };
           

        }

        Mesh m = GetComponent<MeshFilter>().mesh;

        m.vertices = vertices;
      
        GetComponent<MeshRenderer>().material.color = Col;

    }

    void DrawMeshOld() {
        Vector3[] vertices = new Vector3[4] {
            StartPosition1,
            StartPosition2,
            StartPosition2 + Length2 * new Vector3(Mathf.Cos(Direction2), Mathf.Sin(Direction2), 0),
            StartPosition1 + Length1 * new Vector3(Mathf.Cos(Direction1), Mathf.Sin(Direction1), 0)
        };

        /*Vector3[] vertices = new Vector3[4] {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(0,1,0)
        };*/

        mesh.vertices = vertices;
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh=mesh;
    }

    void DrawLineRenderer() {
        lr.SetPosition(0, StartPosition1);
        lr.SetPosition(1, StartPosition1 + Length1 * new Vector3(Mathf.Cos(Direction1), Mathf.Sin(Direction1), 0));

        float s = transform.lossyScale.x;

        lr.startWidth = Width * s;
        lr.endWidth = (Width + 2f * Mathf.Tan(0.5f * Divergence) * Length1) * s;


        Color c = Col;
        //c.a = intensity / Width;
        lr.startColor = c;
       
        //c.a = intensity / Width * ( 1 - 10f*Divergence * Length);
        lr.endColor = c;

        foreach(Transform child in transform)
        {
            child.GetComponent<LightRay>().Draw();
        }

    }


    public void Propagate()
    {

    }



}
