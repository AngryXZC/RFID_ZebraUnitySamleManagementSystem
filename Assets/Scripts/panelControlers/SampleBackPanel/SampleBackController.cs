using System;
using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class SampleBackController : BasePanel
{
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 管理员刷卡确认
    /// </summary>
    public GameObject admiConfirm;

    /// <summary>
    /// 通道门设备
    /// </summary>
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 列表父节点
    /// </summary>
    public Transform content;
    /// <summary>
    /// 列表子项
    /// </summary>
    public GameObject backItem;
    /// <summary>
    /// 完成退样按钮
    /// </summary>
    public Button finishBtn;
    /// <summary>
    /// 退还样品数据信息
    /// </summary>
    private BackSamRes backSamInfo;
    //子项数据绑定
    private List<BackSampleItem> backSampleItemList = new List<BackSampleItem>();
    /// <summary>
    /// 初始化数据
    /// </summary>
    public void initInfo(BackSamRes res)
    {
        
        backSamInfo= res;
        
        generateItem();
    }
    private void generateItem()
    {

        if (backSamInfo != null)
        {
            foreach (var item in backSamInfo.herdmanList)
            {
                GameObject go = Instantiate(backItem);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);
                BackSampleItem temp = go.GetComponent<BackSampleItem>();
                backSampleItemList.Add(temp);
                temp.initItem(backSampleItemList.Count, item, "");
            }

        }
    }

    private void OnDisable()
    {
        backSamInfo = null;
        alert.returnBtn.onClick.RemoveAllListeners();
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
        StopAllCoroutines();
        finishBtn.onClick.RemoveAllListeners();
        backSampleItemList.Clear();
        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        //注册读取事件监听函数
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        
       
    }
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();


        finishBtn.onClick.AddListener(confirmBackBtn);
        doorDevice = GangwayDoorDevice.getInstance();
        if (doorDevice.IsConnect && (!doorDevice.IsInventory))
        {
            doorDevice.StartInventory();
            //注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
    }


    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        bool isSampleNum=DataCheck.isSampleNumber(tagID);
        if (isSampleNum) 
        {
            var tag_data = tagID.Substring(1,15);
            foreach (BackSampleItem item in backSampleItemList)
            {
                item.getATag(tag_data);
            }
        }
       
    }

    public void confirmBackBtn() 
    {
        List<BackHerdsmanListItem> result= new List<BackHerdsmanListItem>();
       
        foreach (BackSampleItem item in backSampleItemList) 
        {
            if (item.myData.detectSampleNum - item.myData.abnormalBackSampleNum - item.myData.cleanSampleNum == 0)
            {

                result.Add(item.myData);
            }
          
        }
        //返回结果列表为空则数据不正确
        if (result.Count <= 0)
        {
            alert.setAlertInfo("没有检测到完整的牧户！");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //重新绑定弹窗的点击事件
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, "没有完整的牧民信息不允许提交！", "提示框", 0);
        }
        else 
        {
           //可以提交
            var temp = admiConfirm.GetComponent<SampleBackAdminConfirm>();
            backSamInfo.herdmanList = result;
            temp.initData(backSamInfo);
           
            this.gameObject.SetActive(false);
            admiConfirm.SetActive(true);

        }
       
    }
}
