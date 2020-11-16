// Database
// Contient toutes les configurations de variables disponibles pour les calculs
// Assure l'organisation et la cohérence de ces données
// - productionChains: des configurations de chaine de production
// - demandScenarios: des scenarios de prévision de commandes client
// - productInfos: des produits (les informations sur les procédés impliqués dans leur production)

using System;
using System.Collections.Generic;

public class Database
{
    private List<ProductionChain> productionChains;
    private List<DemandScenario> demandScenarios;
    private List<ProductInfo> productInfos;

    public enum OperationResult { Success, Item_Found, Item_Not_Found, Item_Invalid, Database_Corrupted }

    // Initialisation
    public Database()
    {
        productionChains = new List<ProductionChain>();
        demandScenarios = new List<DemandScenario>();
        productInfos = new List<ProductInfo>();
    }

    //Opérations/Accès sur la liste de chaines de production 
    public OperationResult AddProductionChain(ProductionChain newChain)
    {
        if (productionChains == null)
            return OperationResult.Database_Corrupted;
        if (newChain.IsDataValid() == false)
            return OperationResult.Item_Invalid;
        if (productionChains.Contains(newChain))
            return OperationResult.Item_Found;

        productionChains.Add(newChain);
        return OperationResult.Success;
    }

    public OperationResult RemoveProductionChain(ProductionChain removeChain)
    {
        if (productionChains == null)
            return OperationResult.Database_Corrupted;

        int removeIndex = productionChains.IndexOf(removeChain);
        if (removeIndex == -1)
            return OperationResult.Item_Not_Found;

        productionChains.RemoveAt(removeIndex);
        return OperationResult.Success;
    }

    public OperationResult GetProductionChains(out ProductionChain[] chains)
    {
        if (productionChains == null)
        {
            chains = null;
            return OperationResult.Database_Corrupted;
        }

        chains = productionChains.ToArray();
        return OperationResult.Success;
    }

    //Idem sur la liste de scénarios 
    public OperationResult AddDemandScenario(DemandScenario newScenario)
    {
        if (demandScenarios == null)
            return OperationResult.Database_Corrupted;
        if (newScenario.IsDataValid() == false)
            return OperationResult.Item_Invalid;
        if (demandScenarios.Contains(newScenario))
            return OperationResult.Item_Found;

        demandScenarios.Add(newScenario);
        return OperationResult.Success;
    }

    public OperationResult RemoveDemandScenario(DemandScenario removeScenario)
    {
        if (demandScenarios == null)
            return OperationResult.Database_Corrupted;

        int removeIndex = demandScenarios.IndexOf(removeScenario);
        if (removeIndex == -1)
            return OperationResult.Item_Not_Found;

        demandScenarios.RemoveAt(removeIndex);
        return OperationResult.Success;
    }

    public OperationResult GetDemandScenarios(out DemandScenario[] chains)
    {
        if (demandScenarios == null)
        {
            chains = null;
            return OperationResult.Database_Corrupted;
        }

        chains = demandScenarios.ToArray();
        return OperationResult.Success;
    }

    //Idem sur la liste de chaines de produits 
    public OperationResult AddProductInfo(ProductInfo newProduct)
    {
        if (productInfos == null)
            return OperationResult.Database_Corrupted;
        if (newProduct.IsDataValid() == false)
            return OperationResult.Item_Invalid;
        if (productInfos.Contains(newProduct))
            return OperationResult.Item_Found;

        productInfos.Add(newProduct);
        return OperationResult.Success;
    }

    public OperationResult RemoveProductInfo(ProductInfo removeProduct)
    {
        if (productInfos == null)
            return OperationResult.Database_Corrupted;

        int removeIndex = productInfos.IndexOf(removeProduct);
        if (removeIndex == -1)
            return OperationResult.Item_Not_Found;

        productInfos.RemoveAt(removeIndex);
        return OperationResult.Success;
    }

    public OperationResult GetProductInfos(out ProductInfo[] chains)
    {
        if (productInfos == null)
        {
            chains = null;
            return OperationResult.Database_Corrupted;
        }

        chains = productInfos.ToArray();
        return OperationResult.Success;
    }
}
