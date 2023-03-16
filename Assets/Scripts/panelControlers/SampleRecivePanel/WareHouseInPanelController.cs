using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class WareHouseInPanelController : BasePanel
{
    /// <summary>
    /// 库房管理员确认刷卡
    /// </summary>
    public GameObject confirmPanel;
    /// <summary>
    /// 入库按钮
    /// </summary>
    public Button warehouseInBtn;
    /// <summary>
    /// 警告框
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 货架选择面板
    /// </summary>
    public GameObject rackSelectPanel;
    private SelectRackPanelController selectRackPanelController;

    /// <summary>
    /// 入库牧户总数量
    /// </summary>
    public Text warehouseInHersmanSum;
    /// <summary>
    /// 入库样品总数量
    /// </summary>
    public Text warehouseInSampleSum;
    /// <summary>
    /// 当前送检单位
    /// </summary>
    public Text currentInpectedDepartment;
    /// <summary>
    /// 当前检验批次
    /// </summary>
    public Text currentInspectPatchName;
    /// <summary>
    /// 列表相关
    /// </summary>
    public Transform content;
    public GameObject sackItem;
    private List<SackData> sackDatas;
    /// <summary>
    /// 库房信息和当前接收样品的信息
    /// </summary>
    WarehouseAndRackRoot warehouseAndRack;
    SampleInfo sampleInfo;
    /// <summary>
    /// 批量选择货架
    /// </summary>
    public Button multiSelectRackBtn;
    /// <summary>
    /// 入库按钮
    /// </summary>
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
        multiSelectRackBtn.onClick.AddListener(() => multiHerdsRackBtnClick());
        warehouseInBtn.onClick.AddListener(() => sampleWarehouseIn());
        returnBtn.onClick.AddListener(() => onReturnBtnlick());
    }


    private void OnDisable()
    {
        sampleInfo = null;
        warehouseInBtn.onClick.RemoveAllListeners();
        sackDatas.Clear();
        if (alert.returnBtn.onClick != null)
        {
            alert.returnBtn.onClick.RemoveAllListeners();
        }


        multiSelectRackBtn.onClick.RemoveAllListeners();
        while (content.childCount != 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        returnBtn.onClick.RemoveAllListeners();
    }
    /// <summary>
    /// 设置入库的初始化信息
    /// </summary>
    public void initInfo(WarehouseAndRackRoot warehouseArg, SampleInfo sampleInfoArg, List<SackData> sacks)
    {
        sackDatas = sacks;
        sampleInfo = sampleInfoArg;
        warehouseAndRack = warehouseArg;
        int samsum = 0;
        foreach (var item in sacks)
        {
            samsum += item.sampleSum;
            GameObject go = GameObject.Instantiate(sackItem);
            go.GetComponent<WarehouseInItemController>().initInfo(item);
            go.transform.SetParent(content);
            go.transform.localScale = new Vector3(1, 1, 1);

        }
        warehouseInHersmanSum.text = sampleInfoArg.result.detectHermanNum.ToString();
        warehouseInSampleSum.text = samsum.ToString();
        currentInpectedDepartment.text = sampleInfoArg.result.sampleSource;
        currentInspectPatchName.text = sampleInfoArg.result.patchTitle;
        selectRackPanelController = rackSelectPanel.GetComponent<SelectRackPanelController>();
        selectRackPanelController.initInfo(warehouseArg);
        Debug.Log("入库" + Regex.Unescape(LitJson.JsonMapper.ToJson(sampleInfo)));
        Debug.Log("库房信息" + Regex.Unescape(LitJson.JsonMapper.ToJson(warehouseAndRack)));
        Debug.Log("摆放结果：" + Regex.Unescape(LitJson.JsonMapper.ToJson(sacks)));
    }
    /// <summary>
    /// 批量选择货架
    /// </summary>
    public void multiHerdsRackBtnClick()
    {
        /// <summary>
        /// 用来存储当前有多少牧民被选取
        /// </summary>
        List<WarehouseInItemController> warehouseInItems = new List<WarehouseInItemController>();
        for (int i = 0; i < content.childCount; i++)
        {
            WarehouseInItemController temp = content.GetChild(i).GetComponent<WarehouseInItemController>();
            if (temp.selectToggle.isOn)
            {
                warehouseInItems.Add(temp);
            }
        }
        if (warehouseInItems.Count < 1)
        {
            alert = baseAlertPanel.GetComponent<AlertController>();
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            alert.setAlertInfo("请选择至少一个袋子来将其放入货架！");
            baseAlertPanel.SetActive(true);
            return;
        }
        selectRackPanelController.multiHerdsmanIn(warehouseInItems);
        //显示选择货架面板
        rackSelectPanel.SetActive(true);
    }

    /// <summary>
    /// 根据一个牧民的信息发生变化重新渲染这个界面
    /// </summary>
    public void reloadInfo()
    {
        while (content.childCount != 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        ///遍历所有袋子
        //////默认没有发生变化
        bool hasChange = false;
        int originalScale = sackDatas.Count;
        for (int i = 0; i < originalScale; i++)
        {
            var item = sackDatas[i];
            for (int j = 0; j < item.herdsmanItems.Count; j++)
            {
                var item2 = item.herdsmanItems[j];
                if (item2.bagCode != item.sackCode)
                {
                    hasChange = true;
                    // 更换了袋子
                    //构建一个新的袋子用来装这个牧户
                    //这个新构建出来的袋子还要与原有的数据进行比对如果有重复的就要数据合并
                    SackData sackData = new SackData();
                    sackData.sampleSum = item2.actualSampleNum + item2.detectSampleNum + item2.earCodeLackSampleNum;
                    sackData.sackCode = item2.bagCode;
                    sackData.noEarcodeSum = item2.earCodeLackSampleNum;
                    sackData.inspectSamSum = item2.sampleQuantity;
                    sackData.noTagSamSum = item2.actualSampleNum;
                    sackData.lackSamNum = item2.lackSampleNum;
                    sackData.detectSamSum = item2.detectSampleNum;
                    sackData.herdsmanItems=new List<HerdsmanListItem>();
                    sackData.herdsmanItems.Add(item2);
                    //从这个袋子中剔除这家牧户
                    //设置原有袋子的数据
                    item.sampleSum -= sackData.sampleSum;
                    item.inspectSamSum -= sackData.inspectSamSum;
                    item.detectSamSum -= sackData.detectSamSum;
                    item.lackSamNum -= sackData.lackSamNum;
                    item.noEarcodeSum -= sackData.noEarcodeSum;
                    item.noTagSamSum -= sackData.noTagSamSum;
                    item.herdsmanItems.Remove(item2);
                    //增加一个袋子
                    sackDatas.Add(sackData);
                    

                }
            }
        }
        if (hasChange)
        {
            Debug.Log("袋号发生变化！"+sackDatas.Count);
            //移除已经装完的袋子
            for (int i = 0; i < sackDatas.Count; i++)
            {
                if (sackDatas[i].sampleSum==0)
                {
                    sackDatas.RemoveAt(i);
                }
            }
            //先进行合并
            combineSack(sackDatas);
            //展示列表、
            int samsum = 0;
            foreach (var item in sackDatas)
            {
                samsum += item.sampleSum;
                GameObject go = GameObject.Instantiate(sackItem);
                go.GetComponent<WarehouseInItemController>().initInfo(item);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);

            }

        }
        else
        {
            int samsum = 0;
            foreach (var item in sackDatas)
            {
                samsum += item.sampleSum;
                GameObject go = GameObject.Instantiate(sackItem);
                go.GetComponent<WarehouseInItemController>().initInfo(item);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);

            }
        }
        Debug.Log(LitJson.JsonMapper.ToJson(sackDatas));
    }

    /// <summary>
    /// 合并相同的项
    /// </summary>
    private  void combineSack (List<SackData> sacks)
    {
       
        for (int i = 0; i < sacks.Count; i++)
        {
            
            for (int j = i+1; j < sacks.Count; j++)
            {
                if (sacks[i].sackCode==sacks[j].sackCode)
                {
                  
                  //合并操作
                    sacks[i].sampleSum+=sacks[j].sampleSum;
                    sacks[i].inspectSamSum+=sacks[j].inspectSamSum;
                    sacks[i].detectSamSum+=sacks[j].detectSamSum;
                    sacks[i].lackSamNum+=sacks[j].lackSamNum;
                    sacks[i].noEarcodeSum+=sacks[j].noEarcodeSum;
                    sacks[i].noTagSamSum+=sacks[j].noTagSamSum;
                    sacks[i].rackCode = null;
                    sacks[i].herdsmanItems=sacks[i].herdsmanItems.Concat(sacks[j].herdsmanItems).ToList();
                    sacks.Remove(sacks[j]);
                    Debug.Log("合并");
                }
            }
        } 
     
    }
    /// <summary>
    /// 样品入库
    /// </summary>
    public void sampleWarehouseIn() 
    {
        Debug.Log("入库" + Regex.Unescape(LitJson.JsonMapper.ToJson(sampleInfo)));
        Debug.Log("摆放结果：" + Regex.Unescape(LitJson.JsonMapper.ToJson(sackDatas)));
        foreach (var item in sampleInfo.result.herdsmanList)
        {
            foreach (var item2 in sackDatas)
            {
                if (item.bagCode==item2.sackCode)
                {
                    item.rackCode = item2.rackCode;
                }
            }
        }
        ConfirmPanelController confirmPanelController= confirmPanel.GetComponent<ConfirmPanelController>();
        confirmPanelController.initPanel(sampleInfo.result);
        this.gameObject.SetActive(false);
        confirmPanel.gameObject.SetActive(true);
      
    }
}
