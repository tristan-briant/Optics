using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]

public class ExecInEdit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Initialize");
    }

    // Update is called once per frame
    void Update()
    {

        if (!Application.isPlaying)
        {
            Debug.Log("Updated");
            GetComponent<GenericComponent>().ClampParameters();
            GetComponent<GenericComponent>().ChangeVisual();
        }
    }
}
