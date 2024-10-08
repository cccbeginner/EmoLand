#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlantHelper))]
public class PlantGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlantHelper myScript = (PlantHelper)target;
        if (GUILayout.Button("Update Plant"))
        {
            myScript.UpdatePlant();
        }
    }
}
#endif
