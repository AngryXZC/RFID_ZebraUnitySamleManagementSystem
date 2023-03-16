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
    
    /// ����alert�Ľű�
    /// </summary>
    AlertController alert;

    /// <summary>
    /// ˢ��Э��
    /// </summary>
    private Coroutine recive;
    //���ջ�����
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    /// <summary>
    ///��ʶ�Ƿ���������
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// ����ȡ��Ʒ�嵥
    /// </summary>
    public GameObject ledSamplePanel;

    /// <summary>
    /// ��Ա�绰��
    /// </summary>
    private string operatorPhone = string.Empty;
    // Start is called before the first frame update
    void OnEnable()
    {
      
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(() => onReturnBtnlick());

       
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("����ϴ������Աˢ����");

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
    /// ��ȡ��ǩ�¼���������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    //ʹ��Unity�����෢��
    IEnumerator UntiyGet(string url, string getDataStr)
    {
     
        StopCoroutine(recive);
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
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
                Messagebox.MessageBox(IntPtr.Zero, "���ݽ�����������ϵ������", "��ʾ��", 0);

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

                Debug.Log("�������������⣡");
                alert.setAlertInfo("���鵥����д��������������!");
                if (!baseAlertPanel.activeInHierarchy) 
                {
                    baseAlertPanel.SetActive(true);
                    //���°󶨵����ĵ���¼�
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                }
               
            }
            yield return new WaitForSeconds(3f);
        }


    }
}
