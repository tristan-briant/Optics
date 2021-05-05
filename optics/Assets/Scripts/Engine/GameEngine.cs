using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class GameEngine : MonoBehaviour
{
    [HideInInspector]
    public LightSource[] LightSources;
    [HideInInspector]
    public OpticalComponent[] OpticalComponents;
    [HideInInspector]
    public Target[] Targets;

    [Header("Performance")]
    public int NRaysMax = 1000;
    public float LengthMax = 15.0f;
    public int DepthMax = 10;

    public Transform Rays;


    public enum Mode { Edit, Play, Inactive }
    public Mode PlayMode;

    public enum GridSnapMode { None, Fine, Gross }
    public GridSnapMode snapMode = GridSnapMode.None;
    //public GridSnapMode SnapMode { get; set; }

    public GameObject GUIEdit;
    public GameObject GUIPlay;

    public void SetSnapMode(int mode)
    {
        snapMode = (GridSnapMode)mode;
    }


    public float SnapIncrement
    {
        get
        {
            switch (snapMode)
            {
                case GridSnapMode.Gross:
                    return 0.25f;
                case GridSnapMode.Fine:
                    return 0.125f;
                default:
                    return 0;
            }
        }
    }


    #region SINGLETON
    [System.NonSerialized]
    public static GameEngine instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }
    #endregion

    void Start()
    {
        Application.targetFrameRate = 30;
        LightRay.Rays = Rays;
        LightRay.DepthMax = DepthMax;

        StartCoroutine("FillRaysReserve");
    }

    IEnumerator FillRaysReserve()
    {
        for (int i = 0; i < NRaysMax; i++)
        {
            int k = i;
            for (; i < k + 10 && i < NRaysMax; i++)
                LightRay.InstantiateLightRay();

            yield return new WaitForFixedUpdate();
        }
    }

    public bool isInEditMode { get => PlayMode == Mode.Edit; }
    public bool isInInactiveMode { get => PlayMode == Mode.Inactive; }
    public bool LevelCompleted;

    public void StartGameEngine(Mode playmod)
    {
        if (playmod != Mode.Inactive)
            UpdateComponentList();
        PlayMode = playmod;
        LevelCompleted = false;
    }

    public void StartGameEngine()
    {
        if (PlayMode != Mode.Inactive)
            UpdateComponentList();

        LevelCompleted = false;
    }

    public void StopGameEngine()
    {
        ResetLightRay();
        Array.Clear(LightSources, 0, LightSources.Length);
        Array.Clear(OpticalComponents, 0, OpticalComponents.Length);
        Array.Clear(Targets, 0, Targets.Length);
        PlayMode = Mode.Inactive;
    }

    void LateUpdate()
    {
        if (PlayMode == Mode.Inactive) return;

        bool update = false;

        if (LightRay.misere && LightRay.NewRaysAvailable)
        {
            LightRay.misere = false;
            UpdateAllRays();
            return;
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            op.UpdateCoordinates();
        }

        foreach (OpticalComponent op in OpticalComponents)
        {
            if (op.hasChanged)
            {
                if (op is LightSource ls) // C'est une source on update les rayons emit (plus la source en tant que op ensuite) 
                    UpdateLightRays1LS(ls);

                UpdateLightRays1OP(op);
                op.hasChanged = false;
                update = true;
            }
        }

        if (Targets.Length > 0)
        {
            bool completed = true;
            foreach (Target tg in Targets)
            {
                tg.UpdateCoordinates();
                if (update || tg.hasChanged)
                {
                    tg.ComputeScore();
                    tg.hasChanged = false;
                }

                completed &= tg.success;
            }

            if (completed && !LevelCompleted && !ChessPiece.Manipulated)
            {
                LevelCompleted = true;
                LevelManager.instance.ShowsScoreBoard();
            }
        }


        /*if (update)
        {
            foreach (Target tg in Targets)
            {
                tg.ComputeScore();
                tg.hasChanged = false;
            }
        }*/
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

        lr.DrawMesh(); // Met à jour le mesh centrer sur la camera
        return false;
    }

    public void ResetLightRay()
    {
        foreach (LightSource lightSource in GameEngine.instance.LightSources)
            if (lightSource != null)
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

    void OnDestroy()
    {
        instance = null;
    }

}
