using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SackData 
{
    /// <summary>
    /// 袋号
    /// </summary>
    public string sackCode { get; set; }
    /// <summary>
    /// 样品总数
    /// </summary>
    public int sampleSum { get; set; }
    /// <summary>
    /// 送检数量
    /// </summary>
    public int inspectSamSum { get; set; }

    /// <summary>
    /// 感应数量
    /// </summary>
    public int detectSamSum { get; set; }
    /// <summary>
    /// 缺少样品数量
    /// </summary>
    public int lackSamNum { get; set; }
    /// <summary>
    /// 无耳号数量
    /// </summary>
    public int noEarcodeSum { get; set; }

    /// <summary>
    /// 无标签数量
    /// </summary>
    public int noTagSamSum { get; set; }
    /// <summary>
    /// 货架编号
    /// </summary>
    public string rackCode { get; set; }
    /// <summary>
    /// 该袋中包含的牧民
    /// </summary>
    public List<HerdsmanListItem> herdsmanItems { get; set; }

}
