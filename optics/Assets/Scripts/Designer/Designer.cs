using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;
//using SimpleFileBrowser;

public class Designer : MonoBehaviour
{

    const int MaxSize = 8;
    const int MinSize = 3;
    static public Vector2 PGSize;
    static public string PGdata;

    [System.NonSerialized]
    public Designer instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }


    static public byte[] MakeThumbBytes(int size = 32) // 16 pixel per unit
    {
        //prepare l'image
        GameObject Pg = GameObject.Find("Playground");
        RectTransform rect = Pg.transform as RectTransform;
        int w = (int)(rect.rect.width * size);
        int h = (int)(rect.rect.height * size);

        // Prepare la texture et la camera
        RenderTexture rt = new RenderTexture(w, h, 24);
        Camera cam = GameObject.Find("CameraThumbNail").GetComponent<Camera>();
        cam.targetTexture = rt;
        cam.orthographicSize = 0.5f * rect.rect.height;
        cam.transform.position = new Vector3(0, 0, -10);
        cam.Render();

        Texture2D texture = new Texture2D(w, h, TextureFormat.RGB24, false);

        RenderTexture.active = rt;//cam.targetTexture;
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

        texture.Apply();

        return texture.EncodeToPNG();
    }

    static public void MakeThumb(string filename) => File.WriteAllBytes(filename, MakeThumbBytes());

    static public void MakeThumbNail()
    {
        Scene scene = SceneManager.GetActiveScene();
        MakeThumb("Assets/Resources/Levels/" + scene.name + ".png");
    }

    /*
            //[ContextMenu("SaveToFile")]
            public void SaveToFile()
            {
                string filename = "PlayGround" + System.DateTime.Now.ToShortDateString().Replace("/", "-") + "-"
                    + System.DateTime.Now.ToLongTimeString().Replace(":", "-");

                string file = Path.Combine(Application.persistentDataPath, filename + ".txt");

                SaveToString();

                File.WriteAllText(file, PGdata);

                MakeThumb(Path.Combine(Application.persistentDataPath, filename + ".png"), 1024);
            }

            */

    /*
        static public void SaveToPrefs()
        {
            LevelManager lvm = GameObject.Find("LevelManager").GetComponent<LevelManager>();

            if (lvm.currentLevel == 0 || lvm.designerScene)
            {
                if (LevelManager.designerMode)
                    SaveToString();

                PlayerPrefs.SetString("SandBox", PGdata);
            }
        }
    */
    /*
        static public void LoadFromPrefs()
        {
            if (PlayerPrefs.HasKey("SandBox"))
            {
                PGdata = PlayerPrefs.GetString("SandBox");
                LoadFromString();
            }
            else
            {
                PGdata = Resources.Load<TextAsset>("Levels/Default-Designer").ToString();
                LoadFromString();
            }
        }

        public void SaveToResources(string path)
        {
            path.Replace(".txt", "");

            SaveToString();
            Debug.Log("Saving " + path);
            StreamWriter sr = File.CreateText("Assets/Resources/Levels/" + path + ".txt");
            sr.Write(PGdata);
            sr.Close();
            sr.Dispose();

            MakeThumb("Assets/Resources/Levels/" + path + ".png");
        }
    */

    //[ContextMenu("SaveToString")]
    static public void SaveToString()
    {
        GameObject PG = GameObject.Find("Playground");

        PGdata = "";
        RectTransform rt = (RectTransform)PG.transform;
        PGdata += rt.rect.width + "\n";
        PGdata += rt.rect.height + "\n";

        GameObject PGComponents = GameObject.Find("Playground/Components");
        foreach (Transform child in PGComponents.transform)
        {
            GenericComponent component = child.GetComponent<GenericComponent>();
            if (component)
                PGdata += component.ToJson() + "\n";
        }

        Debug.Log(PGdata);
    }

    static public void ClearPlayground()
    {
        GameObject PG = GameObject.Find("Playground/Components");

        while (PG.transform.childCount > 0)
        {
            GenericComponent component = PG.transform.GetChild(0).GetComponent<GenericComponent>();
            component.Delete();
        }
    }

    //[ContextMenu("LoadFromString")]
    static public void LoadFromString(bool prefab = false)
    {
        if (PGdata == null) return;
        //PGdata = PGdata.Replace("\r", ""); //clean up string

        ClearPlayground();


        string[] tokens = PGdata.Split('\n');

        if (GameEngine.instance != null) // otherwise not in play mode
        {
            GameObject PGGround = GameObject.Find("Playground/ChessBoard");
            PGGround.GetComponent<PanZoom>().SetPlaygroundSize(float.Parse(tokens[0]), float.Parse(tokens[1]));
        }


        GameObject PGComponents = GameObject.Find("Playground/Components");

        for (int i = 2; i < tokens.Length; i++)
        {
            GameObject component = InstantiateComponent(tokens[i]);
            if (component == null) continue;

            GenericComponent gc = component.GetComponent<GenericComponent>();
            gc.FromJson(tokens[i]);
        }


        GameEngine.instance?.UpdateComponentList();
    }


    public static GameObject InstantiateComponent(string str)
    {
        String PrefabComponentPath = getBetween(str, "\"prefabPath\":\"", "\"");
        if (PrefabComponentPath == "")
            return null;

        GameObject resource = Resources.Load(PrefabComponentPath, typeof(GameObject)) as GameObject;

#if UNITY_EDITOR
        return PrefabUtility.InstantiatePrefab(resource) as GameObject;
#else
        return Instantiate(resource) as GameObject;
#endif
    }


    public static string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }


}