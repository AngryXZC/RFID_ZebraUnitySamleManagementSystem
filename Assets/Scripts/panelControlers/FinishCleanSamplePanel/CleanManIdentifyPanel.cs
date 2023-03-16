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
    /// ����һ�εı��
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// �����ϴ��Ʒ����
    /// </summary>
    public GameObject finishCleanPanel;
  
    /// <summary>
    /// ��Ա�绰��
    /// </summary>
    private string operatorPhone = string.Empty;
    // Start is called before the first frame update
    void OnEnable()
    {
       
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("�������ϴ��Աˢ����");
        alert = baseAlertPanel.GetComponent<AlertController>();
        recive = StartCoroutine(reciveCorutine());
        sendFlag = false;
        //�󶨷��ذ�ť
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

                Debug.Log("�������������⣡");
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
