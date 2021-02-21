using System;
using System.Collections.Generic;
public class Calculator
{
    private CalculatorInput input;
    private CalculatorOutput output;

    public CalculatorInput Input
    {
        get => input;
        set
        {
            input = value;
            Compute();
        }
    }

    public CalculatorOutput Output
    {
        get => output;
    }

    private void Compute()
    {
        output = new CalculatorOutput() { inputSummary = input.DisplayName };
        InitializeStationReports();
        ComputeStationReport();
    }

    private void InitializeStationReports()
    {
        if (DatabaseReferenceAttribute.TryGetSourceItem("workChains", input.workChain, out WorkChain chain) == false)
            return;
        int stationCount = chain.workstations != null ? chain.workstations.Length : 0;
        output.reports = new StationReport[stationCount];
        for (int i = 0; i < stationCount; i++)
        {
            output.reports[i] = new StationReport()
            {
                workStation = chain.workstations[i].key,
                machineCount = chain.workstations[i].machineCount
            };
        }
    }

    private void ComputeStationReport()
    {
        //if (Output == null || input == null) return;
        StationReport[] reports = output.reports;
        if (reports == null) return;
        if (DatabaseReferenceAttribute.TryGetSourceItem("workChains", input.workChain, out WorkChain chain) == false)
            return;
        if (DatabaseReferenceAttribute.TryGetSourceItem("demandScenarios", input.scenario, out DemandScenario scenario) == false)
            return;
        if (scenario.orders == null) return;

        //Quantité de produits et procédés par poste, temps total
        foreach (StationReport r in reports)
        {
            r.productionCount = 0;
            r.processes = new ProcessReport[0];
            //r.totalProcessDuration = 0f;
        }
        foreach(CustomerOrder co in scenario.orders)
        {
            foreach(ProductOrder po in co.products)
            {                
                if (DatabaseReferenceAttribute.TryGetSourceItem("productInfos", po.product, out ProductInfo product) == false)
                    continue;
                foreach (ProductionStep ps in product.productionSteps)
                {
                    int wsIndex = chain.GetWorkStationIndex(ps.workStationKey);
                    if (wsIndex != -1)
                    {
                        reports[wsIndex].productionCount += po.quantity;
                        ProcessReport pr = new ProcessReport()
                        {
                            product = product.productKey,
                            process = ps.operationName + " (" + ps.workStationKey + ")",
                            duration = ps.duration,
                            inputRate = po.quantity / chain.openHours
                        };
                        reports[wsIndex].AddProcess(pr);
                        //reports[wsIndex].totalProcessDuration += po.quantity * ps.duration;
                    }
                }
            }
        }

        //Temps moyen de procédé
        foreach (StationReport r in reports)
        {
            float rateSum = 0f;
            float average_numerator = 0f;
            foreach(ProcessReport pr in r.processes)
            {
                rateSum += pr.inputRate;
                average_numerator += pr.inputRate * pr.duration;
            }
            r.averageProcessDuration = average_numerator / rateSum;

            float sqr_deviation_numerator = 0f;
            foreach (ProcessReport pr in r.processes)
                sqr_deviation_numerator += pr.inputRate * (float)Math.Pow(pr.duration - r.averageProcessDuration, 2f);
            r.durationVariability = (float)Math.Sqrt(sqr_deviation_numerator) / r.averageProcessDuration;

            //if (r.productionCount != 0)
            //    r.averageProcessDuration = r.totalProcessDuration / r.productionCount;
            //else
            //    r.averageProcessDuration = 0f;
        }

        //Utilisation
        for (int i = 0, iend = reports.Length; i < iend; i++)
        {
            float inputDebit = chain.openHours != 0f ? reports[i].productionCount / chain.openHours : 0f;
            if (chain.workstations[i].machineCount != 0)
                reports[i].usageIntensity = inputDebit * reports[i].averageProcessDuration / chain.workstations[i].machineCount;
            else
                reports[i].usageIntensity = 0f;
        }

        //Coefficient de variabilité du débit d'arrivée
        if (reports.Length > 0)
            reports[0].inputRateVariability = chain.inputRateVariability;
        for (int i = 1, iend = reports.Length; i < iend; i++)
        {
            double u2 = Math.Pow(reports[i-1].usageIntensity, 2f);
            double ca2 = Math.Pow(reports[i-1].inputRateVariability, 2f);
            double ce2 = Math.Pow(reports[i - 1].durationVariability, 2f);
            double sqrtm = Math.Sqrt(chain.workstations[i - 1].machineCount);
            if (sqrtm != 0)
            {
                double cd = 1f + (1f - u2) * (ca2 - 1f) + (u2 / sqrtm) * (ce2 - 1f);
                reports[i].inputRateVariability = (float)cd;
            }
        }

        //Temps d'attente
        for (int i = 1, iend = reports.Length; i < iend; i++)
        {
            double ca2 = Math.Pow(reports[i].inputRateVariability, 2f);
            double ce2 = Math.Pow(reports[i - 1].durationVariability, 2f);
            float u = reports[i].usageIntensity;
            float m = chain.workstations[i].machineCount;
            float te = reports[i].averageProcessDuration;
            double ctq = ((ca2 + ce2) / 2f) * (u * Math.Sqrt(2f * (m + 1f) / (m * (1f - u)))) * te;

            reports[i].cycleTimeQueuing = (float)ctq;
        }

        output.reports = reports;
    }
}
