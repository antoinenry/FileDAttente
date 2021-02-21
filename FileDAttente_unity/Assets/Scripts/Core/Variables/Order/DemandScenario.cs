// DemandScenario
// Décrit une prévision de demande, c-a-d un ensemble de commandes
// - name: un nom pour le scnario (juste pour l'affichage)
// - orders: une liste de commandes

using System;
using System.Collections.Generic;

[Serializable]
public struct DemandScenario : IDatapack
{
    public string name;
    public CustomerOrder[] orders;

    public string DataID { get => name; set { } }

    public bool IsDataValid => DataValid(out DataError[] errors, out int[] indices);

    public string DisplayName => name != null ? name : "";

    public bool DataValid(out DataError[] errors, out int[] orderErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> orderErrorIndexList = new List<int>();

        if (name == null || name == string.Empty)
        {
            errorList.Add(DataError.Name_Invalid);
            orderErrorIndexList.Add(-1);
        }
        if (orders == null)
        {
            errorList.Add(DataError.Orders_Null);
            orderErrorIndexList.Add(-1);
        }
        else
        {
            for (int i = 0, iend = orders.Length; i < iend; i++)
                if (orders[i].IsDataValid == false)
                {
                    errorList.Add(DataError.Order_Invalid);
                    orderErrorIndexList.Add(i);
                }
        }

        if (errorList.Count == 0)
        {
            errors = null;
            orderErrorIndices = null;
            return true;
        }
        else
        {
            errors = errorList.ToArray();
            orderErrorIndices = orderErrorIndexList.ToArray();
            return false;
        }
    }
}
