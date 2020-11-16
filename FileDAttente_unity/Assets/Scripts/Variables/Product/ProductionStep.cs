// ProductionStep
// Contient les infos relatives à une étape de production
// - operationName: nom de l'opération effectuée lors de cette étape (juste pour l'affichage)
// - workStationKey: clé WS, désigne le poste utilisé (clé unique au poste)
// - durationHours: durée du procédé en heures

using System.Collections.Generic;

public struct ProductionStep
{
    public string operationName;
    public string workStationKey;
    public float durationHours;

    public enum DataError { OperationName_Invalid, WorkStationKey_Invalid, Duration_Invalid }


    public bool IsDataValid(out DataError[] errors)
    {
        List<DataError> errorList = new List<DataError>();

        if (operationName == null || operationName == string.Empty) errorList.Add(DataError.OperationName_Invalid);
        if (workStationKey == null || workStationKey == string.Empty) errorList.Add(DataError.WorkStationKey_Invalid);
        if (durationHours < 0) errorList.Add(DataError.Duration_Invalid);

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
