using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class GameEngine : MonoBehaviour
{

    public LightSource[] LightSources;
    public OpticalComponent[] OpticalComponents;
    public Target[] Targets;
    public int NRaysMax = 1000;
    public float LengthMax = 15.0f;
    public Transform Rays;
    public Transform RaysReserve;
    public int DepthMax = 10;
    public bool running = false;
    public bool levelLoaded = false;


    Vector3 CamPositionPrev = Vector3.zero;


    void Start()
    {
        Application.targetFrameRate = 30;
        RaysReserve = GameObject.Find("RaysReserve").transform;  // find and deactivate
        RaysReserve.gameObject.SetActive(false);
        Rays = GameObject.Find("Rays").transform;

        // Initialize Ray system (static variables)
        LightRay.RaysReserve = RaysReserve;
        LightRay.Rays = Rays;
        LightRay.DepthMax = DepthMax;

        StartCoroutine("FillRaysReserve");
    }

    IEnumerator FillRaysReserve()
    {
        for (int i = 0; i < LightRay.rayNumberMax; i++)
        {
            int k = i;
            for (; i < k + 100 && i < LightRay.rayNumberMax; i++)
                LightRay.InstantiateLightRay();

            yield return null;
        }
    }

    public void StartGameEngine()
    {
        UpdateComponentList();
        Rays = GameObject.Find("Rays").transform;
        Debug.Log("Nombre de ray en reserve : " + RaysReserve.transform.childCount);
        running = true;
    }

    void LateUpdate()
    {
        //Profiler.BeginSample("MyPieceOfCode");

        if (!running) return;

        bool update = false;

        if (Camera.main.transform.localPosition != CamPositionPrev)
        {
            CamPositionPrev = Camera.main.transform.localPosition;

            foreach (LightRay lr in Rays.GetComponentsInChildren<LightRay>())
                lr.DrawMesh();
        }

        /*foreach (LightSource ls in LightSources)
        {
            if (ls.hasChanged)
            {
                update = true;
                break;
            }
        }

        if (update)
        {
            UpdateAllRays();
            return;
        }*/

        if (LightRay.NewRaysAvailable)
        {
            UpdateAllRays();
            return;
        }


        foreach (OpticalComponent op in OpticalComponents)
        {
            if (op.hasChanged)
            {
                if (op.GetComponent<LightSource>()) // C'est une source on update tout par défaut
                {
                    UpdateAllRays();
                    return;
                }

                UpdateLightRays1OP(op);
                update = true;
            }
        }

        if (update)
        {
            foreach (Target t in Targets) t.ComputeScore();
        }

        //Profiler.EndSample();
    }

    private void UpdateAllRays()
    {

        foreach (LightSource ls in LightSources)
        {
            ls.EmitLight();
        }

        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(lr);
            lr.Draw();
        }

        foreach (LightSource ls in LightSources) ls.hasChanged = false;
        foreach (OpticalComponent op in OpticalComponents) op.hasChanged = false;
        foreach (Target t in Targets) t.ComputeScore();

    }

    bool Collision(LightRay lr)
    {

        float lmin = -1;
        OpticalComponent opCollision = null;

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (lr.Origin == op) continue;

            float l = op.Collision2(lr);

            if (l > 0 && (l < lmin || lmin < 0)) // trouve la plus proche collision
            {
                lmin = l;
                opCollision = op;
            }
        }

        if (lmin > 0)
        {
            lr.End = opCollision;
            opCollision.Deflect(lr);

            foreach (Transform lchild in lr.transform)
                Collision(lchild.GetComponent<LightRay>());

        }
        else  // Si on touche personne
        {
            lr.Length1 = lr.Length2 = LengthMax;
            lr.End = null;

            // On retire tous les rayon enfants
            while (lr.transform.childCount > 0) // Attention le foreach ne marche pas car on change le nombre de child !
                                                //ResetLightRay(lr.transform.GetChild(0).GetComponent<LightRay>());
                lr.transform.GetChild(0).GetComponent<LightRay>().FreeLightRay();
        }

        return false;
    }

    public void ResetLightRay()
    {
        foreach (LightRay r in Rays.GetComponentsInChildren<LightRay>())
            r.FreeLightRay();
    }

    private void UpdateLightRays1OP(OpticalComponent op)
    {
        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Update1LightRay1OP(lr, op);
        }

        op.hasChanged = false;

    }

    private void Update1LightRay1OP(LightRay lr, OpticalComponent op)
    {
        if (Collision1OP(lr, op)) // si nouvelle collision ou perte de collision
        {
            Collision(lr);
            lr.Draw();
        }
        else
        {
            foreach (Transform lchild in lr.transform)
            {
                Update1LightRay1OP(lchild.GetComponent<LightRay>(), op);
            }
        }
    }

    bool Collision1OP(LightRay lr, OpticalComponent op) // test la collision avec 1 optical component
    {
        if (lr.End == op) // si l'op touchait le rayon, on l'update
        {
            lr.Length1 = LengthMax; // On redonne une long max pour le test Fast collision 
            return true;
        }
        float l = op.Collision2(lr);
        if (l > 0) return true; // si l'op touche le rayon on l'update
        return false;
    }

    public void UpdateComponentList()
    {
        LightSources = FindObjectsOfType<LightSource>();
        OpticalComponents = FindObjectsOfType<OpticalComponent>();
        Targets = FindObjectsOfType<Target>();

        UpdateAllRays();
    }

}
