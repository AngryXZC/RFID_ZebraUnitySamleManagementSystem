using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRackPanelController : BasePanel
{
    /// <summary>
    /// 确定按钮
    /// </summary>
    public Button confirmBtn;
    /// <summary>
    /// 当前单个牧户选择
    /// </summary>
    WarehouseInItemController herd;
    /// <summary>
    /// 下拉框
    /// </summary>
    public Dropdown dropdown;
    /// <summary>
    /// 库房信息
    /// </summary>
    WarehouseAndRackRoot warehouseAndRackRoot;
    List<string> racks = new List<string>();
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        returnBtn.onClick.AddListener(() => onReturnBtnlick());
    }

    private void OnDisable()
    {
     
        confirmBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveAllListeners();
    }
    /// <summary>
    /// 在单户接受的界面就初始化货架的参数
    /// </summary>
    public void initInfo(WarehouseAndRackRoot args) 
    {
        racks.Clear();
        dropdown.options.Clear();
        warehouseAndRackRoot = null;
        Debug.Log("Lit"+LitJson.JsonMapper.ToJson(args));
        warehouseAndRackRoot = args;
       
        foreach (var item in warehouseAndRackRoot.result.rack)
        {
            racks.Add(item.rackTitle);
        }
        dropdown.AddOptions(racks);
    }
    /// <summary>
    /// 从牧户进入选择
    /// </summary>
    public void herdsmanItemIn(WarehouseInItemController herdsItem) 
    {
        herd = herdsItem;
        confirmBtn.onClick.AddListener(()=>singleHerdManCOnfirmCilick());
    }
    /// <summary>
    /// 单户选择货架
    /// </summary>
    public void singleHerdManCOnfirmCilick() 
    {
        herd.rackCode.text = dropdown.captionText.text;
        herd.data.rackCode = dropdown.captionText.text;
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 多选了牧户进入
    /// </summary>
    /// <param name="warehouseInItems"></param>
    public void multiHerdsmanIn(List<WarehouseInItemController> warehouseInItems) 
    {
        confirmBtn.onClick.AddListener(() => multiHerdsmanConfirmCilick(warehouseInItems));
    }
    public void multiHerdsmanConfirmCilick(List<WarehouseInItemController> warehouseInItems) 
    {
        foreach (var item in warehouseInItems)
        {
            item.data.rackCode=dropdown.captionText.text;
            item.rackCode.text=dropdown.captionText.text;
        }
        gameObject.SetActive(false);
    }
}
