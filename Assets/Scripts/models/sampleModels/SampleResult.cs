using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleResult 
{
    /// <summary>
    /// 库房管理人员手机号
    /// </summary>
    private string creator_phone;
    /// <summary>
    /// 送检单位编码
    /// </summary>
    private string applyCode;
    /// <summary>
    /// 送检单位名称
    /// </summary>
    private string inspectDepartmentName;
    /// <summary>
    /// 实收样品数量
    /// </summary>
    private int actualSampleSum;

    /// <summary>
    /// 牧民列表
    /// </summary>
    private List<HerdsmanListItem> herdsmanLists;
    /// <summary>
    /// 检测到的牧民数量
    /// </summary>
    private int detectHermanNum;

    public List<HerdsmanListItem> HerdsmanLists { get => herdsmanLists; set => herdsmanLists = value; }
    public int DetectHermanNum { get => detectHermanNum; set => detectHermanNum = value; }
    public string ApplyCode { get => applyCode; set => applyCode = value; }
    public string InspectDepartmentName { get => inspectDepartmentName; set => inspectDepartmentName = value; }
    public int ActualSampleNum { get => actualSampleSum; set => actualSampleSum = value; }
    public string Creator_phone { get => creator_phone; set => creator_phone = value; }
}
