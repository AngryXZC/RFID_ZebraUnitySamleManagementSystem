using System.Collections;
using System.Collections.Generic;
public class HerdsmanListItem
{
    /// <summary>
    /// 当前牧户货架编号
    /// </summary>
    public string rackCode { get; set; }

    /// <summary>
    /// 当前牧户袋号
    /// </summary>
    public string bagCode;
    /// <summary>
    /// 该牧民拥有的标签列表
    /// </summary>
    public List<Taginfo> tagCodeList { get; set; }
    /// <summary>
    /// 牧民ID
    /// </summary>
    public string herdsmanId { get; set; }
    /// <summary>
    /// 该牧民拥有的样品数量
    /// </summary>
    public int sampleQuantity { get; set; }
    /// <summary>
    /// 牧民编号
    /// </summary>
    public string herdsmanCode { get; set; }
    /// <summary>
    /// 牧民
    /// </summary>
    public string herdsmanName { get; set; }
    /// <summary>
    /// 牧民缺少的样品数量
    /// </summary>
    public int lackSampleNum{get;set;}
    /// <summary>
    /// 实际输入的数量
    /// </summary>
    public int actualSampleNum { get; set; }
    /// <summary>
    /// 感应样品数量
    /// </summary>
    public int detectSampleNum { get; set; }
    /// <summary>
    /// 缺少耳号数量
    /// </summary>
   public int earCodeLackSampleNum { get; set; }
   /// <summary>
   /// 牧民样品总数
   /// </summary>
    public int herdsmanSampleSum { get; set; }
    /// <summary>
    /// 缺少耳号列表
    /// </summary>
    public List<string> noEarCodeList { get; set; }
}