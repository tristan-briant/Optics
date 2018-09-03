using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour {

    public LightSource[] LightSources;
    public OpticalComponent[] OpticalComponents;
    public Target[] Targets;
    public int NRaysMax=5;
    public float LengthMax=15.0f;

    public Transform Rays;
    public Transform RaysReserve;
    public int DepthMax = 10;

    public float ll;
    // Use this for initialization
    void Start()
    {
        LightSources = FindObjectsOfType<LightSource>();
        OpticalComponents = FindObjectsOfType<OpticalComponent>();
        Targets = FindObjectsOfType<Target>();
        Rays = GameObject.Find("Rays").transform;
        RaysReserve = GameObject.Find("RaysReserve").transform;

        foreach (LightSource ls in LightSources)
        {
            //ls.InitializeSource();
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            op.DepthMax = DepthMax;
            op.Rays = Rays;
            op.RaysReserve = RaysReserve;
        }

            for (int i = 0; i < NRaysMax; i++)
        {

            GameObject ray = new GameObject("Ray");
            ray.transform.SetParent(RaysReserve);
            ray.transform.localScale = Vector3.one;
            ray.transform.localPosition = Vector3.zero;

            LightRay r = ray.AddComponent<LightRay>();

            r.Initiliaze();
            r.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    int i = 0;
    void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (i++ == 1) { i = 0; } else return;

        bool update = false;

        foreach (LightSource ls in LightSources)
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
        }

        OpticalComponent opUpdate=null;

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (op.hasChanged)
            {
                update = true;
                opUpdate = op;
                break;
            }
        }
        if (update) {
            UpdateLightRays1OP(opUpdate);
            opUpdate.hasChanged = false;
        }
    }

    private void UpdateAllRays()
    {
        
        ResetLightRay();

        foreach (LightSource ls in LightSources)
        {
            ls.EmitLight();
        }
        foreach (Target t in Targets)
        {
            t.ResetTarget();
        }


        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(lr);
            lr.Draw();
        }

        foreach (LightSource ls in LightSources) ls.hasChanged = false;
        foreach (OpticalComponent op in OpticalComponents) op.hasChanged = false;


    }


    bool Collision(LightRay lr) {

        float lmin = -1;
        OpticalComponent opCollision = null;

        while (lr.transform.childCount>0) // Attention le foreach ne marche pas car on change le nombre de child !
        {
            ResetLightRay(lr.transform.GetChild(0).GetComponent<LightRay>());
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (lr.Origin == op) continue;

            float l = op.Collision2(lr);

            if (l > 0 && ( l < lmin || lmin <0 ) ) // trouve la plus proche collision
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
            {
                Collision(lchild.GetComponent<LightRay>());
            }
        }
        else
        {
             lr.Length1 = lr.Length2 = LengthMax;
        }


        ll = lmin;
        return false;
    }

    

    private void ResetLightRay()
    {
        foreach (LightRay r in Rays.GetComponentsInChildren<LightRay>())
        {
            r.gameObject.SetActive(false);
            r.transform.parent = RaysReserve;
            r.End = null;
            r.Origin = null;
        }
    }


    private void UpdateLightRays1OP(OpticalComponent op)
    {
        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Update1LightRay1OP(lr,op);
        }

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

    bool Collision1OP(LightRay lr,OpticalComponent op) // test la collision avec 1 optical component
    {
        if (lr.End == op ) return true;  // si l'op touchait le rayon, on l'update
        float l = op.Collision2(lr);
        if ( l>0 ) return true; // si l'op touche le rayon on l'update
        return false;
    }

    private void ResetLightRay(LightRay ray) // remove child recursively
    {
        foreach (LightRay r in ray.GetComponentsInChildren<LightRay>())
        {
            r.gameObject.SetActive(false);
            r.transform.parent = RaysReserve;
            r.End = null;
            r.Origin = null;
        }

        /*
        while (ray.transform.childCount > 0)
        {
            ResetLightRay(ray.transform.GetChild(0).GetComponent<LightRay>());
        }

        ray.gameObject.SetActive(false);
        ray.transform.parent = RaysReserve;
        ray.End = null;
        ray.Origin = null;
        */
    }

}
