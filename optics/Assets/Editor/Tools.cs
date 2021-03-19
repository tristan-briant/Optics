using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tools : MonoBehaviour
{
    /*[MenuItem("MyTools/CreateGameObjects")]
    static void Create()
    {
        for (int x = 0; x != 10; x++)
        {
            GameObject go = new GameObject("MyCreatedGO");
            go.transform.position = new Vector3(x, 0, 0);
        }
    }*/

    [MenuItem("LightMare/SaveToString")]
    static void SaveToString()
    {
        Designer.SaveToString();
    }

    [MenuItem("LightMare/LoadFromString")]
    static void LoadFromString()
    {
        Designer.LoadFromString(true);
    }

    [MenuItem("LightMare/UpdateComponentList")]
    static void UpdateComponentList()
    {
        GameEngine.instance?.UpdateComponentList();
    }

    [MenuItem("LightMare/MakeThumbNail")]
    static void MakeThumbNail()
    {
        Designer.MakeThumbNail();
    }

    [MenuItem("LightMare/SaveToPrefs")]
    static void SaveToPrefs()
    {
        Designer.SaveToPrefs();
    }

    [MenuItem("LightMare/LoadFromPrefs")]
    static void LoadFromPrefs()
    {
        Designer.LoadFromPrefs();
    }
}

