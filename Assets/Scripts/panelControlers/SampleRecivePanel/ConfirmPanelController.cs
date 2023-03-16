using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Symbol.RFID3;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Net.Sockets;

public class ConfirmPanelController : BasePanel
{
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    private AlertController alert;
    /// <summary>
    /// 刷卡协程
    /// </summary>
    private Coroutine recive;
    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    
    /// <summary>
    /// 返回结果提示
    /// </summary>
    public Text returnInfoText;
    /// <summary>
    /// 申请单号
    /// </summary>
    public Text apply_code;
    /// <summary>
    /// 送检单位
    /// </summary>
    public Text inspectDepartmentName;
    /// <summary>
    /// 牧户数量
    /// </summary>
    public Text herdsmanNum;
    /// <summary>
    /// 实收样品数量
    /// </summary>
    public Text actualSampleNum;
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string adminPhone = string.Empty;

    /// <summary>
    /// 发送到服务器的实体
    /// </summary>
    private InstitutionInfo sendResult;
    /// <summary>
    /// 标记是否已发送的flag（确保只在一帧发送数据）
    /// </summary>
    private bool sendFlag = false;
 
    /// <summary>
    /// 点击返回返回到批次列表界面
    /// </summary>
    public GameObject patchPanle;
    /// <summary>
    /// 日期
    /// </summary>
    public Text dateText;

    private void OnEnable()
    {
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请库房管理人员确认信息并刷卡！");
        returnBtn.onClick.AddListener(()=>onReturnBtnlick());
        alert=baseAlertPanel.GetComponent<AlertController>();
        adminPhone = string.Empty;
        recive= StartCoroutine(reciveCorutine());
        sendFlag = false;
    }
    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        returnBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        StopCoroutine(recive);
        stopLoadAni();
        adminPhone = string.Empty;
        sendFlag = false;
        StopAllCoroutines();

    }

    /// <summary>
    /// 初始化面板
    /// </summary>
    public void initPanel(InstitutionInfo res)
    {
        sendResult = res;
        apply_code.text = res.applyCode;
        inspectDepartmentName.text = res.sampleSource;
        this.herdsmanNum.text = res.detectHermanNum.ToString();
        actualSampleNum.text = res.actualSampleSum.ToString()+"个";

    }

   

    private void Update()
    {
        dateText.text = DateTime.Now.ToString();
        if (sendFlag)
        {   
            
            sendResult.creator_phone = adminPhone;
            //转成Json
            string json = Regex.Unescape(JsonMapper.ToJson(sendResult));
            //没写完去空格的替换特殊字符
            //json=json.Replace("[\b\r\n\t]*", "");
            Debug.Log("发送库管确认数据："+json);
            //使用原生UnityWebRequest（推荐）
             StartCoroutine(UntiyPost(IPAddressConfig.submitReciveSampleResAdress, json));
            sendFlag = false;
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
            Debug.LogError(_request.error);
            onReturnBtnlick();
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            alert.setAlertInfo(_request.error);
           // Messagebox.MessageBox(IntPtr.Zero, _request.error, "提示框", 0);
        }
        else
        {
            stopLoadAni();
            Debug.Log(_request.downloadHandler.text);
            ReturnMessage returnMessage = JsonMapper.ToObject<ReturnMessage>(_request.downloadHandler.text);
            if (returnMessage.success)
            {
                alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, patchPanle));
                alert.setAlertInfo("接收样品成功！");
                if (!baseAlertPanel.activeInHierarchy) 
                {
                    baseAlertPanel.SetActive(true);
                }
                //gameObject.SetActive(false);
                //alertPanel.SetActive(true);
                //alertPanel.GetComponent<AlertController>().setAlertInfo("接收样品成功！");
                StopAllCoroutines();
            }
            else 
            {
                alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, patchPanle));
                alert.setAlertInfo("接收样品失败！");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                //gameObject.SetActive(false);
                //alertPanel.SetActive(true);
                //alertPanel.GetComponent<AlertController>().setAlertInfo("接收样品失败！");
                StopAllCoroutines();
            }
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
                alert.setAlertInfo("请检查单个读写器的连接与设置!");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                    baseAlertPanel.SetActive(true);
                }

            }
            yield return new WaitForSeconds(3f);
        }


    }

}
