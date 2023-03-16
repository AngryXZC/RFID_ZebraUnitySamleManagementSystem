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
    /// �������Ϣ
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
    public GameObject sampleBackPanel;
   
    /// <summary>
    /// ��Ա�绰��
    /// </summary>
    private string operatorPhone = string.Empty;
    // Start is called before the first frame update
    void OnEnable()
    {
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("���˻�������Աˢ����");
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
            Debug.Log("���ͣ�"+operatorPhone);
            StartCoroutine(UntiyGet(IPAddressConfig.backSampleConfirm, "operatorManPhone=" + operatorPhone));
            sendFlag = false;
        }
        
    }

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
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
            alert.setAlertInfo("�������Ӵ���");
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
                    //��ת����Ʒ�˻�
                    this.gameObject.SetActive(false);
                    sampleBackPanel.SetActive(true);

                }
                
            }
            catch (Exception e)
            {
                Debug.Log(e);
                alert.setAlertInfo("ˢ������");
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

                alert.setAlertInfo("����д�����ӣ�");
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
