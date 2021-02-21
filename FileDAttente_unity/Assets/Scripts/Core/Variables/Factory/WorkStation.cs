// WorkStation
// Contient les infos relatives à un poste de travail
// - key: clé du poste, permet de l'identifier (unique au poste)
// - machineCount: nombre de machine sur ce poste

using System;
using System.Collections.Generic;

[Serializable]
public struct WorkStation : IDatapack
{
    public string key;
    public int machineCount;
    //public float durationVariability;

    public string DataID { get => null; set { } }

    public string DisplayName => key != null && key != "" ? key : "...";

    public bool IsDataValid => DataValid(out DataError[] errors, out int[] indices);

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        errorIndices = null;

        if (key == null || key == string.Empty) errorList.Add(DataError.Key_Invalid);
        if (machineCount < 0) errorList.Add(DataError.MachineCount_Invalid);

        if (errorList.Count == 0)
        {
            errors = null;
            return true;
        }
        else
        {
            errors = errorList.ToArray();
            return false;
        }
    }
}
