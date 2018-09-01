using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour {

    public LightSource[] LightSources;
    public OpticalComponent[] OpticalComponents;
    public Target[] Targets;
    public int NRaysMax=10000;

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

        //if (i++ == 2) { i = 0; } else return;

        bool update = false;
        foreach (LightSource ls in LightSources)
        {
            if (ls.hasChanged)
            {
                update = true;
                break;
            }
        }
        foreach (OpticalComponent op in OpticalComponents)
        {
            if (op.hasChanged)
            {
                update = true;
                break;
            }
        }
        if (!update) return;

        ResetLightRay();

        foreach (LightSource ls in LightSources)
        {
            ls.EmitLight();
        }
        foreach(Target t in Targets)
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
        OpticalComponent opCollision = OpticalComponents[0];

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
                
                opCollision.Deflect(lr);
                foreach (Transform lchild in lr.transform)
                {
                    if(lchild.GetComponent<LightRay>().isVisible)
                        Collision(lchild.GetComponent<LightRay>());
                }
            }
            else
            {
                foreach (Transform lchild in lr.transform) {
                    lchild.gameObject.SetActive(false);
                }
            }


        ll = lmin;
        return false;
    }


    private void ResetLightRay()
    {
        foreach (LightRay r in Rays.GetComponentsInChildren<LightRay>())
        {
            r.transform.parent = RaysReserve;
            r.gameObject.SetActive(false);
        }
    }
}
