using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class LedSamController : BasePanel
{
#pragma warning disable 0414
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    private AlertController alert;
    bool isStartCoroutine = true;
    /// <summary>
    /// 通道门设备
    /// </summary>
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 存储识别的样品的哈希表
    /// </summary>

    private Hashtable m_TagTable = new Hashtable();
    /// <summary>
    /// 待领取的样品信息
    /// </summary>
    private CleanManIdentifyRes ledInfo;
    /// <summary>
    /// 开始领取按钮
    /// </summary>
    public Button ledSampleBtn;
    /// <summary>
    /// 列表子项
    /// </summary>
    public GameObject ledSampleItem;
    /// <summary>
    /// 列表父节点
    /// </summary>
    public Transform content;
    /// <summary>
    /// 子项计数
    /// </summary>
    private int number = 0;
    /// <summary>
    /// 列表
    /// </summary>
    private List<LedSamItem> ledSamItems = new List<LedSamItem>();
    /// <summary>
    /// 库房管理人员确认面板
    /// </summary>
    public GameObject warehouseManPanel;
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(()=>onReturnBtnlick());
        doorDevice = GangwayDoorDevice.getInstance();
    }
    private void Update()
    {
        Debug.Log(m_TagTable.Count);
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveAllListeners();
        number = 0;
        //同步销毁
        while (content.childCount!=0) 
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        //异步销毁
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    Destroy(content.GetChild(i).gameObject);
        //}
        m_TagTable.Clear();
        //取消注册读取事件监听函数
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        isStartCoroutine = true;
        ledInfo = null;
        
        ledSamItems.Clear();
       
        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        ledSampleBtn.GetComponentInChildren<Text>().text = "开始领样";
    }
    // Update is called once per frame

    public void ledSamBtnClick()
    {
        if (ledSampleBtn.GetComponentInChildren<Text>().text == "开始领样")
        {
            ledSampleBtn.GetComponentInChildren<Text>().text = "完成领样";
            if (doorDevice.IsConnect && (!doorDevice.IsInventory))
            {
                doorDevice.StartInventory();

                //注册读取事件监听函数
                doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
            }

        }
        else
        {
            List<CleanHerdsmanListItem> result=new List<CleanHerdsmanListItem>();
            //doorDevice.StopInventory();
            ////取消注册读取事件监听函数该处不加这句了，加上之后会有问题。问题原因目前还不清楚
            //doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
            Debug.Log("校验数据！");
            foreach (LedSamItem item in ledSamItems)
            {
                if (item.data.detectSampleNum == item.data.reciveSampleNum)
                {
                    Debug.Log("该户纳入！！");
                    result.Add(item.data);
                }
            }
            if (result.Count<=0)
            {
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                alert.setAlertInfo("没有检测到完整的牧户信息！");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                return;
                //Messagebox.MessageBox(IntPtr.Zero, "感应数量数不正确！", "提示框", 0);
            }
            //if (content.childCount <= 0)
            //{
            //    alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            //    alert.setAlertInfo("不允许提交空数据！");
            //    if (!baseAlertPanel.activeInHierarchy)
            //    {
            //        baseAlertPanel.SetActive(true);
            //    }
            //    //Messagebox.MessageBox(IntPtr.Zero, "不允许提交空数据！", "提示框", 0);
            //    Debug.Log("数据不正确！");
            //    return;
            //}
            Debug.Log("数据正确！");
            ///库房管理人员刷卡
            ///ledInfo发送过去
            ledInfo.herdmanList = result;
            warehouseManPanel.GetComponent<LedSamWarehouseConf>().setInfo(ledInfo);
            gameObject.SetActive(false);
            warehouseManPanel.SetActive(true);

        }
    }
    public void init(CleanManIdentifyRes result)
    {

        ledInfo = result;
        //设置列表
        foreach (var item in ledInfo.herdmanList)
        {
            number++;
            GameObject go = Instantiate(ledSampleItem);
            go.transform.SetParent(content);
            go.transform.localScale = new Vector3(1, 1, 0);
            LedSamItem temp = go.GetComponent<LedSamItem>();
            temp.setInfo(number.ToString(), item);
            ledSamItems.Add(temp);
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
            foreach (LedSamItem ledSamItem in ledSamItems)
            {

                if (ledSamItem.getATag(tagID.Substring(1, 15)))
                {
                    m_TagTable.Add(tagID, "1");
                }


            }

        }
    }
}
