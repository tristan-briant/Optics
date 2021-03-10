using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//Tells the Editor class that this will be the Editor for the LevelManager
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    private ReorderableList list;
    const float cellHeight = 50;
    int activeCell = -1;

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Levels"),
                true, true, true, true);

        list.drawElementCallback = drawElement;
        list.elementHeightCallback = (index) => { if (index == activeCell) return cellHeight * 2; return cellHeight; };
        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Levels name"); };
    }



    void drawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        if (isActive)
            activeCell = index;

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y + 2, 100, EditorGUIUtility.singleLineHeight),
            element, GUIContent.none);

        string filename = element.stringValue;
        Debug.Log("Levels/" + filename);

        if (filename != "")
        {
            Texture texture = Resources.Load<Texture>("Levels/" + filename);
            if (texture)
            {
                float w = texture.width * (rect.height - 4) / texture.height;
                EditorGUI.DrawPreviewTexture(
                    new Rect(rect.x + 160, rect.y + 2, w, rect.height - 4), texture);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}