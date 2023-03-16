using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleInfo
{
    /// <summary>
    /// 是否请求成功
    /// </summary>
    public bool success { get; set; }
    /// <summary>
    /// 操作信息
    /// </summary>
    public string message { get; set; }
    /// <summary>
    /// 请求码
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 机构信息
    /// </summary>
    public InstitutionInfo result { get; set; }
    /// <summary>
    /// 时间戳
    /// </summary>
    public long timestamp { get; set; }
}
