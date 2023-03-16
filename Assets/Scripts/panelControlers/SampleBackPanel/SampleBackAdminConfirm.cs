using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.Networking;
using System;
using System.Net.Sockets;

public class SampleBackAdminConfirm : BasePanel
{
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    AlertController alert;

    /// <summary>
    /// 刷卡协程
    /// </summary>
    private Coroutine recive;
    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    /// <summary>
    ///标识是否发送了数据
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// 发送的数据
    /// </summary>
    private BackSamRes result = null;
 
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string adminPhone = string.Empty;
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请库房管理人员刷卡！");
        returnBtn.onClick.AddListener(() => onReturnBtnlick());
        recive = StartCoroutine(reciveCorutine());
        sendFlag = false;
        adminPhone = string.Empty;
    }

    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        alert.returnBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveAllListeners();
        stopLoadAni();
        StopAllCoroutines();
        adminPhone = string.Empty;
        sendFlag = false;
        result = null;
    }
    private void Update()
    {
        Debug.Log(sendFlag);
        if (sendFlag)
        {
            Debug.Log(result.herdmanList.Count);
            if (result.herdmanList.Count > 0)
            {
                result.administratorPhone = adminPhone;
                string json = Regex.Unescape(JsonMapper.ToJson(result));
                Debug.Log(json);
                StartCoroutine(UntiyPost(IPAddressConfig.backSamolePutup, json));
                sendFlag = false;
            }

        }
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="res"></param>
    public void initData(BackSamRes res)
    {

        result = res;
    }

    /// <summary>
    /// 使用Unity的方法发送Post数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postDataStr"></param>
    /// <returns></returns>
    public IEnumerator UntiyPost(string url, string postDataStr)
    {
        StopCoroutine(recive);
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
            Debug.LogError(_request.error);
            alert.setAlertInfo("网络错误!");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //重新绑定弹窗的点击事件
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, _request.error, "提示框", 0);
            stopLoadAni();
        }
        else
        {
            stopLoadAni();
            Debug.Log(_request.downloadHandler.text);
            ReturnMessage returnMessage = JsonMapper.ToObject<ReturnMessage>(_request.downloadHandler.text);
            if (returnMessage.success)
            {
                alert.setAlertInfo("样品退还成功!");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                }
                //Messagebox.MessageBox(IntPtr.Zero, "样品退还成功！", "提示框", 0);
                //onReturnBtnlick();
            }
            else
            {
                alert.setAlertInfo("样品退还失败!");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                }
                
                //Messagebox.MessageBox(IntPtr.Zero, "样品退还失败！", "提示框", 0);
            }
        
        }
        
    }
    IEnumerator reciveCorutine()
    {
        while (true)
        {

            try
            {
                adminPhone=String.Empty;
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
