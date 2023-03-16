using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutboundInfoRoot 
{
    /// <summary>
    /// 
    /// </summary>
    public bool success { get; set; }
    /// <summary>
    /// ²Ù×÷³É¹¦£¡
    /// </summary>
    public string message { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public OutboundPatchInfoRes result { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long timestamp { get; set; }
}
