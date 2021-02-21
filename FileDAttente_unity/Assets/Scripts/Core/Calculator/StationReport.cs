using System;

public class StationReport : IDatapack
{
    public string workStation;
    public float cycleTimeQueuing;
    public int machineCount;
    public int productionCount;
    //public float totalProcessDuration;
    public float averageProcessDuration;
    public float usageIntensity;
    public float inputRateVariability;
    public float durationVariability;
    public ProcessReport[] processes;

    public bool IsDataValid => true;

    public string DisplayName => workStation + ": " + cycleTimeQueuing + " h";

    public string DataID { get => null; set { } }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        throw new NotImplementedException();
    }

    public void AddProcess(ProcessReport pr)
    {
        if (processes == null) processes = new ProcessReport[0];
        int count = processes.Length;
        Array.Resize(ref processes, count + 1);
        processes[count] = pr;
    }
}