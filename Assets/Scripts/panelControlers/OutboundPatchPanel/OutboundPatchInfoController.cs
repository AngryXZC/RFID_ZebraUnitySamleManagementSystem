using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;
using Symbol.RFID3;

public class OutboundPatchInfoController : BasePanel
{
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 当前页面信息
    /// </summary>
    private OutboundPatchInfoRes infoRes;
    /// <summary>
    /// 管理员确认刷卡界面
    /// </summary>
    public GameObject administratorConfirmPan;
    /// <summary>
    /// 送检单位
    /// </summary>
    public Text inspectionDepartment;
    /// <summary>
    /// 出库按钮
    /// </summary>
    public Button outboundBtn;
    /// <summary>
    /// 列表父项
    /// </summary>
    public Transform content;
    /// <summary>
    /// 列表子项
    /// </summary>
    public GameObject listItem;
    /// <summary>
    /// 列表所有数据
    /// </summary>
    private List<OutboundSampleItemController> listDatas = new List<OutboundSampleItemController>();
    /// <summary>
    /// 当前批次编号
    /// </summary>
    private string applyCode = string.Empty;
    /// <summary>
    /// 设备引用
    /// </summary>
    GangwayDoorDevice doorDevice;
    /// <summary>
    /// 存储识别的样品的哈希表
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        doorDevice = GangwayDoorDevice.getInstance();
        if (doorDevice.IsConnect)
        {
            doorDevice.StartInventory();
            //注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
       
        if (outboundBtn != null)
        {
            outboundBtn.onClick.AddListener(outboundBtnClick);
        }
        returnBtn.onClick.AddListener(onReturnBtnlick);
        StartCoroutine(UntiyGet(IPAddressConfig.getOutboundPatchInfo, "applyCode=" + applyCode));
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        if (doorDevice.IsConnect)
        {
            doorDevice.StopInventory();
            //取消注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        }
        m_TagTable.Clear();
        listDatas.Clear();
        outboundBtn?.onClick.RemoveListener(outboundBtnClick);
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        applyCode = string.Empty;
        returnBtn.onClick.RemoveAllListeners();
        StopAllCoroutines();
    }
    public void init(string apply_code)
    {
        applyCode = apply_code;

    }
    //使用Unity发送类发送
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("网络错误!");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //重新绑定弹窗的点击事件
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
            stopLoadAni();
        }
        else
        {
            stopLoadAni();
            OutboundInfoRoot outboundInfoRoot = JsonMapper.ToObject<OutboundInfoRoot>(webRequest.downloadHandler.text);
            infoRes = outboundInfoRoot.result;
            infoRes.applyCode = applyCode;
            int num = 0;
            foreach (OutboundHerdsmanListItem item in infoRes.herdsmanList)
            {
                inspectionDepartment.text = "样品来源：" + item.HusbandryBureauName;
                num++;
                GameObject go = Instantiate(listItem);
                OutboundSampleItemController outboundSampleItemController = go.GetComponent<OutboundSampleItemController>();
                outboundSampleItemController.init(num, item);
                listDatas.Add(outboundSampleItemController);
                go.transform.SetParent(content);
            }
            Debug.Log(outboundInfoRoot.result.herdsmanList.Count);
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
    /// <summary>
    /// 出库按钮点击
    /// </summary>
    public void outboundBtnClick()
    {
        Debug.Log("出库按钮点击！" + listDatas.Count);
        int num = 0;
        foreach (var item in listDatas)
        {
            if (item.data.detectSampleNum - item.data.abnormalSampleNum - item.data.hasInspectedNum != 0)
            {
                num++;
               
            }
        }
        alert.setAlertInfo("共有"+num+"牧民数据不全！");
        if (!baseAlertPanel.activeInHierarchy)
        {
            baseAlertPanel.SetActive(true);
            //重新绑定弹窗的点击事件
            alert.returnBtn.onClick.AddListener(() =>alert.hideSelf());
            alert.continueBtn.onClick.AddListener(() =>confirmOut());
        }
        //Messagebox.MessageBox(IntPtr.Zero, "感应数量不正确不允许提交！", "提示框", 0);
        
        Debug.Log("可发送！");
    }
    //确认出库封装
    private void confirmOut() 
    {
        administratorConfirmPan.GetComponent<OutboundPatchConfirm>().initResultInfo(infoRes);
        gameObject.SetActive(false);
        administratorConfirmPan.SetActive(true);
        alert.gameObject.SetActive(false);
    }
    //覆盖
    public new void  initLoadPanel() 
    {
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        loadPanel = GameObject.FindGameObjectWithTag("AnimationPanel").transform.Find("LoadPanel").gameObject;
        baseAlertPanel = GameObject.FindGameObjectWithTag("AlertPanelAnchor").transform.Find("OutAlertPanel").gameObject;
        //初始化时将按钮所有监听事件移除代码中动态指定按钮的回调函数

        if (returnBtn != null)
        {
            returnBtn.onClick.RemoveAllListeners();
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
            foreach (var item in listDatas)
            {
               
                if (item.getAtag(tagID.Substring(1, 15)))
                {
                    m_TagTable.Add(tagID, "1");
                }
            }
        }
    }
}
