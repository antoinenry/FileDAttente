// CustomerOrder
// Décrit une commande de plusieurs produits différents
// - name: un nom pour la commande (juste pour l'affichage)
// - products: détail de la commande par produit (liste)

using System;
using System.Collections.Generic;

[Serializable]
public struct CustomerOrder : IDatapack
{
    public string name;
    public ProductOrder[] products;

    public string DataID { get => null; set { } }

    public string DisplayName => name != null && name != "" ? name : "...";

    public bool IsDataValid => DataValid(out DataError[] errors, out int[] indices);

    public bool DataValid(out DataError[] errors, out int[] productErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> productErrorIndexList = new List<int>();

        if (name == null || name == string.Empty)
        {
            errorList.Add(DataError.Name_Invalid);
            productErrorIndexList.Add(-1);
        }
        if (products == null)
        {
            errorList.Add(DataError.Products_Null);
            productErrorIndexList.Add(-1);
        }
        else
        {
            for (int i = 0, iend = products.Length; i < iend; i++)
                if (products[i].IsDataValid == false)
                {
                    errorList.Add(DataError.Product_Invalid);
                    productErrorIndexList.Add(i);
                }
        }

        if (errorList.Count == 0)
        {
            errors = null;
            productErrorIndices = null;
            return true;
        }
        else
        {
            errors = errorList.ToArray();
            productErrorIndices = productErrorIndexList.ToArray();
            return false;
        }
    }
}
