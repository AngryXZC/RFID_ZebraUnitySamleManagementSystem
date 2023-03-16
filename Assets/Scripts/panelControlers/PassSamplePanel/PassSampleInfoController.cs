using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PassSampleInfoController : BasePanel
{
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    //完成按钮
    public Button finishBtn;
    public GameObject deliverManPanel;
    /// <summary>
    /// 存储识别的样品的哈希表
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private GangwayDoorDevice doorDevice;
    //列表数据结果
    private PassSampleRes samplePassInfo;
    //列表子项
    public GameObject listItem;
    //列表父项
    public Transform content;
    //列表数据绑定
    private List<PassSampleItemController> passSampleItems = new List<PassSampleItemController>();
    private void OnEnable()
    {
       
        finishBtn.onClick.AddListener(()=> onFinishBtnClick());
        initLoadPanel();
        //初始化警告框
        alert = baseAlertPanel.GetComponent<AlertController>();
        doorDevice = GangwayDoorDevice.getInstance();
        if (doorDevice.IsConnect)
        {
            doorDevice.StartInventory();
            //注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        alert.returnBtn.onClick.RemoveAllListeners();
        finishBtn.onClick.RemoveAllListeners();
        passSampleItems.Clear();
        m_TagTable.Clear();
        //同步销毁
        while(content.childCount!=0)
        {
            
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        //异步销毁
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    Destroy(content.GetChild(i).gameObject);
        //}
        samplePassInfo=null;
        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
            //取消注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        }
    }
    
    public void  bindInfo(PassSampleRes res) 
    {
        
        this.samplePassInfo=res;

        foreach (PassSampleListItem item in samplePassInfo.sampleList)
        {
            GameObject go = Instantiate(listItem);
            go.transform.SetParent(content);
            go.transform.localScale = new Vector3(1, 1, 0);
            PassSampleItemController temp = go.GetComponent<PassSampleItemController>();
            passSampleItems.Add(temp);
            temp.init(passSampleItems.Count.ToString(), item, samplePassInfo.recivemanName);
        }
    }

    


    public void onFinishBtnClick() 
    {
        List<PassSampleListItem> result=new List<PassSampleListItem>();
        
        if (passSampleItems.Count <= 0) 
        {
            alert.setAlertInfo("不允许提交空数据！");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, "不允许提交空数据！！", "提示框", 0);
            return;
        }
        foreach (var item in passSampleItems)
        {
            if(item.data.detectSampleNum + item.data.abnormalBackNum == item.data.cleanSampleCount)
            {
                //TODO
                result.Add(item.data);
            }
          
        }
        if (result.Count>0)
        {
            samplePassInfo.sampleList=result;
            //deliverManPanel.GetComponent<DeliverConfirm>().setInfo(samplePassInfo);
            Debug.Log("可以提交！");
            //gameObject.SetActive(false);
            //deliverManPanel.SetActive(true);
            Debug.Log(samplePassInfo.sampleList.Count);
            string json = Regex.Unescape(JsonMapper.ToJson(samplePassInfo));
            StartCoroutine( UntiyPost(IPAddressConfig.sampleDeliverConfirm,json));
        }
        else
        {
            alert.setAlertInfo("没有感应到完整牧户！");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
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

            //添加标签
            m_TagTable.Add(tagID, "1");
            if (samplePassInfo != null)
            {
                if (passSampleItems.Count > 0)
                {
                    foreach (var item in passSampleItems)
                    {
                        item.getAtag(tagID.Substring(1, 15));
                    }
                }
            }
        }
    }



    public IEnumerator UntiyPost(string url, string postDataStr)
    {
      
        Debug.Log(postDataStr);
        playLoadAni();
        Debug.Log("协程被调用！");
        Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
        parameters.Add("params", postDataStr);
        byte[] postData = Encoding.UTF8.GetBytes(NetTools.BuildQuery(parameters, "utf8")); //使用utf-8格式组装post参数
        //byte[] databyte = Encoding.UTF8.GetBytes(jsondata);
        UnityWebRequest _request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

        _request.uploadHandler = new UploadHandlerRaw(postData);
        _request.downloadHandler = new DownloadHandlerBuffer();
        _request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
        yield return _request.SendWebRequest();
        if (_request.result == UnityWebRequest.Result.ProtocolError || _request.result == UnityWebRequest.Result.ConnectionError)
        {
            alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
            alert.setAlertInfo("网络请求错误！");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
            }
            Debug.LogError(_request.error);
            //Messagebox.MessageBox(IntPtr.Zero, _request.error, "提示框", 0);
            stopLoadAni();
        }
        else
        {
            stopLoadAni();
            alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
            alert.setAlertInfo("领样成功！！");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
            }

            Debug.Log(_request.downloadHandler.text);

        }
        StopAllCoroutines();

    }
}
