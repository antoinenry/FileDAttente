//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(UICardScriptable))]
//public class UICardScriptableInspector : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        UICardScriptable targetCard = target as UICardScriptable;
//        DataTypeEditor(targetCard);
//        UICard content = targetCard.content;
//        ContentEditor(content);
//    }

//    private void DataTypeEditor(UICardScriptable targetCard)
//    {
//        EditorGUI.BeginChangeCheck();
//        string dataType = EditorGUILayout.TextField("Data type", targetCard.dataType);
//        if (EditorGUI.EndChangeCheck() == true)
//        {
//            Undo.RecordObject(target, "Change card type");
//            targetCard.dataType = dataType;
//            targetCard.BuildFromDataType();
//            EditorUtility.SetDirty(target);
//        }
//    }

//    private void ContentEditor(UICard content)
//    {
//        if ((Application.isPlaying == true && (content == null || content.GetDataType() == null)) ||
//            (content == null || content.fields == null))
//        {
//                EditorGUILayout.HelpBox("No content", MessageType.Error);
//        }
//        else
//        {
//            int fieldCpt = 0;
//            EditorGUILayout.LabelField("Fields:");
//            foreach (UIField f in content.fields)
//            {
//                if (f != null) EditorGUILayout.LabelField("Field #" + fieldCpt++, f.GetType().Name);
//                EditorGUILayout.BeginVertical("box");
//                FieldEditor(f);
//                EditorGUILayout.EndVertical();
//            }
//        }
//    }

//    private void FieldEditor(UIField field)
//    {
//        if (field == null)
//        {
//            EditorGUILayout.HelpBox("Field is null.", MessageType.Error);
//            return;
//        }

//        if (field is UIValueField)
//            ShowValueField(field as UIValueField);
//        else if (field is UIDataPackField)
//            ShowDataPackField(field as UIDataPackField);
//        else if (field is UIValueArrayField)
//            ShowValueArrayField(field as UIValueArrayField);

//        //EditorGUILayout.LabelField("Input type", field.inputMethod.ToString());
//    }

//    private void ShowValueField(UIValueField field)
//    {
//        if (field != null)
//        {
//            if (Application.isPlaying)
//                EditorGUILayout.LabelField(field.label, field.StringInput);
//            else
//                EditorGUILayout.LabelField(field.label);
//        }
//    }

//    private void ShowDataPackField(UIDataPackField field)
//    {
//        if (field != null)
//        {
//            if (Application.isPlaying || field.InputCard == null)
//                EditorGUILayout.LabelField(field.label, "null");
//            else
//                EditorGUILayout.LabelField(field.label, field.InputCard.GetDataType().Name);
//        }
//    }

//    private void ShowValueArrayField(UIValueArrayField field)
//    {
//        /*
//        if (field != null)
//        {
//            int valueCount = field.itemFields != null ? field.itemFields.Length : 0;
//            if (Application.isPlaying)
//                EditorGUILayout.LabelField(field.label, "(" + valueCount + ")");
//            else
//                EditorGUILayout.LabelField(field.label);

//            if (valueCount == 0) return;
//            EditorGUILayout.BeginVertical("box");
//            if (field.itemFields == null) field.itemFields = new string[0];
//            for (int i = 0; i < field.itemFields.Length; i++)
//                EditorGUILayout.LabelField("#" + i, field.itemFields[i]);        
//            EditorGUILayout.EndVertical();
//        }
//        */
//    }
//}