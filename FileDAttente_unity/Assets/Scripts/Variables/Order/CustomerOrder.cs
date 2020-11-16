// CustomerOrder
// Décrit une commande de plusieurs produits différents
// - name: un nom pour la commande (juste pour l'affichage)
// - products: détail de la commande par produit (liste)

using System.Collections.Generic;

public class CustomerOrder
{
    public string name;
    public ProductOrder[] products;

    public enum DataError { Name_Invalid, Products_Null, Product_Invalid }


    public bool IsDataValid(out DataError[] errors, out int[] productErrorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> productErrorIndexList = new List<int>();

        if (name == null || name == string.Empty)
            errorList.Add(DataError.Name_Invalid);
        if (products == null)
            errorList.Add(DataError.Products_Null);
        else
        {
            for (int i = 0, iend = products.Length; i < iend; i++)
                if (products[i].IsDataValid() == false)
                    productErrorIndexList.Add(i);

            if (productErrorIndexList.Count > 0)
                errorList.Add(DataError.Product_Invalid);
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

    public bool IsDataValid()
    {
        return IsDataValid(out DataError[] errors, out int[] pIndices);
    }
}
