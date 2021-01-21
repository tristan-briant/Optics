using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotation : MonoBehaviour
{
    void Start()
    {
        transform.rotation=Quaternion.identity;
    }

}
