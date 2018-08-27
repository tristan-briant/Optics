using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour {

    public LightSource[] LightSources;
    public OpticalComponent[] OpticalComponents;
    public Transform Rays;

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
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        foreach (LightSource ls in LightSources)
        {
            ls.EmitLight();
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            op.Deflection();
        }



        foreach (Transform t in Rays)
        {
            t.GetComponent<LightRay>().Draw();
        }
    }
}
