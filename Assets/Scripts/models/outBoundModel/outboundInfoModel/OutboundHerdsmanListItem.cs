using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutboundHerdsmanListItem 
{
    /// <summary>
    /// 
    /// </summary>
    public string applyId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string herdsmanId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int detectSampleNum { get; set; }
    /// <summary>
    /// 鄂托克旗农牧局
    /// </summary>
    public string HusbandryBureauName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int abnormalSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string applyCode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<OutboundSampleListItem> sampleList { get; set; }
    /// <summary>
    /// 牛革命
    /// </summary>
    public string herdsmanName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int hasInspectedNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int reciveSampleNum { get; set; }
}
