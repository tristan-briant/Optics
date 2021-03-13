using UnityEngine;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int LevelToBeloaded;
    public TextMeshProUGUI text;

    void Start()
    {

        text?.SetText((LevelToBeloaded + 1).ToString());
    }

    public void SelectLevel()
    {
        LevelManager.instance.SelectLevel(LevelToBeloaded);
    }


}
