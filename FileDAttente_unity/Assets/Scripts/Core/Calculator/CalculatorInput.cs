using System;

[Serializable]
public struct CalculatorInput : IDatapack
{
    [DatabaseReference(typeof(WorkChain), "workChains")]
    public string workChain;
    [DatabaseReference(typeof(DemandScenario), "demandScenarios")]
    public string scenario;

    public bool IsDataValid => true;// workChain.IsDataValid && scenario.IsDataValid;

    public string DisplayName => workChain + " - " + scenario;

    public string DataID { get => null; set { } }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        throw new System.NotImplementedException();
    }
}
