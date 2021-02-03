using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolParameterModifier : MonoBehaviour
{

    public string ParameterName; // Name of the property
    public Toggle toggle;
    //public Image checkMark;

    object ob;

    private void Start()
    {
        ob = GetComponentInParent<OptionManager>().CP.GetComponent<GenericComponent>();
        toggle.isOn = ((bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
        //checkMark.enabled = ((bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
    }

    public void Toogle()
    {
        //bool a = (bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null);
        bool a = toggle.isOn;
        ob.GetType().GetProperty(ParameterName).SetValue(ob, a, null);
        //checkMark.enabled = ((bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
        toggle.isOn = ((bool)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
    }

}
