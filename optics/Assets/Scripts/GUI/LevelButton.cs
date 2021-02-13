using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public void SelectLevel(int n)
    {
        LevelManager.instance.SelectLevel(n);
    }
}
