using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrasManager : MonoBehaviour
{

    public void SetSize(Vector2 size)
    {
        ((RectTransform)transform).sizeDelta = size;
    }


    void Update()
    {
        Vector3 CamPos, GrassPos;
        CamPos = Camera.main.transform.position;

        GrassPos = CamPos / 2; // To give a perspective effect
        GrassPos.z = 2;

        transform.position = GrassPos;
    }
}
