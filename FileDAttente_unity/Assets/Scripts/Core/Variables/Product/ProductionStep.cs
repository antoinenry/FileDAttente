// ProductionStep
// Contient les infos relatives à une étape de production
// - operationName: nom de l'opération effectuée lors de cette étape (juste pour l'affichage)
// - workStationKey: clé WS, désigne le poste utilisé (clé unique au poste)
// - durationHours: durée du procédé en heures

using System;
using System.Collections.Generic;

[Serializable]
public struct ProductionStep : IDatapack
{
    public string operationName;
    public string workStationKey;
    public float duration;

    public string DataID { get => null; set { } }

    public bool IsDataValid => DataValid(out DataError[] errors, out int[] errorIndices);

    public string DisplayName => operationName;

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        errorIndices = null;

        if (operationName == null || operationName == string.Empty) errorList.Add(DataError.OperationName_Invalid);
        if (workStationKey == null || workStationKey == string.Empty) errorList.Add(DataError.WorkStationKey_Invalid);
        if (duration < 0) errorList.Add(DataError.Duration_Invalid);

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
