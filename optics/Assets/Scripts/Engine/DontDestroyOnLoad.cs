using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            //SceneManager.LoadScene("MiniMap");
        }
        else{
            DestroyImmediate(gameObject);
        }
    }

}
