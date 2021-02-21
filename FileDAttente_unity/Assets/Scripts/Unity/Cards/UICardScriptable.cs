//using UnityEngine;
//using System;

//[CreateAssetMenu(fileName = "NewUICard", menuName = "FileDattente/UICard", order = 2)]
//public class UICardScriptable : ScriptableObject
//{
//    [SerializeField] public string dataType;
//    [SerializeField] public UICard content;

//    private void OnEnable()
//    {
//        if (dataType == null) dataType = "";
//        Type currentDataType = Type.GetType(dataType);
//        if (content == null) content = UICard.NewCard(currentDataType, false);
//        else if (content.GetDataType() == null) content.SetDataType(currentDataType);
//    }

//    public void BuildFromDataType()
//    {
//        Type type = Type.GetType(dataType);
//        if (content == null || content.GetDataType() != type)
//            content = UICard.NewCard(type, false);
//        else if (content.GetDataType() != null)
//            dataType = content.GetDataType().Name;
//        else
//            dataType = "(select type)";
//    }
//}
