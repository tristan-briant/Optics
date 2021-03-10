using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public int LevelToBeloaded;

    public void SelectLevel()
    {
        LevelManager.instance.SelectLevel(LevelToBeloaded);
    }
}
