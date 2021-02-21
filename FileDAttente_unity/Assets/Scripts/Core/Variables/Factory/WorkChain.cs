// ProductionChain
// Décrit la configuration de l'usine/ de la chaine de production
// - openHours: temps d'ouverture (remplaçable par une heure d'ouverture et une heure de fermeture)
// - inputRateVariability: variabilité du débit d'entrée de la chaine
// - workstations: liste (ordonnée) des postes de travail dans la chaine de production

using System;
using System.Collections.Generic;

[Serializable]
public struct WorkChain : IDatapack
{
    public string name;
    public float openHours;
    public float inputRateVariability;
    public WorkStation[] workstations;

    public string DataID { get => name; set { } }

    public bool IsDataValid => DataValid(out DataError[] errors, out int[] indices);

    public string DisplayName => name != null ? name : "";

    public bool DataValid(out DataError[] errors, out int[] workStationErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> workStationIndexList = new List<int>();

        if (openHours < 0)
        {
            errorList.Add(DataError.OpenHours_Invalid);
            workStationIndexList.Add(-1);
        }
        if (inputRateVariability < 0f || inputRateVariability > 1f)
        {
            errorList.Add(DataError.InputRateVariability_Invalid);
            workStationIndexList.Add(-1);
        }

        if (workstations == null)
        {
            errorList.Add(DataError.Workstations_Null);
            workStationIndexList.Add(-1);
        }
        else
        {
            for (int i = 0, iend = workstations.Length; i < iend; i++)
                if (workstations[i].IsDataValid == false)
                {
                    errorList.Add(DataError.WorkStation_Invalid);
                    workStationIndexList.Add(i);
                }
        }

        if (errorList.Count == 0)
        {
            errors = null;
            workStationErrorIndices = null;
            return true;
        }
        else
        {
            errors = errorList.ToArray();
            workStationErrorIndices = workStationIndexList.ToArray();
            return false;
        }
    }

    public int GetWorkStationIndex(string key)
    {
        return workstations != null ? Array.FindIndex(workstations, x => x.key == key) : -1;
    }
}
