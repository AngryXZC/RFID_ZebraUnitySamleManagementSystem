using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.Networking;

public class SampleBackManConfirm : BasePanel
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
    ///标识是否发送了数据
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// 待领取样品清单
    /// </summary>
    public GameObject sampleBackPanel;
   
    /// <summary>
    /// 人员电话号
    /// </summary>
    private string operatorPhone = string.Empty;
    // Start is called before the first frame update
    void OnEnable()
    {
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请退还检验人员刷卡！");
        returnBtn.onClick.AddListener(()=>onReturnBtnlick());
        alert=baseAlertPanel.GetComponent<AlertController>();
        
        recive =StartCoroutine(reciveCorutine());
        sendFlag = false;
    }
    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        returnBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        stopLoadAni();
        StopAllCoroutines();
        operatorPhone = string.Empty;
        sendFlag = false;
      
    }
   

    private void Update()
    {
        if (sendFlag)
        {
            Debug.Log("发送！"+operatorPhone);
            StartCoroutine(UntiyGet(IPAddressConfig.backSampleConfirm, "operatorManPhone=" + operatorPhone));
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
            
            try
            {
                Debug.Log(webRequest.downloadHandler.text);
                BackResRoot res= JsonMapper.ToObject<BackResRoot>(webRequest.downloadHandler.text);
                if (res.success)
                {
                    Debug.Log(res.result.ToString());
                    sampleBackPanel.GetComponent<SampleBackController>().initInfo(res.result);
                    //跳转到样品退还
                    this.gameObject.SetActive(false);
                    sampleBackPanel.SetActive(true);

                }
                
            }
            catch (Exception e)
            {
                Debug.Log(e);
                alert.setAlertInfo("刷卡错误！");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                baseAlertPanel.SetActive(true);
            }

            finally
            {
                stopLoadAni();
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
                    sendFlag = true;
                }

                //Close
                socket.Close();
            }
            catch (System.Exception)
            {

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
