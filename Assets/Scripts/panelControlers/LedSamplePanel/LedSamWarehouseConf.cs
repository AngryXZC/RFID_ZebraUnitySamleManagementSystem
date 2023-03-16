using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;
using System;
using System.Text;
using UnityEngine.Networking;
using System.Net.Sockets;

public class LedSamWarehouseConf : BasePanel
{
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    private CleanManIdentifyRes info;
   
    /// <summary>
    ///标识是否发送了数据
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string adminPhone = string.Empty;
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        returnBtn.onClick.AddListener (() =>onReturnBtnlick());
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请库房管理人员刷卡！");
        //初始化警告框
        alert = baseAlertPanel.GetComponent<AlertController>();
        adminPhone = string.Empty;
        StartCoroutine(reciveCorutine());
       sendFlag = false;
    }
    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        returnBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        info = null;
        stopLoadAni();
        StopAllCoroutines();
        adminPhone = string.Empty;
        sendFlag = false;
       
        
    }
    void Update()
    {
        Debug.Log(sendFlag);
        if (sendFlag)
        {
            Debug.Log("可发送！");
            if (info != null && adminPhone != String.Empty)
            {
                Debug.Log("真正发送！");
                info.warehouseManPhone = adminPhone;
                string json = JsonMapper.ToJson(info);
                json = Regex.Unescape(json);
                StartCoroutine(UntiyPost(IPAddressConfig.warehouseManIdentify, json));
                sendFlag = false;
            }

        }
    }


    public void setInfo(CleanManIdentifyRes res)
    {
        info = res;
    }
  
    /// <summary>
    /// 使用Unity的方法发送Post数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postDataStr"></param>
    /// <returns></returns>
    public IEnumerator UntiyPost(string url, string postDataStr)
    {
        StopCoroutine(reciveCorutine());
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
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
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
            
            LedResultRoot ledResultRoot = JsonMapper.ToObject<LedResultRoot>(_request.downloadHandler.text);
            if (ledResultRoot.success)
            {
                if (ledResultRoot.result.isLedSampleSuccess)
                {
                    Debug.Log("领样成功！");
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                    alert.setAlertInfo("领样成功！");
                    if (!baseAlertPanel.activeInHierarchy)
                    {
                        baseAlertPanel.SetActive(true);
                    }
                    //Messagebox.MessageBox(IntPtr.Zero, "领样成功！！", "提示框", 0);
                }
                else
                {
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel, gameObject));
                    alert.setAlertInfo("领样失败！");
                    if (!baseAlertPanel.activeInHierarchy)
                    {
                        baseAlertPanel.SetActive(true);
                    }
                    //Messagebox.MessageBox(IntPtr.Zero, "领样失败！！", "提示框", 0);
                    Debug.Log("领样失败！");
                }
            }
            else
            {
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                alert.setAlertInfo("管理员确认失败！");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                //Messagebox.MessageBox(IntPtr.Zero, "管理员确认失败！！", "提示框", 0);
                Debug.Log("领样失败！");
            }
            Debug.Log(_request.downloadHandler.text);
          
        }
        StopAllCoroutines();
       
    }
    IEnumerator reciveCorutine()
    {
        while (true)
        {

            try
            {

                Socket socket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Stream, ProtocolType.Tcp);
                //Connect
                socket.Connect(NET.host, NET.port);
                //Send
                string str = "ManCode";
                byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
                socket.Send(bytes);
                //Recv
                int count = socket.Receive(readBuff);
                str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                if (str.Length > 15)
                {
                    adminPhone = str.Substring(5);
                    sendFlag = true;
                }

                //Close
                socket.Close();
            }
            catch (System.Exception)
            {

                Debug.Log("数据连接有问题！");
            }
            yield return new WaitForSeconds(3f);
        }


    }
}