using UnityEngine;

[System.Serializable]
public struct LevelDescriptor
{
    public string LevelName;
    public Sprite thumbnail;
    // Start is called before the first frame update

    [ContextMenu("Do Something")]
    void DoSomething()
    {
        Debug.Log("Perform operation");
    }
}
