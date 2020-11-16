// WorkStation
// Contient les infos relatives à un poste de travail
// - key: clé du poste, permet de l'identifier (unique au poste)
// - machineCount: nombre de machine sur ce poste

using System.Collections.Generic;

public class WorkStation
{
    public string key;
    public int machineCount;

    public enum DataError { Key_Invalid, MachineCount_Invalid }


    public bool IsDataValid(out DataError[] errors)
    {
        List<DataError> errorList = new List<DataError>();

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

    public bool IsDataValid()
    {
        return IsDataValid(out DataError[] errors);
    }
}
