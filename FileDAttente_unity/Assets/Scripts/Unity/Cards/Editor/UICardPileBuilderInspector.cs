using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UICardPileBuilder))]
public class UICardPileBuilderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Edit PileBuilder");
            (target as UICardPileBuilder).ResetPile();
        }
    }
}
