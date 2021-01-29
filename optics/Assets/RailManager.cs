using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : GenericComponent
{
    public List<GenericComponent> AttachedComponents = new List<GenericComponent>();
    public bool attachMode = false;


    public float length = 0.5f;
    public float Length { get => length; set => length = value; }


    


    public void FixedUpdate()
    {
        if (GetComponent<ChessPiece>().moving)
        {
            Vector3 delta = GetComponent<ChessPiece>().PositionSet - transform.position;

            foreach (GenericComponent op in AttachedComponents)
            {
                op.GetComponent<ChessPiece>().PositionSet += delta;
                op.GetComponent<ChessPiece>().Constrain(true,false);
                //op.GetComponent<ChessPiece>().LetFindPlace();
            }
        }

        foreach (GenericComponent op in AttachedComponents)
        {
            LockComponent(op, attachMode);
            //op.GetComponent<ChessPiece>().LetFindPlace();
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (attachMode)
        {
            GenericComponent op = collision.gameObject.GetComponent<GenericComponent>();
            if (op)
            {
                Vector3 pos = ClampPosition(op.transform.position);

                if ((op.transform.position - pos).magnitude < .9f)
                {
                    AttachComponent(op);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("enter");
        if (attachMode)
        {
            GenericComponent op = collision.gameObject.GetComponent<GenericComponent>();
            if (op)
            {
                Vector3 pos = ClampPosition(op.transform.position);

                if ((op.transform.position - pos).magnitude > 1.0f)
                {
                    DetachComponent(op);
                }
            }

        }
    }


    Vector3 ClampPosition(Vector3 pos)
    {
        float l = (pos.x - GetComponent<ChessPiece>().PositionSet.x) * cos + (pos.y - GetComponent<ChessPiece>().PositionSet.y) * sin;
        if (l > Length / 2) l = Length / 2;
        if (l < -Length / 2) l = -Length / 2;

        pos.x = GetComponent<ChessPiece>().PositionSet.x + l * cos;
        pos.y = GetComponent<ChessPiece>().PositionSet.y + l * sin;

        return pos;
    }

    void LockComponent(GenericComponent cp, bool soft = false)
    {
        Vector3 pos = cp.GetComponent<ChessPiece>().PositionSet;


        float l = (pos.x - x) * cos + (pos.y - y) * sin;
        if (l > Length / 2) l = Length / 2;
        if (l < -Length / 2) l = -Length / 2;

        pos.x = x + l * cos;
        pos.y = y + l * sin;


        //Rigidbody2D rb = cp.GetComponent<Rigidbody2D>();
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (soft && cp.GetComponent<ChessPiece>().moving)
        {
            const float alpha = 0.5f;
            //cp.transform.position = alpha * cp.transform.position + (1 - alpha) * pos;
            //cp.GetComponent<Rigidbody2D>().MovePosition(new Vector2(pos.x,pos.y));
            cp.GetComponent<ChessPiece>().PositionSet = alpha * cp.GetComponent<ChessPiece>().PositionSet + (1 - alpha) * pos;
        }
        else
        {
            //cp.GetComponent<Rigidbody2D>().position= new Vector2(pos.x,pos.y);
            //cp.transform.position = pos;
            cp.GetComponent<ChessPiece>().PositionSet = pos;
        }
    }

    void AttachComponent(GenericComponent op)
    {
        if (!AttachedComponents.Contains(op))
        {
            AttachedComponents.Add(op);

            //FixedJoint2D joint = op.gameObject.AddComponent<FixedJoint2D>();
            //joint.connectedBody = GetComponent<Rigidbody2D>();
            /*foreach (Collider2D cd1 in GetComponentsInChildren<Collider2D>())
                foreach (Collider2D cd2 in op.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(cd1, cd2);*/
        }
        /*else
        {
            AttachedComponents.Remove(op);

            foreach (Collider2D cd1 in GetComponentsInChildren<Collider2D>())
                foreach (Collider2D cd2 in op.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(cd1, cd2, false);

        }*/
    }

    void DetachComponent(GenericComponent op)
    {
        AttachedComponents.Remove(op);

        /*foreach (Collider2D cd1 in GetComponentsInChildren<Collider2D>())
            foreach (Collider2D cd2 in op.GetComponentsInChildren<Collider2D>())
                Physics2D.IgnoreCollision(cd1, cd2, false);*/
    }

}
