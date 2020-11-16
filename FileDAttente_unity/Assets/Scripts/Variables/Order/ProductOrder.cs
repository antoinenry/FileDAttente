// ProductOrder
// Décrit une commande d'une certaine quantité d'un certain produit
// - productKey: clé du produit commandé, unique au produit
// - quantity: quantité souhaitée

using System.Collections.Generic;

public class ProductOrder
{
    public string productKey;
    public int quantity;

    public enum DataError { ProductKey_Invalid, Quantity_Invalid }


    public bool IsDataValid(out DataError[] errors)
    {
        List<DataError> errorList = new List<DataError>();

        if (productKey == null || productKey == string.Empty) errorList.Add(DataError.ProductKey_Invalid);
        if (quantity < 0) errorList.Add(DataError.Quantity_Invalid);

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
