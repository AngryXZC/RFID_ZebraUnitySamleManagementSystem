using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountSampleResult
{
    /// <summary>
    /// 
    /// </summary>
    public int tobeCleanSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int detectSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int tobeInspectSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int abnormalSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<CountSampleItem> reciveSampleList { get; set; }
    /// <summary>
    /// ·®Ô½
    /// </summary>
    public string herdsmanName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int tobeBackSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int reciveSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int lackEarSampleNum { get; set; }
}