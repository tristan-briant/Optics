using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
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

    /*
        static public byte[] MakeThumbBytes(int size = 256)
        {
            //prepare l'image
            GameObject Pg = GameObject.Find("Playground");
            GameObject canvas = GameObject.Find("CanvasThumbnail");
            Pg.transform.SetParent(canvas.transform);
            GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gc.ResizePlayGround();

            // Prepare la texture
            RenderTexture rt = new RenderTexture(size, size, 24);
            Camera cam = GameObject.Find("CameraThumbnail").GetComponent<Camera>();
            cam.targetTexture = rt;
            cam.Render();


            Texture2D texture = new Texture2D(size, size, TextureFormat.RGB24, false);

            RenderTexture.active = rt;//cam.targetTexture;
            texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

            texture.Apply();

            //Remet tout en ordre
            canvas = GameObject.Find("PlaygroundHolder");
            Pg.transform.SetParent(canvas.transform);
            gc.ResizePlayGround();


            return texture.EncodeToPNG();
        }

        public void MakeThumb(string filename, int size = 256) => File.WriteAllBytes(filename, MakeThumbBytes());

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


        GameObject PGGround = GameObject.Find("Playground/ChessBoard");
        PGGround.GetComponent<PanZoom>().SetPlaygroundSize(float.Parse(tokens[0]), float.Parse(tokens[1]));


        GameObject PGComponents = GameObject.Find("Playground/Components");

        for (int i = 2; i < tokens.Length; i++)
        {
            GameObject component = InstantiateComponent(tokens[i]);
            if (component == null) continue;

            GenericComponent gc = component.GetComponent<GenericComponent>();
            gc.FromJson(tokens[i]);
        }

        if (GameEngine.instance)
            GameEngine.instance.UpdateComponentList();
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