using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectZone : GenericComponent
{
    const float offsetColision = 0.015f; // Use in physics 2D to determine collision

    public float height;
    public float width;

    public float Width { get => width; set { width = Mathf.Clamp(value, WidthMin, WidthMax); hasChanged = true; ChangeVisual(); } }
    public float Height { get => height; set { height = Mathf.Clamp(value, HeightMin, HeightMax); hasChanged = true; ChangeVisual(); } }
    virtual public float WidthMin { get => 0.25f; }
    virtual public float HeightMin { get => 0.25f; }
    virtual public float WidthMax { get => Mathf.Infinity; }
    virtual public float HeightMax { get => Mathf.Infinity; }


    override public void Start()
    {
        width = ((RectTransform)transform).rect.width;
        height = ((RectTransform)transform).rect.height;

        ChangeVisual();

        //GetComponent<ChessPiece>().LetFindPlace();
    }

    public override void ChangeVisual()
    {
        RectTransform rt = (RectTransform)transform;
        rt.sizeDelta = new Vector2(width, height);

        Transform element = transform.Find("Optics");
        if (element)
            foreach (BoxCollider2D bc in element.GetComponentsInChildren<BoxCollider2D>())
                bc.size = new Vector2(width - offsetColision, height - offsetColision);

        element = transform.Find("Base");
        if (element)
            foreach (BoxCollider2D bc in element.GetComponentsInChildren<BoxCollider2D>())
                bc.size = new Vector2(width - offsetColision, height - offsetColision);

    }

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        RectTransform rt = (RectTransform)transform;
        rt.sizeDelta = new Vector2(width, height);

        ChangeVisual();
    }

}
