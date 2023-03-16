/// <summary>
/// 通过返回结果获取
/// </summary>
public class ReturnMessage
{
    /// <summary>
    /// 
    /// </summary>
    public bool success { get; set; }
    /// <summary>
    /// 操作成功！
    /// </summary>
    public string message { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 样品接收成功！
    /// </summary>
    public object result { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long timestamp { get; set; }
}
