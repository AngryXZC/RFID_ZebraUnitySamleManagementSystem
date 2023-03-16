using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstitutionInfo
{
    /// <summary>
    /// 库房管理员手机号
    /// </summary>
    public string creator_phone { get; set; }

    /// <summary>
    /// 批次编号
    /// </summary>
    public string applyCode { get; set; }
    /// <summary>
    /// 批次名称
    /// </summary>
    public string patchTitle { get; set; }
    /// <summary>
    /// 该单位下的牧民数量
    /// </summary>
    public int herdsmanQuantity { get; set; }
    /// <summary>
    /// 机构名称
    /// </summary>
    public string sampleSource { get; set; }
    /// <summary>
    /// 实收样品总数
    /// </summary>
    public int actualSampleSum { get; set; }
    /// <summary>
    /// 检测牧民总数
    /// </summary>
    public int detectHermanNum { get; set; }
    /// <summary>
    /// 该机构下的牧民列表
    /// </summary>
    public List<HerdsmanListItem> herdsmanList { get; set; }
    /// <summary>
    /// 该机构下的样品总数
    /// </summary>
    public int sampleQuantitys { get; set; }
}
