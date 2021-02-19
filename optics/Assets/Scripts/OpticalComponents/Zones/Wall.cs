using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : OpticalComponent
{

    override public float RadiusMax { get => Mathf.Infinity; }
    override public float RadiusMin { get => -Mathf.Infinity; }


    override public void Deflect(LightRay r)
    {

        r.ClearChildren();  // nothing goes throw

        float xo1 = r.StartPosition1.x;
        float yo1 = r.StartPosition1.y;
        float ao1 = r.Direction1;
        float xo2 = r.StartPosition2.x;
        float yo2 = r.StartPosition2.y;
        float ao2 = r.Direction2;

        r.Length1 = (xc1 - xo1) * r.cos1 + (yc1 - yo1) * r.sin1;
        r.Length2 = (xc2 - xo2) * r.cos2 + (yc2 - yo2) * r.sin2;

    }

    override public void ChangeVisual()
    {
        // Nothing to do 
    }

    override public float Collision2(LightRay lr)
    {
        float l1 = FlatCollision(lr, 1);
        xc1 = xc; yc1 = yc;
        //if (l1 < 0) return -1;
        float l2 = FlatCollision(lr, 2);
        xc2 = xc; yc2 = yc;
        if (l2 >= 0)
            return l2;
        if (l1 > 0)
            return l1;

        return -1;
    }


}
