using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMathf
{
    static public float MapTo(float x, float xmin, float xmax, float ymin, float ymax)
    {
        if (xmin != xmax)
            return ymin + (x - xmin) / (xmax - xmin) * (ymax - ymin);
        else
            return 0;

    }

    static public float MapToFrame(float x, float xmin, float xmax)
    /// map float to 0 -> 0.999f for frame
    {
        if (xmin != xmax)
            return Mathf.Clamp((x - xmin) / (xmax - xmin), 0, 0.99f);
        else
            return 0;

    }

    static public float Round(float x, float increment)
    {
        if (increment == 0)
            return x;
        else
            return Mathf.Round(x / increment) * increment;
    }

    static public Vector2 Round(Vector2 v, float increment)
    {
        v.x = Round(v.x, increment);
        v.y = Round(v.y, increment);
        return v;
    }

    static public Vector3 Round(Vector3 v, float increment)
    {
        v.x = Round(v.x, increment);
        v.y = Round(v.y, increment);
        v.z = Round(v.z, increment);
        return v;
    }

    public static Vector2 rotate(Vector2 v, float angleDeg)
    {
        float angle = angleDeg * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
            v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle)
        );
    }



}
