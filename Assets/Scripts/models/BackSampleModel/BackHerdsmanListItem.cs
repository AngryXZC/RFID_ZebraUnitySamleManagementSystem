using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHerdsmanListItem 
{
    /// <summary>
    /// 
    /// </summary>
    public string applyId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int cleanSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string sampleCheckId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string herdsmanId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int detectSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int abnormalBackSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<BackSamListItem> cleanSampleList { get; set; }
    /// <summary>
    /// ������
    /// </summary>
    public string cleanMan { get; set; }
    /// <summary>
    /// �����������
    /// </summary>
    public string herdsmanName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int reciveSampleNum { get; set; }
    /// <summary>
    /// ������Ա
    /// </summary>
    public string inspectMan { get; set; }
}
