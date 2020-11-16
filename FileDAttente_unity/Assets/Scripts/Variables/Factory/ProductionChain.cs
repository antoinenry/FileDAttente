// ProductionChain
// Décrit la configuration de l'usine/ de la chaine de production
// - openHours: temps d'ouverture (remplaçable par une heure d'ouverture et une heure de fermeture)
// - inputRateVariability: variabilité du débit d'entrée de la chaine
// - workstations: liste (ordonnée) des postes de travail dans la chaine de production

using System.Collections.Generic;

public struct ProductionChain
{
    public float openHours;
    public float inputRateVariability;
    public WorkStation[] workstations;

    public enum DataError { OpenHours_Invalid, InputRateVariability_Invalid, Workstations_Null, WorkStation_Invalid }


    public bool IsDataValid(out DataError[] errors, out int[] workStationErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> workStationIndexList = new List<int>();

        if (openHours < 0)
            errorList.Add(DataError.OpenHours_Invalid);
        if (inputRateVariability < 0f || inputRateVariability > 1f)
            errorList.Add(DataError.InputRateVariability_Invalid);
        if (workstations == null)
            errorList.Add(DataError.Workstations_Null);
        else
        {
            for (int i = 0, iend = workstations.Length; i < iend; i++)
                if (workstations[i].IsDataValid() == false)
                    workStationIndexList.Add(i);

            if (workStationIndexList.Count > 0)
                errorList.Add(DataError.WorkStation_Invalid);
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

    public bool IsDataValid()
    {
        return IsDataValid(out DataError[] errors, out int[] wsIndices);
    }
}
