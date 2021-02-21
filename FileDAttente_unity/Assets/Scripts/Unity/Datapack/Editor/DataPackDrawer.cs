using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(DataPackAttribute))]
public class DataPackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            List<Type> typeOptions = new List<Type>(Database.GetDatapackTypes());
            List<string> typeAssemblyNames = new List<string> { "(null)" };
            typeAssemblyNames.AddRange(typeOptions.ConvertAll(t => t.AssemblyQualifiedName));
            List<string> typeShortNames = new List<string> { "(null)" };
            typeShortNames.AddRange(typeOptions.ConvertAll(t => t.Name));

            int selectedIndex = Array.IndexOf(typeAssemblyNames.ToArray(), property.stringValue); ;
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, typeShortNames.ToArray());
            if (selectedIndex >= 0 && selectedIndex < typeAssemblyNames.Count)
                property.stringValue = typeAssemblyNames[selectedIndex];
        }
    }
}