using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SampleReciveController : BasePanel
{
    public static int detectSum = 0;
    ///
    private ReciveEnum reciveType;
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 缺少样品详情
    /// </summary>
    public Button lackSampleBtn;
    public GameObject lackSamPanel;
    /// <summary>
    /// 识别中图片
    /// </summary>
    public Sprite recongnizeSprite;
    public Sprite recongnizeLittleSprite;
    /// <summary>
    /// 缺少样品的图片
    /// </summary>
    public Sprite lackSprite;
    public Sprite lackLittleSprite;
    /// <summary>
    /// 数量正确的Sprite
    /// </summary>
    public Sprite rightSprite;
    public Sprite rightLittleSprite;
    /// <summary>
    /// 当前批次信息
    /// </summary>
    private SampleInfo sampleInfo;
    /// <summary>
    /// 当前受理单号
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    ///库管刷卡确认界面
    /// </summary>
    //public GameObject confirmpanel;
    
    ///<summary>
    ///入库面版
    ///</summary>
    public GameObject warehouseInPanel;
    /// <summary>
    /// 样品来源
    /// </summary>
    public Text sampleSourceText;
    /// <summary>
    /// 牧户相关Text
    /// </summary>
    public Text herdsmanText;
    private Hashtable currentHerdsManCodeTable = new Hashtable();
    /// <summary>
    /// 牧户bg
    /// </summary>
    public Image herdsmanImage;
    public Image herdsmanLittleBG;
    /// <summary>
    /// 检测样品相关
    /// </summary>
    public Text sampleText;
    /// <summary>
    /// 样品图片
    /// </summary>
    public Image sampleImage;
    public Image sampleLittleBG;
    /// <summary>
    /// 缺少样品的数量
    /// </summary>
    public Text lackSampleNumText;
    /// <summary>
    /// 缺少样品图片显示
    /// </summary>
    public Image lackImage;
    public Image lackImageLittleBG;
    /// <summary>
    /// 存储识别的样品的哈希表
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 列表相关
    /// </summary>
    public RectTransform sampleList;
    public GameObject sampleItem;
    private List<SampleItemController> sampleItemControllers = new List<SampleItemController>();
    private int num = 0;
   
    /// <summary>
    /// 初始化数据并开始盘点
    /// </summary>
    private void OnEnable()
    {
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        voiceController.startVoiceCoroutine();
        lackSampleBtn.onClick.AddListener(()=>this.lackBtnOnclick());
        changeSprite(lackSprite,lackLittleSprite);
        initLoadPanel();
        ///设置alert相关
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(()=>alert.patchAlert(gameObject,parentPanel));
        //
        returnBtn.onClick.AddListener(onReturnBtnlick);
        doorDevice = GangwayDoorDevice.getInstance();
        if (!doorDevice.IsConnect)
        {
            baseAlertPanel.SetActive(true);
            alert.setAlertInfo("设备未连接请返回菜单页或重新打开应用!");
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            //Messagebox.MessageBox(IntPtr.Zero, "设备未连接请返回菜单页或重新打开应用！", "提示", 0);
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
    private void OnDisable()
    {
        detectSum = 0;
        voiceController.stopVoiceCoroutine();
        lackSampleBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        sampleInfo = null;
        currentApplyCode = string.Empty;
        doorDevice.StopInventory();
        //取消注册读取事件监听函数
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        m_TagTable.Clear();
        sampleItemControllers.Clear();
        currentHerdsManCodeTable.Clear();
        //清除列表内容
        while (sampleList.childCount>0)
        {
            DestroyImmediate(sampleList.GetChild(0).gameObject);
        }
        num = 0;
    }
    // Update is called once per frame
    void Update()
    {
       
        if (sampleInfo != null) 
        {
            sampleText.text = "<size=65>" + m_TagTable.Count.ToString() + "</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
            sampleInfo.result.actualSampleSum = m_TagTable.Count;
            int temp= sampleInfo.result.sampleQuantitys - detectSum;
            if (temp < 0)
            {
                lackSampleNumText.text = 0.ToString();
            }
            else 
            {
                lackSampleNumText.text=temp.ToString();
            }
            herdsmanText.text= "<size=65>"+currentHerdsManCodeTable.Count.ToString()+"</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
            //感应到样品
            if (m_TagTable.Count > 0)
            {
                changeSprite(recongnizeSprite, recongnizeLittleSprite);
                sampleText.color = Style.blue;
                lackSampleNumText.color = Style.blue;
                herdsmanText.color = Style.blue;
                
                lackSampleBtn.enabled = true;
            }
            else 
            {
                sampleText.color = Color.red;
                lackSampleNumText.color = Color.red;
                herdsmanText.color = Color.red;
               
            }
            //牧户数量正确
            if (sampleInfo.result.herdsmanQuantity==currentHerdsManCodeTable.Count)
            {
                herdsmanImage.sprite = rightSprite;
                herdsmanLittleBG.sprite = rightLittleSprite;
                herdsmanText.color = Color.green;
            }
            //样品数量正确
            if (sampleInfo.result.sampleQuantitys<=m_TagTable.Count)
            {
                changeSprite(rightSprite,rightLittleSprite);
                sampleImage.sprite = rightSprite;
                lackImage.sprite= rightSprite;
                lackSampleNumText.color = Color.green;
                sampleText.color = Color.green;
                lackSampleBtn.enabled = false;
            }
        }
        
    }

    //接收样品提交按钮
    public void reciveSampleBtn()
    {
        
        
        sampleInfo.result.applyCode= currentApplyCode;
        sampleInfo.result.detectHermanNum = currentHerdsManCodeTable.Count; 
        sampleInfo.result.actualSampleSum= m_TagTable.Count;
        //激活面板
        // ConfirmPanelController confirmPanelController = confirmpanel.GetComponent<ConfirmPanelController>();
        // confirmPanelController.initPanel(sampleInfo.result);
        MultiWarehouseInPanel multiWarehouseInPanel = warehouseInPanel.GetComponent<MultiWarehouseInPanel>();
        multiWarehouseInPanel.initPanel(sampleInfo.result);
        //此处顺序不能乱先隐藏
        //this.gameObject.SetActive(false);
        doorDevice.StopInventory();
        warehouseInPanel.SetActive(true);
        
        //confirmpanel.gameObject.SetActive(true);
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
            //是正确的的样品编号且当前列表中不包含这个数据
            foreach (var item in sampleItemControllers)
            {
                //检测样品标签
                if (item.detectOneSample(tagID.Substring(1, 15))) 
                {
                    m_TagTable.Add(tagID, "1");
                    string herdsmanCodeTemp = tagID.Substring(5,7);
                    if (item.data.herdsmanCode== herdsmanCodeTemp)
                    {
                        
                        if (!currentHerdsManCodeTable.Contains(herdsmanCodeTemp) )
                        {
                            currentHerdsManCodeTable.Add(herdsmanCodeTemp, "2");
                        }
                     
                    }
                }
               
            }

        }
    }

    public void setcurrentData(string apply_code,string patchTitle, SampleInfo info,ReciveEnum reciveEnum)
    {
        reciveType = reciveEnum;
        Debug.Log(gameObject.name+reciveType);
        sampleInfo= info;
        sampleInfo.result.patchTitle=patchTitle;
        currentApplyCode = apply_code;
        //收取数据正常
        
        if (sampleInfo.success)
        {
            
            sampleSourceText.text = "样品来源：" + sampleInfo.result.sampleSource;
            Debug.Log(sampleInfo.result.sampleSource);
            herdsmanText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
            sampleText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
            lackSampleNumText.text = sampleInfo.result.sampleQuantitys.ToString();
           
            foreach (var item in sampleInfo.result.herdsmanList)
            {
                ++num;
                GameObject go = Instantiate(sampleItem);
                SampleItemController sampleItemController = go.GetComponent<SampleItemController>();
                sampleItemController.setInfo(num.ToString(), item,reciveType);
                sampleItemController.isReconizing = true;
                go.transform.SetParent(sampleList);
                go.transform.localScale = new Vector3(1, 1, 1);
                sampleItemControllers.Add(sampleItemController);
            }
        }

    }
   

    /// <summary>
    /// 改变界面中几个背景图
    /// </summary>
    /// <param name="currentSprite"></param>
    public void changeSprite(Sprite currentSprite,Sprite currentLittleSprite)
    {
        herdsmanImage.sprite = currentSprite;
        herdsmanLittleBG.sprite = currentLittleSprite;
        sampleImage.sprite = currentSprite;
        sampleLittleBG.sprite = currentLittleSprite;
        lackImage.sprite = currentSprite;
        lackImageLittleBG.sprite = currentLittleSprite;
    }
    /// <summary>
    /// 缺少样品详情点击
    /// </summary>
    public void lackBtnOnclick() 
    {
        Debug.Log("点击查看缺少牧户！");
        
        ReciveLackSamplePanel reciveLackSamplePanel=lackSamPanel.GetComponent<ReciveLackSamplePanel>();
        reciveLackSamplePanel.initData(sampleInfo.result);
        doorDevice.StopInventory();
        lackSamPanel.SetActive(true);
    } 
}
