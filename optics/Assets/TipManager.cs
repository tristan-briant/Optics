using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    public float lifeTime = Mathf.Infinity;

    void Start()
    {
        Destroy(gameObject,lifeTime);
    }
}
