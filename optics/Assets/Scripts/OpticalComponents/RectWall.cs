using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectWall : GenericComponent
{

    public float height;
    public float width;

    //public Transform Diag1, Diag2;
    const float EPSILON = 0.01f;

    public float Width { get => width; set { width = Mathf.Clamp(value, WidthMin, Mathf.Infinity); hasChanged = true; ChangeVisual(); } }
    public float Height { get => height; set { height = Mathf.Clamp(value, HeightMin, Mathf.Infinity); hasChanged = true; ChangeVisual(); } }
    public float WidthMin { get => 0.25f; }
    public float HeightMin { get => 0.25f; }


    void Start()
    {

        ChangeVisual();

    }

    public override void ChangeVisual()
    {
        RectTransform rt = (RectTransform)transform;
        Debug.Log(width);
        rt.sizeDelta = new Vector2(width, height);


        Transform W = transform.Find("Diag1");
        if (W)
        {
            W.localPosition = new Vector3(0, 0, 0);
            W.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(width, height) * 180 / Mathf.PI);
            W.GetComponent<OpticalComponent>().Radius = Mathf.Sqrt(height * height + width * width) / 2;

        }

        W = transform.Find("Diag2");
        if (W)
        {
            W.localPosition = new Vector3(0, 0, 0);
            W.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(width, height) * 180 / Mathf.PI);
            W.GetComponent<OpticalComponent>().Radius = Mathf.Sqrt(height * height + width * width) / 2;
        }

        foreach (BoxCollider2D bc in transform.Find("Optics").GetComponentsInChildren<BoxCollider2D>())
            bc.size = new Vector2(width, height);
        foreach (BoxCollider2D bc in transform.Find("Base").GetComponentsInChildren<BoxCollider2D>())
            bc.size = new Vector2(width, height);

        GetComponent<ChessPiece>().LetFindPlace();
    }

}
