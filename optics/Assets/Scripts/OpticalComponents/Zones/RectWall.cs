using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectWall : RectZone
{

    public override void ChangeVisual()
    {
        Transform W;

        base.ChangeVisual();

        W = transform.Find("Diag1");
        if (W)
        {
            W.localPosition = new Vector3(0, 0, 0);
            W.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(height, width) * Mathf.Rad2Deg);
            W.GetComponent<OpticalComponent>().Radius = Mathf.Sqrt(height * height + width * width) / 2;
        }

        W = transform.Find("Diag2");
        if (W)
        {
            W.localPosition = new Vector3(0, 0, 0);
            W.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(height, width) * Mathf.Rad2Deg);
            W.GetComponent<OpticalComponent>().Radius = Mathf.Sqrt(height * height + width * width) / 2;
        }
    }


}
