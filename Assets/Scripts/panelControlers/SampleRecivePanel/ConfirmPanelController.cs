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
    /// ����alert�Ľű�
    /// </summary>
    private AlertController alert;
    /// <summary>
    /// ˢ��Э��
    /// </summary>
    private Coroutine recive;
    //���ջ�����
    const int BUFFER_SIZE = 1024;
    byte[] readBuff = new byte[BUFFER_SIZE];
    
    /// <summary>
    /// ���ؽ����ʾ
    /// </summary>
    public Text returnInfoText;
    /// <summary>
    /// ���뵥��
    /// </summary>
    public Text apply_code;
    /// <summary>
    /// �ͼ쵥λ
    /// </summary>
    public Text inspectDepartmentName;
    /// <summary>
    /// ��������
    /// </summary>
    public Text herdsmanNum;
    /// <summary>
    /// ʵ����Ʒ����
    /// </summary>
    public Text actualSampleNum;
    /// <summary>
    /// ��Ա�绰��
    /// </summary>
    private string adminPhone = string.Empty;

    /// <summary>
    /// ���͵���������ʵ��
    /// </summary>
    private InstitutionInfo sendResult;
    /// <summary>
    /// ����Ƿ��ѷ��͵�flag��ȷ��ֻ��һ֡�������ݣ�
    /// </summary>
    private bool sendFlag = false;
 
    /// <summary>
    /// ������ط��ص������б����
    /// </summary>
    public GameObject patchPanle;
    /// <summary>
    /// ����
    /// </summary>
    public Text dateText;

    private void OnEnable()
    {
        initLoadPanel();
        voiceController.startVoiceCoroutine();
        voiceController.voiceQueue.Enqueue("��ⷿ������Աȷ����Ϣ��ˢ����");
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
    /// ��ʼ�����
    /// </summary>
    public void initPanel(InstitutionInfo res)
    {
        sendResult = res;
        apply_code.text = res.applyCode;
        inspectDepartmentName.text = res.sampleSource;
        this.herdsmanNum.text = res.detectHermanNum.ToString();
        actualSampleNum.text = res.actualSampleSum.ToString()+"��";

    }

   

    private void Update()
    {
        dateText.text = DateTime.Now.ToString();
        if (sendFlag)
        {   
            
            sendResult.creator_phone = adminPhone;
            //ת��Json
            string json = Regex.Unescape(JsonMapper.ToJson(sendResult));
            //ûд��ȥ�ո���滻�����ַ�
            //json=json.Replace("[\b\r\n\t]*", "");
            Debug.Log("���Ϳ��ȷ�����ݣ�"+json);
            //ʹ��ԭ��UnityWebRequest���Ƽ���
             StartCoroutine(UntiyPost(IPAddressConfig.submitReciveSampleResAdress, json));
            sendFlag = false;
        }
    }
    /// <summary>
    /// ʹ��Unity�ķ�������Post����
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postDataStr"></param>
    /// <returns></returns>
    public IEnumerator UntiyPost(string url, string postDataStr)
    {
        StopCoroutine(recive);
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
            Debug.LogError(_request.error);
            onReturnBtnlick();
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            alert.setAlertInfo(_request.error);
           // Messagebox.MessageBox(IntPtr.Zero, _request.error, "��ʾ��", 0);
        }
        else
        {
            stopLoadAni();
            Debug.Log(_request.downloadHandler.text);
            ReturnMessage returnMessage = JsonMapper.ToObject<ReturnMessage>(_request.downloadHandler.text);
            if (returnMessage.success)
            {
                alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, patchPanle));
                alert.setAlertInfo("������Ʒ�ɹ���");
                if (!baseAlertPanel.activeInHierarchy) 
                {
                    baseAlertPanel.SetActive(true);
                }
                //gameObject.SetActive(false);
                //alertPanel.SetActive(true);
                //alertPanel.GetComponent<AlertController>().setAlertInfo("������Ʒ�ɹ���");
                StopAllCoroutines();
            }
            else 
            {
                alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, patchPanle));
                alert.setAlertInfo("������Ʒʧ�ܣ�");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                //gameObject.SetActive(false);
                //alertPanel.SetActive(true);
                //alertPanel.GetComponent<AlertController>().setAlertInfo("������Ʒʧ�ܣ�");
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

                Debug.Log("�������������⣡");
                alert.setAlertInfo("���鵥����д��������������!");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    //���°󶨵����ĵ���¼�
                    alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                    baseAlertPanel.SetActive(true);
                }

            }
            yield return new WaitForSeconds(3f);
        }


    }

}
