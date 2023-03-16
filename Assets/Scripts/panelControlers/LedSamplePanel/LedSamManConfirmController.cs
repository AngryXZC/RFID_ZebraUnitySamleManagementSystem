using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.Networking;

public class LedSamManConfirmController : BasePanel
{
    
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
    /// 待领取样品清单
    /// </summary>
    public GameObject ledSamplePanel;

    /// <summary>
    /// 人员电话号
    /// </summary>
    private string operatorPhone = string.Empty;
    // Start is called before the first frame update
    void OnEnable()
    {
      
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(() => onReturnBtnlick());

       
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("请清洗领样人员刷卡！");

        operatorPhone = string.Empty;
        recive=StartCoroutine(reciveCorutine());
        
        sendFlag = false;
      
    }
    private void OnDisable()
    {
        returnBtn.onClick.RemoveAllListeners();
        voiceController.stopVoiceCoroutine();
        stopLoadAni();
        operatorPhone = string.Empty;
        sendFlag = false;
        StopAllCoroutines();
    }
    // Update is called once per frame
    void Update()
    {
        
        if (sendFlag)
        {
            Debug.Log(operatorPhone);
           
            //res = NetTools.HttpGet(IPAddressConfig.ledSampleManIdentify, "executor_id=" + tag.TagID.Substring(0, 11));
            StartCoroutine(UntiyGet(IPAddressConfig.ledSampleManIdentify, "executor_id=" + operatorPhone));
            sendFlag = false;
            
        }
    }

    /// <summary>
    /// 读取标签事件函数监听
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

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
            Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
            stopLoadAni();
        }
        else
        {
            CleanManIdentifyRoot cleanManIdentifyRoot;
            try
            {
                cleanManIdentifyRoot = JsonMapper.ToObject<CleanManIdentifyRoot>(webRequest.downloadHandler.text);
                if (cleanManIdentifyRoot.success)
                {

                    LedSamController ledSam = ledSamplePanel.GetComponent<LedSamController>();
                    ledSam.init(cleanManIdentifyRoot.result);
                    gameObject.SetActive(false);
                    ledSamplePanel.SetActive(true);
                }
            }
            catch (Exception)
            {
                Messagebox.MessageBox(IntPtr.Zero, "数据解析错误！请联系段宁！", "提示框", 0);

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
                if (str.Length>15)
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
                alert.setAlertInfo("请检查单个读写器的连接与设置!");
                if (!baseAlertPanel.activeInHierarchy) 
                {
                    baseAlertPanel.SetActive(true);
                    //重新绑定弹窗的点击事件
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                }
               
            }
            yield return new WaitForSeconds(3f);
        }


    }
}
