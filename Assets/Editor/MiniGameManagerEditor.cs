using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MiniGameManager), true)]
public class MiniGameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MiniGameManager manager = (MiniGameManager)target;
        manager.InitializeObservables();

        var middle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter};
        middle.fontStyle = FontStyle.Bold;
        GUILayout.Label("Observable Variables", middle);
        middle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        foreach (string name in manager.GetAllObservablesNames())
        {
            string[] split = name.Split('|');
            EditorGUILayout.LabelField(split[0], split[1], middle);
        }
    }
}
