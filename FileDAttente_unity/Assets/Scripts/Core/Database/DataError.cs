
public enum DataError
{
    // Common
    Name_Invalid, Key_Invalid,
    // DemandScenario
    Orders_Null, Order_Invalid,
    // ProductionStep
    OperationName_Invalid, WorkStationKey_Invalid, Duration_Invalid,
    // ProductInfo
    ProductiontSteps_Null, ProductionStep_Invalid,
    // ProductOrder
    ProductKey_Invalid, Quantity_Invalid,
    // CustomerOrder
    Products_Null, Product_Invalid,
    // Workstation
    MachineCount_Invalid,
    // WorkChain
    OpenHours_Invalid, InputRateVariability_Invalid, Workstations_Null, WorkStation_Invalid
}