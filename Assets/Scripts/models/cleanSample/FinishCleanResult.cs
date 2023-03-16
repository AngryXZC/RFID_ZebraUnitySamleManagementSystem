using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCleanResult
{
    /// <summary>
    /// 
    /// </summary>
    public int cleanSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int abnormalBackSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int detectSampleNum { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<CleanSampleListItem> cleanSampleList { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string cleanMan { get; set; }
    /// <summary>
    /// ·®Ô½
    /// </summary>
    public string herdsmanName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int reciveSampleNum { get; set; }
}
