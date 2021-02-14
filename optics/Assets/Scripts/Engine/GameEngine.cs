using System.Collections;
using System;
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
    public int DepthMax = 2;
    public bool running = false;
    public bool levelLoaded = false;

    [System.NonSerialized]
    public static GameEngine instance;


    Vector3 CamPosition = Vector3.zero;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    void Start()
    {
        Application.targetFrameRate = 30;
        //RaysReserve.gameObject.SetActive(false);

        // Initialize Ray system (static variables)
        LightRay.RaysReserve = RaysReserve;
        LightRay.Rays = Rays;
        LightRay.DepthMax = DepthMax;

        StartCoroutine("FillRaysReserve");
    }

    IEnumerator FillRaysReserve()
    {
        for (int i = 0; i < NRaysMax; i++)
        {
            int k = i;
            for (; i < k + 100 && i < NRaysMax; i++)
                LightRay.InstantiateLightRay();

            yield return new WaitForFixedUpdate();
        }
    }

    public void StartGameEngine()
    {
        UpdateComponentList();
        running = true;
    }

    public void StopGameEngine()
    {
        ResetLightRay();
        Array.Clear(LightSources, 0, LightSources.Length);
        Array.Clear(OpticalComponents, 0, OpticalComponents.Length);
        Array.Clear(Targets, 0, Targets.Length);
        running = false;
    }

    void LateUpdate()
    {
        if (!running) return;

        bool update = false;

        /*Vector3 CamPositionNew = Camera.main.transform.localPosition;
        if (CamPositionNew != CamPosition)
        {

            foreach (LightSource ls in LightSources)
                foreach (LightRay lr in ls.LightRays)
                    lr.SetRelativePosition(CamPositionNew - CamPosition);

            CamPosition = CamPositionNew;
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
                if (op is LightSource) // C'est une source on update les rayons emit (plus la source en tant que op ensuite) 
                    UpdateLightRays1LS(op as LightSource);

                UpdateLightRays1OP(op);

                update = true;
            }
        }

        if (update)
        {
            foreach (Target t in Targets) t.ComputeScore();
        }
    }

    private void UpdateAllRays()
    {
        foreach (LightSource ls in LightSources)
            UpdateLightRays1LS(ls);

        foreach (LightSource ls in LightSources) ls.hasChanged = false;
        foreach (OpticalComponent op in OpticalComponents) op.hasChanged = false;
        foreach (Target t in Targets) t.ComputeScore();
    }

    bool Collision(LightRay lr)
    {
        float dmin = -1;
        OpticalComponent opCollision = null;

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (lr.Origin == op) continue;

            float d = op.Collision2(lr);

            if (d > 0 && (d < dmin || dmin < 0)) // trouve la plus proche collision
            {
                dmin = d;
                opCollision = op;
            }
        }

        if (dmin > 0)
        {
            lr.End = opCollision;
            opCollision.Deflect(lr);

            foreach (LightRay lchild in lr.Children)
                Collision(lchild);
        }
        else  // Si on touche personne
        {
            lr.Length1 = lr.Length2 = LengthMax;
            lr.End = null;

            // On retire tous les rayon enfants
            lr.ClearChildren();
        }

        lr.DrawMesh(CamPosition); // Met à jour le mesh centrer sur la camera
        return false;
    }

    public void ResetLightRay()
    {
        foreach (LightSource lightSource in GameEngine.instance.LightSources)
            foreach (LightRay lr in lightSource.LightRays)
                lr.FreeLightRay();
    }

    void UpdateLightRays1LS(LightSource ls)
    {
        ls.EmitLight();
        foreach (LightRay lr in ls.LightRays)
            Collision(lr);

        ls.hasChanged = false;
    }

    void UpdateLightRays1OP(OpticalComponent op)
    {
        foreach (LightSource lightSource in GameEngine.instance.LightSources)
            foreach (LightRay lr in lightSource.LightRays)
                Update1LightRay1OP(lr, op);

        op.hasChanged = false;
    }

    private void Update1LightRay1OP(LightRay lr, OpticalComponent op)
    {
        if (Collision1OP(lr, op)) // si nouvelle collision ou perte de collision
            Collision(lr); // recalcule tout le rayon
        else
            foreach (LightRay lchild in lr.Children)
                Update1LightRay1OP(lchild, op);

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
