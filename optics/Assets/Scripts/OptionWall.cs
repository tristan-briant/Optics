using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionWall : OptionManager
{
    float targetHeight, targetWidth;
    Vector2 InitialPos, InitialSize, TargetSize;
    int collisionCount;
    RaycastHit2D[] results = new RaycastHit2D[100];
    ContactPoint2D[] contacts = new ContactPoint2D[100];
    Collider2D[] collider2Ds = new Collider2D[100];

    public override void Start()
    {
        base.Start();

        Transform handle = transform.Find("HandleHolder");
        if (handle)
            foreach (Canvas c in handle.GetComponentsInChildren<Canvas>())
                c.sortingLayerName = "Handle";

        handle.rotation = CP.transform.rotation;

        ResetPosition();
    }

    public void ResetPosition()
    {
        InitialPos = CP.GetComponent<ChessPiece>().transform.position;
        targetHeight = CP.GetComponent<RectWall>().Height;
        targetWidth = CP.GetComponent<RectWall>().Width;
        InitialSize.x = CP.GetComponent<RectWall>().Width;
        InitialSize.y = CP.GetComponent<RectWall>().Height;
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

    public void ChangeSize(Vector2 increment, bool left, bool right, bool up, bool down)
    {
        Vector2 DeltaTargetPos;

        targetWidth += increment.x;
        targetHeight += increment.y;



        Vector2 dir = Vector2.zero;
        float distance = 0f;



        if (up) { dir.y = 1; distance = targetHeight - CP.GetComponent<RectWall>().Height; }
        if (down) { dir.y = -1; distance = targetHeight - CP.GetComponent<RectWall>().Height; }
        if (left) { dir.x = -1; distance = targetWidth - CP.GetComponent<RectWall>().Width; }
        if (right) { dir.x = 1; distance = targetWidth - CP.GetComponent<RectWall>().Width; }

        distance = MyMathf.Round(distance, 0.25f);

        CP.Constrain(true, false); // Need a rigidbody to calculate colisions


        bool IncreaseSize = targetWidth > CP.GetComponent<RectWall>().Width || targetHeight > CP.GetComponent<RectWall>().Height;


        // Compute on maximum distance we can inscrease
        if (IncreaseSize)
            for (int i = 1; i <= distance * 4.0f; i++)
                foreach (Collider2D collider in CP.GetComponentsInChildren<Collider2D>())
                    if (!collider.isTrigger)
                        if (collider.Cast(dir, results, i * 0.25f - 0.02f) > 0)
                        {
                            distance = (i - 1) * 0.25f;
                            break;
                        }

        if (!IncreaseSize || distance > 0)
        {
            if (left || right)
                CP.GetComponent<RectWall>().Width = Mathf.Min(MyMathf.Round(targetWidth, 0.25f), CP.GetComponent<RectWall>().Width + distance);
            else
                CP.GetComponent<RectWall>().Height = Mathf.Min(MyMathf.Round(targetHeight, 0.25f), CP.GetComponent<RectWall>().Height + distance);

            if (right)
                DeltaTargetPos.x = (CP.GetComponent<RectWall>().Width - InitialSize.x) / 2;
            else
                DeltaTargetPos.x = (InitialSize.x - CP.GetComponent<RectWall>().Width) / 2;

            if (up)
                DeltaTargetPos.y = (CP.GetComponent<RectWall>().Height - InitialSize.y) / 2;
            else
                DeltaTargetPos.y = (InitialSize.y - CP.GetComponent<RectWall>().Height) / 2;

            CP.GetComponent<ChessPiece>().TeleportTo(InitialPos + DeltaTargetPos);

        }


        foreach (DimensionHandle dm in GetComponentsInChildren<DimensionHandle>())
            dm.ResetPosition();

        CP.Constrain(false, false);

    }

}
