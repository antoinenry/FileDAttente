using System;

[Serializable]
public class TestData : IDatapack
{
    public string aString;
    [DatabaseReference(typeof(ProductInfo), "productInfos")]
    public string aData;

    public bool IsDataValid => true;

    public string DisplayName => "test " + aString;

    public string DataID { get => aString; set => aString = value; }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        throw new NotImplementedException();
    }
}