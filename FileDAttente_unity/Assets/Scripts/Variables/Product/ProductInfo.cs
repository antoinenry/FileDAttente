// ProductInfos
// Contient les infos relatives à un produit
// - productKey: clé produit, unique à ce produit
// - productionSteps: liste des étapes de production de ce produit (gamme?)

using System.Collections.Generic;

public struct ProductInfo
{
    public string productKey;
    public ProductionStep[] productionSteps;

    public enum DataError { Name_Invalid, ProductiontSteps_Null, ProductionStep_Invalid }


    public bool IsDataValid(out DataError[] errors, out int[] productionStepErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> productiontStepErrorIndexList = new List<int>();

        if (productKey == null || productKey == string.Empty)
            errorList.Add(DataError.Name_Invalid);
        if (productionSteps == null)
            errorList.Add(DataError.ProductiontSteps_Null);
        else
        {
            for (int i = 0, iend = productionSteps.Length; i < iend; i++)
                if (productionSteps[i].IsDataValid() == false)
                    productiontStepErrorIndexList.Add(i);

            if (productiontStepErrorIndexList.Count > 0)
                errorList.Add(DataError.ProductionStep_Invalid);
        }

        if (errorList.Count == 0)
        {
            errors = null;
            productionStepErrorIndices = null;
            return true;
        }
        else
        {
            errors = errorList.ToArray();
            productionStepErrorIndices = productiontStepErrorIndexList.ToArray();
            return false;
        }
    }

    public bool IsDataValid()
    {
        return IsDataValid(out DataError[] errors, out int[] pIndices);
    }
}
