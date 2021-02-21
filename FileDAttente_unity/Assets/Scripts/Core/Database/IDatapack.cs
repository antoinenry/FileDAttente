
public interface IDatapack
{
    bool IsDataValid { get; }
    bool DataValid(out DataError[] errors, out int[] errorIndices);
    string DisplayName { get; }
    string DataID { get; set; }
}