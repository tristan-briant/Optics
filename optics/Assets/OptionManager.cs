using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public GameObject Target;

    public Color EnableColor;
    public Color DisableColor;

    public GameObject RotationSwitch;
    public GameObject TranslationSwitch;

    void Start()
    {
        Transform itemHandle = transform.Find("ItemHolder");
        if (itemHandle)
            foreach (Canvas c in itemHandle.GetComponentsInChildren<Canvas>())
                c.sortingLayerName = "Handle";

        transform.position = Target.transform.position;
        transform.rotation = Quaternion.identity;

        /*if(Target.GetComponent<DragAndDrop>().)
        RotationSwitch
*/
    }

    void Update()
    {
        transform.position = Target.transform.position;
        transform.rotation = Quaternion.identity;
    }


}
