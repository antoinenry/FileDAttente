// ProductOrder
// Décrit une commande d'une certaine quantité d'un certain produit
// - productKey: clé du produit commandé, unique au produit
// - quantity: quantité souhaitée

using System;
using System.Collections.Generic;

[Serializable]
public struct ProductOrder : IDatapack
{
    [DatabaseReference(typeof(ProductInfo), "productInfos")]
    public string product;
    public int quantity;

    public string DataID { get => null; set { } }

    public string DisplayName => "'" + product + "' x" + quantity;

    public bool IsDataValid => DataValid(out global::DataError[] errors, out int[] indices);

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        errorIndices = null;

        //if (productKey == null) errorList.Add(DataError.ProductKey_Invalid);
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
}
