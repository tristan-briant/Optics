using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionWall : OptionManager
{
    public GameObject HandleLeft, HandleRight;

    float targetHeight, targetWidth;
    Vector2 targetPos;

    public override void Start()
    {
        base.Start();

        Transform handle = transform.Find("HandleHolder");
        if (handle)
            foreach (Canvas c in handle.GetComponentsInChildren<Canvas>())
                c.sortingLayerName = "Handle";

        handle.rotation = CP.transform.rotation;

        targetHeight = CP.GetComponent<RectWall>().Height;
        targetWidth = CP.GetComponent<RectWall>().Width;
        targetPos = CP.GetComponent<ChessPiece>().positionSet;
    }

    public Vector3 GetHandleLocalPosition(bool left, bool right, bool up, bool down)
    {
        float x = 0, y = 0;

        if (left) x = -CP.GetComponent<RectWall>().width / 2;
        if (right) x = CP.GetComponent<RectWall>().width / 2;
        if (up) y = CP.GetComponent<RectWall>().height / 2;
        if (down) y = -CP.GetComponent<RectWall>().height / 2;

        return new Vector3(x, y, 0);
    }



    public void ConstrainTarget(bool translation, bool rotation)
    {
        CP.GetComponent<ChessPiece>().Constrain(true, false);
        if (translation || rotation)
            ChessPiece.Manipulated = CP;
        else ChessPiece.Manipulated = null;
    }

    public void ChangeSize(Vector2 increment, bool right, bool up)
    {

        /*if (right)
            targetPos.x += increment.x/2 ;
        else
            targetPos.x -= increment.x /2;

        if (up)
            targetPos.y += increment.y /2;
        else
            targetPos.y -= increment.y /2;*/



        targetWidth = Mathf.Max(targetWidth + increment.x*2, CP.GetComponent<RectWall>().WidthMin);
        targetHeight = Mathf.Max(targetHeight + increment.y*2, CP.GetComponent<RectWall>().HeightMin);

        Debug.Log("change");
        CP.GetComponent<RectWall>().Width = MyMathf.Round(targetWidth, 0.25f);
        CP.GetComponent<RectWall>().Height = MyMathf.Round(targetHeight, 0.25f);

        //CP.GetComponent<ChessPiece>().positionSet = MyMathf.Round(targetPos, 0.125f);

        foreach (DimensionHandle dm in GetComponentsInChildren<DimensionHandle>())
            dm.ResetPosition();
    }

}
