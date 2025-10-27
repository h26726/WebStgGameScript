/// <summary>
/// 任何可列印的物件都可以實作此介面
/// </summary>
public interface IPrintable
{
    /// <summary>
    /// 產生文字描述，用於除錯或 log
    /// </summary>
    /// <returns>物件資訊字串</returns>
    string Print();
}