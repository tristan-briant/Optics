using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatParameterModifier : MonoBehaviour
{

    public string ParameterName; // Name of the property
    public float increment = 0.1f;
    public float Max = 100;
    public float Min = -100;
    public Text ValueText;
    public Slider slider;
    object ob;
    string format;

    private void Start()
    {
        ob = GetComponentInParent<OptionManager>().CP.GetComponent<GenericComponent>();

        slider = GetComponent<Slider>();

        if (increment < 0.1f) format = "F2";
        else format = "F1";

        if (ValueText)
            ValueText.text = ((float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null)).ToString(format);
        if (slider)
        {
            if (ob.GetType().GetProperty(ParameterName + "Max") != null)
                slider.maxValue = ((float)ob.GetType().GetProperty(ParameterName + "Max").GetValue(ob, null));
            else
                slider.maxValue = Max;
            if (ob.GetType().GetProperty(ParameterName + "Min") != null)
                slider.minValue = ((float)ob.GetType().GetProperty(ParameterName + "Min").GetValue(ob, null));
            else
                slider.minValue = Min;
            slider.value = ((float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));

            slider.onValueChanged.AddListener(SliderValueChanged);

        }
    }


    public void SliderValueChanged(float value)
    {
        ob.GetType().GetProperty(ParameterName).SetValue(ob, value, null);
        slider.value = ((float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
    }

    public void IncrementValue(float multiplier)
    {

        float a = (float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null);
        a += multiplier * increment;
        a = Mathf.Clamp(a, Min, Max);

        ob.GetType().GetProperty(ParameterName).SetValue(ob, a, null);

        if (ValueText)
            ValueText.text = ((float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null)).ToString(format);
        if (slider)
            slider.value = ((float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null));
    }
}
