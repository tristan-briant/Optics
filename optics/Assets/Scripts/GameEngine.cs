using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour {

    public LightSource[] LightSources;
    public OpticalComponent[] OpticalComponents;
    public Transform Rays;

    public float ll;
	// Use this for initialization
	void Start () {
        LightSources = FindObjectsOfType<LightSource>();
        OpticalComponents = FindObjectsOfType<OpticalComponent>();
        Rays = GameObject.Find("Rays").transform;

        foreach (LightSource ls in LightSources)
        {
            ls.InitializeSource();
        }
    }

    // Update is called once per frame
    int i = 0;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        //if (i++ == 10) { i = 0; } else return;

        foreach (LightSource ls in LightSources)
        {
            ls.EmitLight();
        }

        /*foreach (OpticalComponent op in OpticalComponents)
        {
            op.Deflection();
        }*/



        foreach (Transform t in Rays)
        {
            LightRay lr = t.GetComponent<LightRay>();
            Collision(lr);
            lr.Draw();
        }
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
}
