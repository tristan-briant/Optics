using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //SetupGround();
    }

    // Update is called once per frame
    public void SetupGround()
    {
        GetComponent<BoxCollider2D>().size = ((RectTransform)transform).rect.size;

        foreach (BoxCollider2D wall in transform.Find("Base").GetComponentsInChildren<BoxCollider2D>())
            wall.size = ((RectTransform)wall.transform).rect.size;
    }
}
