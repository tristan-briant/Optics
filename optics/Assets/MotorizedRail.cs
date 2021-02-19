using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorizedRail : RectWall
{


    public float MotionPeriod = 4f;
    public bool animate = true;

    GameObject ObjectToMove;
    float phase = 0;

    override public void Start()
    {
        base.Start();

        Transform component = transform.Find("Component");
        if (component.childCount > 0)
            ObjectToMove = component.GetChild(0).gameObject;
    }


    void Update()
    {
        if (animate)
            phase += 2f * Mathf.PI * Time.deltaTime / MotionPeriod;

        if (ObjectToMove)
        {
            ObjectToMove.transform.localPosition = Mathf.Sin(phase) * new Vector3(0f, Height / 2f - 0.5f, 0f);
        }
    }

    public override void Delete()
    {
        Transform component = transform.Find("Component");
        GameObject PGComponents = GameObject.Find("Playground/Components");

        while (component.childCount > 0)
            //component.GetChild(0).GetComponent<GenericComponent>().Delete();
            component.GetChild(0).SetParent(PGComponents.transform);

        base.Delete();
    }

    public override string ToJson()
    {
        string str;

        str = base.ToJson();
        if (ObjectToMove)
        {
            GenericComponent genericComponent = ObjectToMove.GetComponent<GenericComponent>();
            if (genericComponent)
                str += "+" + genericComponent.ToJson();
        }
        return str;
    }

    override public void FromJson(string str)
    {

        string[] tokens = str.Split('+');

        base.FromJson(tokens[0]); 

        if (tokens.Length > 1)
        {
            GameObject component = Designer.InstantiateComponent(tokens[1]);

            GenericComponent gc = component.GetComponent<GenericComponent>();
            gc.FromJson(tokens[1]);

            component.transform.SetParent(transform.Find("Component"));
            ObjectToMove = component;
        }
    }

}
