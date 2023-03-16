using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanManIdentifyRes 
{
    /// <summary>
    /// 领养人电话号
    /// </summary>
    public string operatorManPhone { get; set; }
    /// <summary>
    /// 领样列表
    /// </summary>
    public List<CleanHerdsmanListItem> herdmanList { get; set; }
    /// <summary>
    /// 库房管理人员
    /// </summary>
    public string warehouseManPhone { get; set; }
}
