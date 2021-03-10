using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpticalComponent : GenericComponent
{
    protected Transform Optics;
    protected ChessPiece CP;

    public float radius = 0.5f;
    public float Radius { get => radius; set { radius = Mathf.Clamp(value, RadiusMin, RadiusMax); hasChanged = true; ChangeVisual(); } }
    virtual public float RadiusMax { get => 1.5f; }
    virtual public float RadiusMin { get => 0.2f; }

    override public void Start()
    {
        Optics = transform.Find("Optics"); // S'il y a une partie optique
        if (Optics == null)
            Optics = this.transform;

        CP = GetComponent<ChessPiece>();

        base.Start();
    }

    protected Vector3 OldPosition;
    protected Quaternion OldRotation;
    override public void UpdateCoordinates()
    {
        if (Optics == null) return;

        if (OldPosition != Optics.position || Optics.rotation != OldRotation)
        {
            ComputeDir();
            OldPosition = Optics.position;
            OldRotation = Optics.rotation;
            hasChanged = true;
        }
    }

    override public void ComputeDir()
    {
        Vector3 pos = transform.position;
        x = pos.x;
        y = pos.y;

        angle = Optics.rotation.eulerAngles.z * Mathf.Deg2Rad;
        cos = Mathf.Cos(angle);
        sin = Mathf.Sin(angle);
        param = -sin * x + cos * y;
    }

    virtual public bool FastCollision(LightRay lr)
    {
        float p = -lr.sin1 * x + lr.cos1 * y;
        if (p > lr.param1 + radius || p < lr.param1 - radius)
            return false;

        p = lr.cos1 * (x - lr.StartPosition1.x) + lr.sin1 * (y - lr.StartPosition1.y);
        if (p < -radius || p > lr.Length1 + radius)
            return false;

        return true;
    }

    protected float xc, yc;
    public float FlatCollision(LightRay lr, int i) //Collision with linear object
    {
        float cosr, sinr, xr, yr, br;
        if (i == 1)
        {
            if (FastCollision(lr) == false) return -1;  // Pas de collision
            cosr = lr.cos1;
            sinr = lr.sin1;
            xr = lr.StartPosition1.x;
            yr = lr.StartPosition1.y;
            br = lr.param1;
        }
        else
        {
            cosr = lr.cos2;
            sinr = lr.sin2;
            xr = lr.StartPosition2.x;
            yr = lr.StartPosition2.y;
            br = lr.param2;
        }

        float b = param;

        float det = -cosr * sin + sinr * cos;

        if (det == 0) return -1;


        xc = (cosr * b - cos * br) / det;
        yc = (sinr * b - sin * br) / det;


        if ((cosr > 0 && xc > xr) || (cosr < 0 && xc < xr) || (sinr > 0 && yc > yr) || (sinr < 0 && yc < yr))
        {
            float r2 = (xc - x) * (xc - x) + (yc - y) * (yc - y);
            if (r2 < radius * radius)
                return (xc - xr) * (xc - xr) + (yc - yr) * (yc - yr);
        }
        return -1;
    }

    public float CircularCollision(LightRay lr, int i) //Collision with circular object
    {
        float cosr, sinr, xr, yr, br;
        if (i == 1)
        {
            if (FastCollision(lr) == false) return -1;  // Pas de collision
            cosr = lr.cos1;
            sinr = lr.sin1;
            xr = lr.StartPosition1.x;
            yr = lr.StartPosition1.y;
            br = lr.param1;
        }
        else
        {
            cosr = lr.cos2;
            sinr = lr.sin2;
            xr = lr.StartPosition2.x;
            yr = lr.StartPosition2.y;
            br = lr.param2;
        }

        float p = cosr * (x - xr) + sinr * (y - yr);
        return p * p;
    }

    protected float xc1, yc1, xc2, yc2;

    virtual public float Collision2(LightRay lr)
    {
        float l1 = FlatCollision(lr, 1);
        xc1 = xc; yc1 = yc;
        if (l1 < 0) return -1;
        float l2 = FlatCollision(lr, 2);
        xc2 = xc; yc2 = yc;
        if (l2 < 0) return -1;

        return l1;
    }

    public virtual void Deflect(LightRay r) { }

    [ContextMenu("ChangeVisual")]
    override public void ChangeVisual()
    {
        base.ChangeVisual();
        Animator anim = GetComponent<Animator>();

        if (anim)
        {
            anim.SetFloat("Size", MyMathf.MapToFrame(Radius, RadiusMin, RadiusMax));

            if (CP)
                CP.LetFindPlace();
        }

    }

    override public void ClampParameters() // Used to clamp all params between min and max, used in editor mode
    {
        radius = Mathf.Clamp(radius, RadiusMin, RadiusMax);
    }


}
