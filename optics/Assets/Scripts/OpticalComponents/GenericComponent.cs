using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericComponent : MonoBehaviour
{
    public float x = 0, y = 0; // position
    public float angle = 0;

    [System.NonSerialized]
    public float cos, sin, param; // vecteur directeur
    [System.NonSerialized]
    public bool hasChanged = true;


    Vector3 OldPosition;
    Quaternion OldRotation;

    void Start()
    {
        ChangeVisual();
    }

    virtual public void Update()
    {
        if (OldPosition != transform.position || transform.rotation != OldRotation)
        {
            ComputeDir();
            OldPosition = transform.position;
            OldRotation = transform.rotation;
            hasChanged = true;
        }
    }


    public void ComputeDir()
    {
        //Vector3 pos = PlayGround.InverseTransformPoint(transform.position); // Position relative par rapport au playground
        Vector3 pos = transform.position;
        x = pos.x;
        y = pos.y;

        angle = (transform.rotation.eulerAngles.z + 90) * Mathf.PI / 180f;
        cos = Mathf.Cos(angle);
        sin = Mathf.Sin(angle);
        param = -sin * x + cos * y;
    }

    public virtual void Delete() { }

    public virtual void ChangeVisual()
    {
        
    }

}
