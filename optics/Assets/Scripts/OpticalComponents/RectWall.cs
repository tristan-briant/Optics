using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectWall : GenericComponent
{

    const float offsetColision = 0.015f; // Use in physics 2D to determine collision

    public float height;
    public float width;

    public float Width { get => width; set { width = Mathf.Clamp(value, WidthMin, Mathf.Infinity); hasChanged = true; ChangeVisual(); } }
    public float Height { get => height; set { height = Mathf.Clamp(value, HeightMin, Mathf.Infinity); hasChanged = true; ChangeVisual(); } }
    public float WidthMin { get => 0.25f; }
    public float HeightMin { get => 0.25f; }


    override public void Start()
    {
        
        GetComponent<ChessPiece>().LetFindPlace();
        GetComponent<Rigidbody2D>().drag = 0f;

        width = ((RectTransform)transform).rect.width;
        height = ((RectTransform)transform).rect.height;

        ChangeVisual();

    }

    public override void ChangeVisual()
    {
        Transform W;

        RectTransform rt = (RectTransform)transform;
        rt.sizeDelta = new Vector2(width, height);

        W = transform.Find("Diag1");
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
            bc.size = new Vector2(width - offsetColision, height - offsetColision);
        foreach (BoxCollider2D bc in transform.Find("Base").GetComponentsInChildren<BoxCollider2D>())
            bc.size = new Vector2(width - offsetColision, height - offsetColision);

        //GetComponent<ChessPiece>().LetFindPlace();
    }

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        RectTransform rt = (RectTransform)transform;
        rt.sizeDelta = new Vector2(width, height);

        ChangeVisual();
    }

}
