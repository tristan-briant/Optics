using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{


    public GameObject Base;
    public GameObject Optics;

    void Start()
    {
        Vector2 size;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        size.x = rectTransform.rect.width;
        size.y = rectTransform.rect.height;

        if (Base)
        {
            BoxCollider2D bc = GetComponentInChildren<BoxCollider2D>();
            if (bc == null)
                bc = Base.AddComponent<BoxCollider2D>();
            GetComponentInChildren<BoxCollider2D>().size = size;
        }

        if (Optics)
        {
            BoxCollider2D bc = GetComponentInChildren<BoxCollider2D>();
            if (bc == null)
                bc = Optics.AddComponent<BoxCollider2D>();
            GetComponentInChildren<BoxCollider2D>().size = size;
        }



    }
}
