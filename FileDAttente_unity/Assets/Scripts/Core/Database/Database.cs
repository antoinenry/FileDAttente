// Database
// Contient toutes les configurations de variables disponibles pour les calculs
// Assure l'organisation et la cohérence de ces données
// - productionChains: des configurations de chaine de production
// - demandScenarios: des scenarios de prévision de commandes client
// - productInfos: des produits (les informations sur les procédés impliqués dans leur production)
using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public class Database : IDatapack
{
    public WorkChain[] workChains;
    public DemandScenario[] demandScenarios;
    public ProductInfo[] productInfos;

    public string DataID { get => null; set { } }

    public bool IsDataValid => CheckData(out string[] errors);

    public string DisplayName => "";

    public static Type[] GetDatapackTypes()
    {
        Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
        return Array.FindAll(allTypes, t => t != typeof(IDatapack) && typeof(IDatapack).IsAssignableFrom(t));
    }

    public bool DataValid(out DataError[] errors, out int[] errorIndices)
    {
        List<DataError> errorList = new List<DataError>();
        List<int> indicesList = new List<int>();
        List<IDatapack> dataPacks = GetAllData();
        bool isDataValid = true;
        foreach (IDatapack pack in dataPacks)
        {
            if (pack != null && pack.DataValid(out DataError[] es, out int[] ids) == false)
            {
                isDataValid = false;
                errorList.AddRange(es);
                indicesList.AddRange(ids);
            }
        }

        errors = errorList.ToArray();
        errorIndices = indicesList.ToArray();
        return isDataValid;
    }

    public enum OperationResult { Success, Item_Found, Item_Not_Found, Item_Invalid, Type_Invalid, Database_Corrupted }

    public bool CheckData(out string[] errorArray)
    {
        List<string> errorList = new List<string>();
        Database checkData = new Database();
        List<IDatapack> dataPacks = GetAllData();
        int packCount = dataPacks.Count;
        for (int i = 0; i < packCount; i++)
        {
            IDatapack pack = dataPacks[i];
            OperationResult result = checkData.AddDataPack(pack);
            if (result != OperationResult.Success)
            {
                string errorMessage = result.ToString() + "(" + i + "/" + packCount +"): ";
                if (pack == null)
                    errorMessage += "null reference";
                else
                {
                    errorMessage += pack.ToString() + " // ";
                    if (pack.DataValid(out DataError[] errors, out int[] errorIndices) == false && errors != null && errors.Length > 0)
                    {
                        int errorCount = errors.Length;
                        int errorIndexCount = errorIndices != null ? errorIndices.Length : 0;
                        for(int e = 0; e < errorCount; e++)
                        {
                            errorMessage += errors[e].ToString();
                            if (e < errorIndexCount) errorMessage += "[" + errorIndices[e] + "]";
                            errorMessage += "; ";
                        }
                    }
                    else
                        errorMessage += "Bug in code, sorry :(";
                }
                errorList.Add(errorMessage);
            }
        }

        errorArray = errorList.ToArray();
        return errorArray.Length == 0;
    }

    public List<IDatapack> GetAllData()
    {
        Type[] DataTypes = new Type[] { typeof(WorkChain), typeof(DemandScenario), typeof(ProductInfo) };
        List<IDatapack> allData = new List<IDatapack>();
        foreach(Type dataType in DataTypes)
        {
            if (GetDataList(dataType, out List<IDatapack> packs) == OperationResult.Success)
                allData.AddRange(packs);
        }
        return allData;
    }

    public OperationResult GetDataList(Type dataType, out List<IDatapack> packList)
    {
        packList = null;

        if (dataType != null)
        {
            if (dataType == typeof(WorkChain)) packList = workChains != null ? new List<WorkChain>(workChains).ConvertAll(x => x as IDatapack) : new List<IDatapack>();
            else if (dataType == typeof(DemandScenario)) packList = demandScenarios != null ? new List<DemandScenario>(demandScenarios).ConvertAll(x => x as IDatapack) : new List<IDatapack>();
            else if (dataType == typeof(ProductInfo)) packList = productInfos != null ? new List<ProductInfo>(productInfos).ConvertAll(x => x as IDatapack) : new List<IDatapack>();
        }

        return packList != null ? OperationResult.Success : OperationResult.Type_Invalid;
    }

    public OperationResult AddDataPack(IDatapack pack)
    {
        OperationResult result = SelectDataPackList(pack, out List<IDatapack> selectPacks);
        if (result == OperationResult.Success)
            return AddDataPackTo(ref selectPacks, pack);
        else
            return result;        
    }

    public OperationResult RemoveDataPack(IDatapack pack)
    {
        OperationResult result = SelectDataPackList(pack, out List<IDatapack> selectPacks);
        if (result == OperationResult.Success)
            return RemovePackFrom(ref selectPacks, pack);
        else
            return result;
    }

    private OperationResult SelectDataPackList(IDatapack withPack, out List<IDatapack> selected)
    {
        selected = null;
        return withPack != null ? GetDataList(withPack.GetType(), out selected) : OperationResult.Item_Invalid;
    }

    private OperationResult AddDataPackTo(ref List<IDatapack> packList, IDatapack pack)
    {
        if (packList == null) return OperationResult.Database_Corrupted;
        if (pack.IsDataValid == false) return OperationResult.Item_Invalid;
        if (packList.Contains(pack)) return OperationResult.Item_Found;
        packList.Add(pack);
        return OperationResult.Success;
    }

    private OperationResult RemovePackFrom(ref List<IDatapack> packList, IDatapack pack)
    {
        if (packList == null) return OperationResult.Database_Corrupted;
        int removeIndex = packList.IndexOf(pack);
        if (removeIndex == -1) return OperationResult.Item_Not_Found;
        packList.RemoveAt(removeIndex);
        return OperationResult.Success;
    }
}