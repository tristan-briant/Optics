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

    Vector3 Offset;
    void OnMouseDown()
    {
        distance =  Vector3.Distance(transform.position, Camera.main.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        Offset = transform.position - rayPoint;
   
        dragging = true;

        transform.GetComponent<Rigidbody2D>().mass = transform.GetComponent<Rigidbody2D>().mass / 10;
    }
    
    void OnMouseUp()
    {
        dragging = false;
        transform.GetComponent<Rigidbody2D>().mass = transform.GetComponent<Rigidbody2D>().mass * 10;
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

            Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();

            if (rb)
            {
                Vector2 f=Vector2.zero;
                f.x =  rayPoint.x + Offset.x - transform.position.x;
                f.y =  rayPoint.y + Offset.y - transform.position.y;


                transform.GetComponent<Rigidbody2D>().AddForce(f);

            }
            else
            {
                float z = transform.localPosition.z; // Conserve le z de l'objet

                transform.position = rayPoint + Offset;

                Vector3 locPos = transform.localPosition;

                

                locPos.z = z;
                transform.localPosition = locPos;
            }
        }
    }
}
