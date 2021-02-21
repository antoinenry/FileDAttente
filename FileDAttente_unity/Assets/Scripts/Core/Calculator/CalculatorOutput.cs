using System;

[Serializable]
public struct CalculatorOutput : IDatapack
{
    public string inputSummary;
    public StationReport[] reports;

    public bool IsDataValid => true;

    public string DisplayName => "CalculatorOutput (placeholder)";

    public string DataID { get => null; set { } }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        throw new System.NotImplementedException();
    }
}
