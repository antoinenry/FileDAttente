using System;

[Serializable]
public class TestData2 : IDatapack
{
    public string aString;
    public TestData aProduct;

    public bool IsDataValid => true;

    public string DisplayName => "test " + aString;

    public string DataID { get => aString; set => aString = value; }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        throw new NotImplementedException();
    }
}