// ProductInfos
// Contient les infos relatives à un produit
// - productKey: clé produit, unique à ce produit
// - productionSteps: liste des étapes de production de ce produit (gamme?)

using System;
using System.Collections.Generic;

[Serializable]
public struct ProductInfo : IDatapack
{
    public string productKey;
    public ProductionStep[] productionSteps;

    public string DataID { get => productKey; set => productKey = value; }

    public string DisplayName => productKey != null && productKey != "" ? productKey : "...";

    public bool IsDataValid => DataValid(out DataError[] errors, out int[] indices);

    public bool DataValid(out DataError[] errors, out int[] productionStepErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> productiontStepErrorIndexList = new List<int>();

        if (productKey == null || productKey == string.Empty)
        {
            errorList.Add(DataError.Name_Invalid);
            productiontStepErrorIndexList.Add(-1);
        }
        if (productionSteps == null)
        {
            errorList.Add(DataError.ProductiontSteps_Null);
            productiontStepErrorIndexList.Add(-1);
        }
        else
        {
            for (int i = 0, iend = productionSteps.Length; i < iend; i++)
                if (productionSteps[i].IsDataValid == false)
                {
                    errorList.Add(DataError.ProductionStep_Invalid);
                    productiontStepErrorIndexList.Add(i);
                }
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
}
