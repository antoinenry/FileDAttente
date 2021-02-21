using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewDataBase", menuName = "FileDattente/DataBase", order = 1)]
public class DataBaseSave : ScriptableObject
{
    [SerializeField] public Database data;
}
