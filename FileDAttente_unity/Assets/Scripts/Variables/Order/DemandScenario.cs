// DemandScenario
// Décrit une prévision de demande, c-a-d un ensemble de commandes
// - name: un nom pour le scnario (juste pour l'affichage)
// - orders: une liste de commandes

using System.Collections.Generic;

public struct DemandScenario
{
    public string name;
    public CustomerOrder[] orders;

    public enum DataError { Name_Invalid, Orders_Null, Order_Invalid }


    public bool IsDataValid(out DataError[] errors, out int[] orderErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> orderErrorIndexList = new List<int>();

        if (name == null || name == string.Empty)
            errorList.Add(DataError.Name_Invalid);
        if (orders == null)
            errorList.Add(DataError.Orders_Null);
        else
        {
            for (int i = 0, iend = orders.Length; i < iend; i++)
                if (orders[i].IsDataValid() == false)
                    orderErrorIndexList.Add(i);

            if (orderErrorIndexList.Count > 0)
                errorList.Add(DataError.Order_Invalid);
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

    public bool IsDataValid()
    {
        return IsDataValid(out DataError[] errors, out int[] pIndices);
    }
}
