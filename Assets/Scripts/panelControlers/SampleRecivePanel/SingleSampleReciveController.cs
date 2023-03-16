using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class SingleSampleReciveController : BasePanel
{
    Coroutine mCoroutine;
    /// <summary>
    /// 接收类型
    /// </summary>
    ReciveEnum reciveType;

    /// <summary>
    /// 袋号选择面板
    /// </summary>
    public GameObject bagCodePanel;
    /// <summary>
    /// 是否正在识别
    /// </summary>
    bool isRecongenizing = false;
    /// <summary>
    /// 当前识别牧民
    /// </summary>
    public Text currentHerdsManText;
    /// <summary>
    /// 当前牧民的样品情况
    /// </summary>
    public Text currentHerdsManSampleText;
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 存储识别的样品的哈希表
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    [System.NonSerialized]
    public GangwayDoorDevice doorDevice;

    /// <summary>
    /// 当前批次信息
    /// </summary>
    private SampleInfo sampleInfo;
    /// <summary>
    /// 当前受理单号
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    /// 样品来源
    /// </summary>
    public Text sampleSourceText;
    /// <summary>
    /// 牧户相关Text
    /// </summary>
    public Text herdsmanNumText;

    /// <summary>
    /// 检测样品相关
    /// </summary>
    public Text sampleNumText;
    /// <summary>
    /// 列表相关
    /// </summary>
    private HerdsmanListItem currentHerdsman;
    /// <summary>
    /// 当前库房的信息
    /// </summary>
    WarehouseAndRackRoot warehouseAndRack;
    /// <summary>
    /// 当前袋号
    /// </summary>
    string bagCode;
    private List<SampleItemController> sampleItemControllers = new List<SampleItemController>();
    /// <summary>
    /// 列表相关
    /// </summary>
    public RectTransform sampleList;
    public GameObject sampleItem;
    /// <summary>
    /// 入库界面
    /// </summary>
    public GameObject warehouseInPanel;
    /// <summary>
    /// 底部按钮
    /// </summary>
    public Button reciveBtn;
    public Button stopCurrenHerdsMantBtn;
    public Button changeBagBtn;
    private void OnEnable()
    {
        //注册按钮点击世界
        reciveBtn.onClick.AddListener(()=>reciveBtnClick());
        stopCurrenHerdsMantBtn.onClick.AddListener(()=>stopCurrentHerdsmanBtnClick());
        changeBagBtn.onClick.AddListener(() => changeBagNumBtnClick());


        mCoroutine=StartCoroutine(scanCurrentHersman());
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        voiceController.startVoiceCoroutine();
        initLoadPanel();
        ///设置alert相关
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, parentPanel));
        //
        returnBtn.onClick.AddListener(onReturnBtnlick);
        doorDevice = GangwayDoorDevice.getInstance();
        if (!doorDevice.IsConnect)
        {
            baseAlertPanel.SetActive(true);
            alert.setAlertInfo("设备未连接请返回菜单页或重新打开应用!");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
        }
        else
        {

            if (!doorDevice.IsInventory)
            {
                doorDevice.StartInventory();
            }

            //注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
    }
    private void Update()
    {
        if (currentHerdsman != null)
        {
            currentHerdsManText.text = currentHerdsman.herdsmanName;
        }
        else 
        {
            currentHerdsManText.text = "未检测到牧民";
            currentHerdsManSampleText.text = "<size=80>0</size>" + "/<size=65>0</size>"; ;
        }
        herdsmanNumText.text = "<size=80>"+sampleItemControllers.Count+"</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
        sampleNumText.text = "<size=80>"+ m_TagTable.Count + "</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
    }
    private void OnDisable()
    {
        //移除按钮点击事件
        reciveBtn.onClick.RemoveAllListeners();
        stopCurrenHerdsMantBtn.onClick.RemoveAllListeners();
        changeBagBtn.onClick.RemoveAllListeners();

        currentHerdsManText.text = "未检测到牧民";
        isRecongenizing =false;
        currentHerdsman = null;
        sampleItemControllers.Clear();
        if (mCoroutine!=null)
        {
            StopCoroutine(mCoroutine);
        }
        currentApplyCode = string.Empty;
        voiceController.stopVoiceCoroutine();
        alert.returnBtn.onClick.RemoveAllListeners();
        doorDevice.StopInventory();
        //取消注册读取事件监听函数
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        m_TagTable.Clear();
        //清除列表内容
        while (sampleList.childCount > 0)
        {
            DestroyImmediate(sampleList.GetChild(0).gameObject);
        }
    }
    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {

        string tagID = e.ReadEventData.TagData.TagID;

        bool isSampleNumber = DataCheck.isSampleNumber(tagID);
        if (m_TagTable.Contains(tagID) || !isSampleNumber)
        {
            return;
        }
        else
        {

            string herdsmanCodeTemp = tagID.Substring(5, 7);
            //构建牧民
            if (sampleInfo != null)
            {
                //对比本批次所有的牧民信息
                foreach (HerdsmanListItem item in sampleInfo.result.herdsmanList)
                {
                    //如果当前标签的牧民编号在这个批次中。并且当前没有在识别任何牧户
                    if (herdsmanCodeTemp == item.herdsmanCode && currentHerdsman == null)
                    {
                        Debug.LogWarning("命中牧户！" + item.herdsmanName);
                        Debug.LogWarning("创建当前识别的牧民项！" + item.herdsmanName);
                        //如果这个牧户以被识别过则直接略过这个标签
                        foreach (SampleItemController item2 in sampleItemControllers) 
                        {
                            if (item2.data.herdsmanCode == herdsmanCodeTemp) 
                            {
                                return;
                            }
                        }
                        currentHerdsman = item;
                    }
                }
                //识别样品，判断这个标签是否在已识别的列表中找到
                foreach (var item in sampleItemControllers)
                {
                    //检测样品标签
                    if (item.detectOneSample(tagID.Substring(1, 15)))
                    {
                        m_TagTable.Add(tagID, "1");
                    }
                }
                //判断当前牧户是否识别完毕
                foreach (var item in sampleItemControllers)
                {

                    if (isRecongenizing && item.data == currentHerdsman && item.hasFinished())
                    {
                        Debug.LogWarning(item.data.herdsmanName + "识别完毕！");
                        currentHerdsman = null;
                        isRecongenizing = false;
                    }
                }
            }


        }
    }

    public void setcurrentData(string apply_code, SampleInfo info,ReciveEnum reciveEnum)
    {
        reciveType = reciveEnum;
        Debug.Log(gameObject.name+reciveType);
        sampleInfo = info;
        currentApplyCode = apply_code;
        //收取数据正常
        if (sampleInfo.success)
        {
            sampleSourceText.text = "样品来源：" + sampleInfo.result.sampleSource;
            herdsmanNumText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
            sampleNumText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
        }

    }

    /// <summary>
    /// 设置当前装入的袋子
    /// </summary>
    public void setCurreneBag(string arg)
    {
         bagCode = arg;
    }
    /// <summary>
    /// 初始化当前库房信息
    /// </summary>
    /// <param name="arg"></param>
    public void setWarehouseInfo(WarehouseAndRackRoot arg)
    {
        warehouseAndRack = arg;
    }
    IEnumerator scanCurrentHersman()
    {
        while (true)
        {
            if (currentHerdsman != null && !isRecongenizing)
            {
                GameObject go = Instantiate(sampleItem);
                SampleItemController sampleItemController = go.GetComponent<SampleItemController>();
                sampleItemController.setInfo(bagCode, currentHerdsman,reciveType);
                go.transform.SetParent(sampleList);
                go.transform.localScale = new Vector3(1, 1, 1);
                sampleItemController.isReconizing = true;
                //go.transform.SetAsFirstSibling();
                sampleItemControllers.Add(sampleItemController);
                isRecongenizing = true;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    /// <summary>
    /// 更改当前牧民信息
    /// </summary>
    /// <param name="currentSampleNum"></param>
    /// <param name="currentSampleSum"></param>
    public void setcurrentHerdsmanText(string currentSampleNum,string currentSampleSum)
    {
        
        currentHerdsManSampleText.text = "<size=80>" + currentSampleNum + "</size>" + "/<size=65>" + currentSampleSum + "</size>";
    }
    //各种按钮点击事件的触发函数

    /// <summary>
    /// 入库按钮点击
    /// </summary>
    public void reciveBtnClick() 
    {
        List<SackData> sacks = new List<SackData>();
        WareHouseInPanelController wareHouseInPanelController=warehouseInPanel.GetComponent<WareHouseInPanelController>();
        foreach (var item in sampleInfo.result.herdsmanList)
        {
            //构建当前牧民该放的袋子
            SackData sackData = new SackData();
            sackData.herdsmanItems=new List<HerdsmanListItem>();
            //当前牧民的实际送来的样品数量=无标签数+检测数量+缺少耳号样品数量
            int samsum = item.actualSampleNum + item.detectSampleNum+item.earCodeLackSampleNum;
            sampleInfo.result.actualSampleSum+=samsum;
            //判断该牧户有没有样品（1.检测到正常样品，2.不在系统中但是有标签的样品，3.无标签的样品）
            if (item.detectSampleNum > 0||item.earCodeLackSampleNum>0||item.actualSampleNum>0) 
            {
                sampleInfo.result.detectHermanNum++;
                sackData.sackCode = item.bagCode;
                sackData.sampleSum = samsum;
                sackData.inspectSamSum = item.sampleQuantity;
                sackData.detectSamSum = item.detectSampleNum;
                sackData.lackSamNum = item.lackSampleNum;
                sackData.noEarcodeSum = item.earCodeLackSampleNum;
                sackData.noTagSamSum = item.actualSampleNum;
                sackData.lackSamNum = item.lackSampleNum;
                sackData.herdsmanItems.Add(item);
                //判断当前这个袋子有没有放东西
                bool newSack = true;
                foreach (var sackItem in sacks)
                {
                    if (sackItem.sackCode == sackData.sackCode)
                    {
                        newSack = false;
                        Debug.Log("是否是新袋子"+newSack);
                        sackItem.sampleSum += sackData.sampleSum;
                        sackItem.inspectSamSum += sackData.inspectSamSum;
                        sackItem.detectSamSum += sackData.detectSamSum;
                        sackItem.lackSamNum += sackData.lackSamNum;
                        sackItem.noEarcodeSum += sackData.noEarcodeSum;
                        sackItem.noTagSamSum += sackData.noTagSamSum;
                        sackItem.herdsmanItems.Add(item);
                    }
                }
                if (newSack) 
                {
                    sacks.Add(sackData);
                }
            }
           
            
        }
        wareHouseInPanelController.initInfo(warehouseAndRack,sampleInfo, sacks);
        gameObject.SetActive(false);
        warehouseInPanel.SetActive(true);
    }
    /// <summary>
    /// 停止当前牧户识别
    /// </summary>
    public void stopCurrentHerdsmanBtnClick() 
    {
        Debug.Log("停止识别当前牧户");
        currentHerdsman=null;
        isRecongenizing=false;
    }
    /// <summary>
    /// 更换袋号
    /// </summary>
    public void changeBagNumBtnClick()
    {
        doorDevice.StopInventory();
        bagCodePanel.SetActive(true);
        SelectRackAndBagPanelController selectRack=bagCodePanel.GetComponent<SelectRackAndBagPanelController>();    
        selectRack.confirmBtn.onClick.RemoveAllListeners();
        selectRack.confirmBtn.onClick.AddListener(()=>selectRack.changeBagByClickConfirm());
        selectRack.parentPanel = this.gameObject; 
    }
}
