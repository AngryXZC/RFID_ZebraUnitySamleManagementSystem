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
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    private Coroutine recive;
    /// <summary>
    /// ���ʹ�������
    /// </summary>
    private int sendTime = 0;
    //���ջ�����
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];

    /// <summary>
    /// ��Ʒ��������ҳ
    /// </summary>
    public GameObject passSamplePanel;
    /// <summary>
    ///��ʶ�Ƿ���������
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// ��Ա�绰��
    /// </summary>
    private string operatorPhone = string.Empty;

    // Start is called before the first frame update
    private void OnEnable()
    {
        
        initLoadPanel();
        returnBtn.onClick.AddListener(()=>onReturnBtnlick());
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("�����������Աˢ����");
        //��ʼ�������
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
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("�������Ӵ���");
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
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
                alert.setAlertInfo("��Աˢ�����ִ���");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                baseAlertPanel.SetActive(true);
                // Messagebox.MessageBox(IntPtr.Zero, "��Աˢ�����ִ���", "��ʾ��", 0);
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
                alert.setAlertInfo("�����������ӣ�");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                baseAlertPanel.SetActive(true);
                Debug.Log("�������������⣡");
            }

            yield return new WaitForSeconds(3f);
        }


    }
}
