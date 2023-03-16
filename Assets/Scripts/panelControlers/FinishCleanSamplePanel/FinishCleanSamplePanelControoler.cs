using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.Networking;
using System;
/// <summary>
/// 数据处理放在协程里更新Item放在Update里保证其实时性
/// </summary>
public class FinishCleanSamplePanelControoler : BasePanel
{

    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 完成清洗
    /// </summary>
    public Button finishCleanBtn; 
    /// <summary>
    /// 需要识别的数据信息
    /// </summary>
    private CleanManIdentifyRes finishCleanInfo;
    //子项
    public GameObject sampleItem;
    //列表父物体
    public Transform content;
    //子项数据绑定
    private List<CleanItem> finishCleanItems = new List<CleanItem>();
    //设备
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 存储识别的样品的哈希表
    /// </summary>
    private Hashtable m_TagTable;

    private void OnEnable()
    {
        //同步销毁
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    DestroyImmediate(content.GetChild(0).gameObject);
        //}
        //异步销毁
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    Destroy(content.GetChild(i).gameObject);
        //}
        initLoadPanel();
        //初始化警告框
        alert = baseAlertPanel.GetComponent<AlertController>();
        //初始化列表数据
        finishCleanInfo = null;
        //绑定返回按钮
        this.returnBtn.onClick.AddListener(this.onReturnBtnlick);
        //打开通道门存盘功能
        doorDevice = GangwayDoorDevice.getInstance();
        
        if (doorDevice.IsConnect)
        {
            doorDevice.StartInventory();
            //注册读取事件监听函数
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
        m_TagTable = new Hashtable();
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        //取消注册读取事件监听函数
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);

        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        m_TagTable.Clear();
        Debug.Log(content.childCount);
        //同步销毁
        for (; content.childCount!=0; )
        {
            Debug.Log("同步销毁！"+content.childCount);
            DestroyImmediate(content.GetChild(0).gameObject,true);
        }
        //异步销毁
        //for (int i = 0; i < content.childCount; i++)
        //{

        //    Destroy(content.GetChild(i).gameObject);
        //}
        finishCleanItems.Clear();
        
    }
    

    
    public void setFinishCleanRootInfo(CleanManIdentifyRes te)
    {
        this.finishCleanInfo = te;
        generateItem();
    }
    private void generateItem()
    {
        
        if (finishCleanInfo != null)
        {
            foreach (CleanHerdsmanListItem item in finishCleanInfo.herdmanList)
            {
                GameObject go = Instantiate(sampleItem);
                go.transform.SetParent(content);
                CleanItem temp=go.GetComponent<CleanItem>();
                finishCleanItems.Add(temp);
                temp.initItem(finishCleanItems.Count, item, "");
            }

        }
    }
    public void  finsihCleanBtn() 
    {
        bool hasError = false;
        List< CleanHerdsmanListItem> resList=new List< CleanHerdsmanListItem>();
        Debug.Log("点击提交按钮！");
        Debug.Log(content.childCount);
        if (content.childCount<=0) 
        {
            hasError = true;
            alert.setAlertInfo("不允许提交空数据！");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            Debug.Log("不允许提交空数据");
           
            return;
        }
        foreach (CleanHerdsmanListItem item in finishCleanInfo.herdmanList)
        {

            if (item.detectSampleNum + item.abnormalBackSampleNum != item.cleanSampleNum)
            {
               //移除这家牧民 
                continue;
            }
            else 
            {
                resList.Add(item);
            }
        }
        Debug.Log(hasError);
        //发生错误
        if (hasError || resList.Count==0)
        {
            alert.setAlertInfo("核对完成清洗感应数量！");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            Debug.Log("核对完成清洗感应数量");
            
        }
        else 
        {
            finishCleanInfo.herdmanList=resList;
            string json = JsonMapper.ToJson(finishCleanInfo);
            json = Regex.Unescape(json);
            Debug.Log("发送清洗数量"+json);
            StartCoroutine(UntiyPost(IPAddressConfig.submitFinishClean,json));
        }
        

    }
  
  
    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        bool isSampleNumber = DataCheck.isSampleNumber(tagID);
        if (!isSampleNumber || m_TagTable.Contains(tagID))
        {
            return;
        }
        else 
        {

            //识别到一个标签
            foreach (CleanItem item in finishCleanItems)
            {
                
                if (item.getATag(tagID.Substring(1, 15))&&!m_TagTable.Contains(tagID))
                {
                    m_TagTable.Add(tagID, "1");
                }

            }
        }
       
    }

    /// <summary>
    /// 使用Unity的方法发送Post数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postDataStr"></param>
    /// <returns></returns>
    public IEnumerator UntiyPost(string url, string postDataStr)
    {
        playLoadAni();
        Debug.Log("协程被调用！");
        Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
        parameters.Add("params", postDataStr);
        Debug.Log(parameters.ToString());
        byte[] postData = Encoding.UTF8.GetBytes(NetTools.BuildQuery(parameters, "utf8")); //使用utf-8格式组装post参数
        //byte[] databyte = Encoding.UTF8.GetBytes(jsondata);
        UnityWebRequest _request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        _request.uploadHandler = new UploadHandlerRaw(postData);
        _request.downloadHandler = new DownloadHandlerBuffer();
        _request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
        yield return _request.SendWebRequest();
        if (_request.result == UnityWebRequest.Result.ProtocolError || _request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(_request.error);
            alert.setAlertInfo("网络发生错误！");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, _request.error, "提示框", 0);
        }
        else
        {
            stopLoadAni();
            Debug.Log(_request.downloadHandler.text);
            ReturnMessage returnMes = JsonMapper.ToObject<ReturnMessage>(_request.downloadHandler.text);
            if (returnMes.success)
            {
                //不弹窗直接成功
                //this.gameObject.SetActive(false);
                alert.setAlertInfo("操作成功！");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                baseAlertPanel.SetActive(true);
                Debug.Log("操作成功！");
            }
            else 
            {
                alert.setAlertInfo("操作失败！");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                baseAlertPanel.SetActive(true);
                Debug.Log("操作失败！");
            }
        }
        doorDevice.StopInventory();
    }

}
