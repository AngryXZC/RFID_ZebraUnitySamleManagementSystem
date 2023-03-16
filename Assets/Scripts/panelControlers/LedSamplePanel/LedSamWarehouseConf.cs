using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;
using System;
using System.Text;
using UnityEngine.Networking;
using System.Net.Sockets;

public class LedSamWarehouseConf : BasePanel
{
    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    //���ջ�����
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    private CleanManIdentifyRes info;
   
    /// <summary>
    ///��ʶ�Ƿ���������
    /// </summary>
    private bool sendFlag = false;
    /// <summary>
    /// ��Ա�绰��
    /// </summary>
    private string adminPhone = string.Empty;
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        returnBtn.onClick.AddListener (() =>onReturnBtnlick());
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("��ⷿ������Աˢ����");
        //��ʼ�������
        alert = baseAlertPanel.GetComponent<AlertController>();
        adminPhone = string.Empty;
        StartCoroutine(reciveCorutine());
       sendFlag = false;
    }
    private void OnDisable()
    {
        voiceController.stopVoiceCoroutine();
        returnBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        info = null;
        stopLoadAni();
        StopAllCoroutines();
        adminPhone = string.Empty;
        sendFlag = false;
       
        
    }
    void Update()
    {
        Debug.Log(sendFlag);
        if (sendFlag)
        {
            Debug.Log("�ɷ��ͣ�");
            if (info != null && adminPhone != String.Empty)
            {
                Debug.Log("�������ͣ�");
                info.warehouseManPhone = adminPhone;
                string json = JsonMapper.ToJson(info);
                json = Regex.Unescape(json);
                StartCoroutine(UntiyPost(IPAddressConfig.warehouseManIdentify, json));
                sendFlag = false;
            }

        }
    }


    public void setInfo(CleanManIdentifyRes res)
    {
        info = res;
    }
  
    /// <summary>
    /// ʹ��Unity�ķ�������Post����
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postDataStr"></param>
    /// <returns></returns>
    public IEnumerator UntiyPost(string url, string postDataStr)
    {
        StopCoroutine(reciveCorutine());
        Debug.Log(postDataStr);
        playLoadAni();
        Debug.Log("Э�̱����ã�");
        Dictionary<string, string> parameters = new Dictionary<string, string>(); //�����б�
        parameters.Add("params", postDataStr);
        byte[] postData = Encoding.UTF8.GetBytes(NetTools.BuildQuery(parameters, "utf8")); //ʹ��utf-8��ʽ��װpost����
        //byte[] databyte = Encoding.UTF8.GetBytes(jsondata);
        UnityWebRequest _request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

        _request.uploadHandler = new UploadHandlerRaw(postData);
        _request.downloadHandler = new DownloadHandlerBuffer();
        _request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
        yield return _request.SendWebRequest();
        if (_request.result == UnityWebRequest.Result.ProtocolError || _request.result == UnityWebRequest.Result.ConnectionError)
        {
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            alert.setAlertInfo("�����������");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
            }
            Debug.LogError(_request.error);
            //Messagebox.MessageBox(IntPtr.Zero, _request.error, "��ʾ��", 0);
            stopLoadAni();
        }
        else
        {
            stopLoadAni();
            
            LedResultRoot ledResultRoot = JsonMapper.ToObject<LedResultRoot>(_request.downloadHandler.text);
            if (ledResultRoot.success)
            {
                if (ledResultRoot.result.isLedSampleSuccess)
                {
                    Debug.Log("�����ɹ���");
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                    alert.setAlertInfo("�����ɹ���");
                    if (!baseAlertPanel.activeInHierarchy)
                    {
                        baseAlertPanel.SetActive(true);
                    }
                    //Messagebox.MessageBox(IntPtr.Zero, "�����ɹ�����", "��ʾ��", 0);
                }
                else
                {
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel, gameObject));
                    alert.setAlertInfo("����ʧ�ܣ�");
                    if (!baseAlertPanel.activeInHierarchy)
                    {
                        baseAlertPanel.SetActive(true);
                    }
                    //Messagebox.MessageBox(IntPtr.Zero, "����ʧ�ܣ���", "��ʾ��", 0);
                    Debug.Log("����ʧ�ܣ�");
                }
            }
            else
            {
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                alert.setAlertInfo("����Աȷ��ʧ�ܣ�");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                //Messagebox.MessageBox(IntPtr.Zero, "����Աȷ��ʧ�ܣ���", "��ʾ��", 0);
                Debug.Log("����ʧ�ܣ�");
            }
            Debug.Log(_request.downloadHandler.text);
          
        }
        StopAllCoroutines();
       
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

                Debug.Log("�������������⣡");
            }
            yield return new WaitForSeconds(3f);
        }


    }
}