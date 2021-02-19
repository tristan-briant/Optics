using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tools : MonoBehaviour
{
    [MenuItem("MyTools/CreateGameObjects")]
    static void Create()
    {
        for (int x = 0; x != 10; x++)
        {
            GameObject go = new GameObject("MyCreatedGO");
            go.transform.position = new Vector3(x, 0, 0);
        }
    }

    [MenuItem("MyTools/SaveToString")]
    static void SaveToString()
    {
        Designer.SaveToString();
    }

    [MenuItem("MyTools/LoadFromString")]
    static void LoadFromString()
    {
        Designer.LoadFromString(true);
    }

    [MenuItem("MyTools/Add Mirror")]
    static void AddMirror()
    {
        //Designer.AddMirror();
    }

}

