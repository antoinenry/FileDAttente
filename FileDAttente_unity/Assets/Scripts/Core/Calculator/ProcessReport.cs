

public class ProcessReport : IDatapack
{
    public string product;
    public string process;
    public float duration;
    public float inputRate;

    public bool IsDataValid => true;

    public string DisplayName => product + " - " + process;

    public string DataID { get => null; set { } }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        throw new System.NotImplementedException();
    }
}