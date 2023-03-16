using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.Networking;

public class PassSamplePanelController : BasePanel
{
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    private Coroutine recive;
    /// <summary>
    /// 发送次数计数
    /// </summary>
    private int sendTime = 0;
    //接收缓冲区
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];

    /// <summary>
    /// 样品接收详情页
    /// </summary>
    public GameObject passSamplePanel;
    /// <summary>
    ///标识是否发送了数据
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string operatorPhone = string.Empty;

    // Start is called before the first frame update
    private void OnEnable()
    {
        
        initLoadPanel();
        returnBtn.onClick.AddListener(()=>onReturnBtnlick());
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请检验领样人员刷卡！");
        //初始化警告框
        alert = baseAlertPanel.GetComponent<AlertController>();
        recive = StartCoroutine(reciveCorutine());

        sendFlag = false;
    }
    private void OnDisable()
    {
        returnBtn.onClick?.RemoveListener(()=>onReturnBtnlick());
        voiceController.stopVoiceCoroutine();
        alert.returnBtn.onClick.RemoveAllListeners();
        sendTime = 0;
        stopLoadAni();
        sendFlag = false;
        operatorPhone = string.Empty;
        StopAllCoroutines();

    }
    

    // Update is called once per frame
    void Update()
    {
        
        if (sendFlag)
        {
         Debug.Log(operatorPhone);
         StartCoroutine(UntiyGet(IPAddressConfig.sampleReciveManConfirm, "executor_id="+operatorPhone));
               
         sendFlag = false;
        }
    }
 
    
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
            alert.setAlertInfo("网络连接错误！");
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
        }
        else
        {
           
            stopLoadAni();
            
            PassSampleRoot passSampleRoot = JsonMapper.ToObject<PassSampleRoot>(webRequest.downloadHandler.text);
            if (passSampleRoot.success)
            {

                PassSampleInfoController passSampleInfoController = passSamplePanel.GetComponent<PassSampleInfoController>();
                if (passSampleInfoController!=null)
                {
                    passSampleInfoController.bindInfo(passSampleRoot.result);
                }
                gameObject.SetActive(false);
                passSamplePanel.SetActive(true);
            }
            else
            {
                alert.setAlertInfo("人员刷卡出现错误！");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                baseAlertPanel.SetActive(true);
                // Messagebox.MessageBox(IntPtr.Zero, "人员刷卡出现错误！", "提示框", 0);
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
                    
                    operatorPhone = str.Substring(5);
                    sendTime++;
                    sendFlag = true;
                   
                }
                //Close
                socket.Close();
            }
            catch (System.Exception)
            {
                alert.setAlertInfo("检查读卡器连接！");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                baseAlertPanel.SetActive(true);
                Debug.Log("数据连接有问题！");
            }

            yield return new WaitForSeconds(3f);
        }


    }
}
