using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour {

    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    private bool dragging = false;
    private float distance;


    void OnMouseEnter()
    {
       // renderer.material.color = mouseOverColor;
    }

    void OnMouseExit()
    {
        //renderer.material.color = originalColor;
    }

    void OnMouseDown()
    {
        distance =  Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
    
    void OnMouseUp()
    {
        dragging = false;
    }

    void OnMouseDrag(){
       /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        //rayPoint.z = 0;//transform.position.z;
        transform.position = rayPoint;
        //Debug.Log("drag!!");*/
    }

    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);

            float z = transform.localPosition.z; // Conserve le z de l'objet

            transform.position = rayPoint;

            Vector3 locPos = transform.localPosition;
            locPos.z = z;
            transform.localPosition = locPos;
        }
    }
}
