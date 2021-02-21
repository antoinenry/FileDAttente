using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DataBaseSave))]
public class DataBaseSaveInspector : Editor
{
    private string[] warningMessages;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck() == true) warningMessages = null;

        if (warningMessages == null)
        {
            Database data = (target as DataBaseSave).data;
            if (data.CheckData(out warningMessages) == false)
            {
            }
        }
        foreach (string w in warningMessages)
            EditorGUILayout.HelpBox(w, MessageType.Warning);
    }
}
