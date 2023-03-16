using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

public class OutboundPatchConfirm : BasePanel
{
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 刷卡协程
    /// </summary>
    private Coroutine recive;
    /// <summary>
    /// 提交数据
    /// </summary>
    private OutboundPatchInfoRes outboundResult;
    /// <summary>
    /// 标记是否已发送的flag（确保只在一帧发送数据）
    /// </summary>
    private bool sendFlag = false;
    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string adminPhone = string.Empty;
    private void OnEnable()
    {
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请库房管理人员刷卡！");
        alert = baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(onReturnBtnlick);
        adminPhone = string.Empty;
        recive= StartCoroutine(reciveCorutine());
        
        sendFlag = false;
    }
    private void Update()
    {
        if (sendFlag)
        {
            Debug.Log(adminPhone);
            outboundResult.warehouseManPhone = adminPhone;
            string json= Regex.Unescape(JsonMapper.ToJson(outboundResult));
            StartCoroutine(UntiyPost(IPAddressConfig.outboundConfirm,json));
            Debug.Log(json);
            sendFlag = false;
        }
    }
    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        alert.returnBtn.onClick.RemoveAllListeners();
        stopLoadAni();
        sendFlag = false;
        adminPhone = string.Empty;
        returnBtn.onClick.RemoveAllListeners();
        StopAllCoroutines();
    }

    public void initResultInfo(OutboundPatchInfoRes res)
    {
        outboundResult = res;
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
            alert.setAlertInfo("网络错误!");
            Debug.LogError(_request.error);
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //重新绑定弹窗的点击事件
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            // Messagebox.MessageBox(IntPtr.Zero, _request.error, "提示框", 0);
        }
        else
        {
            OutboundPatchResRoot outboundPatchRes = JsonMapper.ToObject<OutboundPatchResRoot>(_request.downloadHandler.text);
            if (outboundPatchRes.success)
            {
                alert.setAlertInfo("出库成功!");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel, gameObject));
                }
            }
            else 
            {
                alert.setAlertInfo("出库失败!"+ outboundPatchRes.message);
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel, gameObject));
                }
            }
            stopLoadAni();
            Debug.Log(_request.downloadHandler.text); 
        }

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
                alert.setAlertInfo("单个读卡器连接有问题");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() =>alert.hideSelf());
                }
            }
            
            yield return new WaitForSeconds(3f);
        }


    }

}
