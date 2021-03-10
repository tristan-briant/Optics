using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public GameObject Button;
    public PathCurve Path;

    void Start()
    {
        LevelManager levelManager;
        levelManager = LevelManager.instance;

        int levelNumber = levelManager.Levels.Count;

        for (int i = 0; i < levelNumber; i++)
        {

            GameObject buttonInstance = Instantiate(Button) as GameObject;
            float t =  ((float)i / (levelNumber - 1)) ;
            Vector3 pos = new Vector3(Path.curveX.Evaluate(t), Path.curveY.Evaluate(t), 0);

            buttonInstance.transform.SetParent(Path.transform.parent.parent);
            buttonInstance.transform.position = pos;
            buttonInstance.transform.localScale = Vector3.one ;

            buttonInstance.GetComponent<LevelButton>().LevelToBeloaded = i;

        }

    }


}
