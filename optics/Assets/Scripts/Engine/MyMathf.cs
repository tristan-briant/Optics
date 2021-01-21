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
            return (x - xmin) / (xmax - xmin) * 0.999f;
        else
            return 0;

    }

}
