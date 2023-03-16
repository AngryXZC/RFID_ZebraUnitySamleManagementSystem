using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using System;
using UnityEngine.Networking;
using System.Net.Sockets;

public class CleanManIdentifyPanel : BasePanel
{
    /// <summary>
    /// 警告框信息
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
    /// 发送一次的标记
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// 完成清洗样品界面
    /// </summary>
    public GameObject finishCleanPanel;
  
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string operatorPhone = string.Empty;
    // Start is called before the first frame update
    void OnEnable()
    {
       
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请完成清洗人员刷卡！");
        alert = baseAlertPanel.GetComponent<AlertController>();
        recive = StartCoroutine(reciveCorutine());
        sendFlag = false;
        //绑定返回按钮
        this.returnBtn.onClick.AddListener(this.onReturnBtnlick);
       

    }
    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        alert.returnBtn.onClick.RemoveAllListeners();
        stopLoadAni();
        StopAllCoroutines();
        sendFlag = false;
      
        this.returnBtn.onClick.RemoveAllListeners();
        operatorPhone = string.Empty;
       
    }
    // Update is called once per frame
    void Update()
    {
        if (sendFlag)
        {
            
            //res = NetTools.HttpGet(IPAddressConfig.finishCleanManIdentify, "operatorManPhone=" + tag.TagID.Substring(0, 11));
            StartCoroutine(UntiyGet(IPAddressConfig.finishCleanManIdentify, "operatorManPhone=" + operatorPhone));
            StopCoroutine(recive);
            sendFlag = false;
        }
    }
   
    //使用Unity发送类发送
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        StopCoroutine(recive);
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {

            Debug.Log(webRequest.error);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
            alert.setAlertInfo("网络连接错误！");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            stopLoadAni();
        }
        else
        {
            CleanManIdentifyRoot root = JsonMapper.ToObject<CleanManIdentifyRoot>(webRequest.downloadHandler.text);
            if (root.success)
            {
                this.gameObject.SetActive(false);
                finishCleanPanel.SetActive(true);
                finishCleanPanel.GetComponent<FinishCleanSamplePanelControoler>().setFinishCleanRootInfo(root.result);
            }
            Debug.Log(webRequest.downloadHandler.text);
            stopLoadAni();
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
                    operatorPhone = str.Substring(5);
                    sendFlag = true;
                }

                //Close
                socket.Close();
            }
            catch (System.Exception)
            {

                Debug.Log("数据连接有问题！");
                alert.setAlertInfo("检查读写器连接！");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
            }
            yield return new WaitForSeconds(3f);
        }


    }
}
