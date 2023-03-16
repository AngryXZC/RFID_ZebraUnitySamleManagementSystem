using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.Networking;
using System;
/// <summary>
/// ���ݴ������Э�������Item����Update�ﱣ֤��ʵʱ��
/// </summary>
public class FinishCleanSamplePanelControoler : BasePanel
{

    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    /// <summary>
    /// �����ϴ
    /// </summary>
    public Button finishCleanBtn; 
    /// <summary>
    /// ��Ҫʶ���������Ϣ
    /// </summary>
    private CleanManIdentifyRes finishCleanInfo;
    //����
    public GameObject sampleItem;
    //�б�����
    public Transform content;
    //�������ݰ�
    private List<CleanItem> finishCleanItems = new List<CleanItem>();
    //�豸
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// �洢ʶ�����Ʒ�Ĺ�ϣ��
    /// </summary>
    private Hashtable m_TagTable;

    private void OnEnable()
    {
        //ͬ������
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    DestroyImmediate(content.GetChild(0).gameObject);
        //}
        //�첽����
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    Destroy(content.GetChild(i).gameObject);
        //}
        initLoadPanel();
        //��ʼ�������
        alert = baseAlertPanel.GetComponent<AlertController>();
        //��ʼ���б�����
        finishCleanInfo = null;
        //�󶨷��ذ�ť
        this.returnBtn.onClick.AddListener(this.onReturnBtnlick);
        //��ͨ���Ŵ��̹���
        doorDevice = GangwayDoorDevice.getInstance();
        
        if (doorDevice.IsConnect)
        {
            doorDevice.StartInventory();
            //ע���ȡ�¼���������
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
        m_TagTable = new Hashtable();
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        //ȡ��ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);

        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        m_TagTable.Clear();
        Debug.Log(content.childCount);
        //ͬ������
        for (; content.childCount!=0; )
        {
            Debug.Log("ͬ�����٣�"+content.childCount);
            DestroyImmediate(content.GetChild(0).gameObject,true);
        }
        //�첽����
        //for (int i = 0; i < content.childCount; i++)
        //{

        //    Destroy(content.GetChild(i).gameObject);
        //}
        finishCleanItems.Clear();
        
    }
    

    
    public void setFinishCleanRootInfo(CleanManIdentifyRes te)
    {
        this.finishCleanInfo = te;
        generateItem();
    }
    private void generateItem()
    {
        
        if (finishCleanInfo != null)
        {
            foreach (CleanHerdsmanListItem item in finishCleanInfo.herdmanList)
            {
                GameObject go = Instantiate(sampleItem);
                go.transform.SetParent(content);
                CleanItem temp=go.GetComponent<CleanItem>();
                finishCleanItems.Add(temp);
                temp.initItem(finishCleanItems.Count, item, "");
            }

        }
    }
    public void  finsihCleanBtn() 
    {
        bool hasError = false;
        List< CleanHerdsmanListItem> resList=new List< CleanHerdsmanListItem>();
        Debug.Log("����ύ��ť��");
        Debug.Log(content.childCount);
        if (content.childCount<=0) 
        {
            hasError = true;
            alert.setAlertInfo("�������ύ�����ݣ�");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            Debug.Log("�������ύ������");
           
            return;
        }
        foreach (CleanHerdsmanListItem item in finishCleanInfo.herdmanList)
        {

            if (item.detectSampleNum + item.abnormalBackSampleNum != item.cleanSampleNum)
            {
               //�Ƴ�������� 
                continue;
            }
            else 
            {
                resList.Add(item);
            }
        }
        Debug.Log(hasError);
        //��������
        if (hasError || resList.Count==0)
        {
            alert.setAlertInfo("�˶������ϴ��Ӧ������");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            Debug.Log("�˶������ϴ��Ӧ����");
            
        }
        else 
        {
            finishCleanInfo.herdmanList=resList;
            string json = JsonMapper.ToJson(finishCleanInfo);
            json = Regex.Unescape(json);
            Debug.Log("������ϴ����"+json);
            StartCoroutine(UntiyPost(IPAddressConfig.submitFinishClean,json));
        }
        

    }
  
  
    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        bool isSampleNumber = DataCheck.isSampleNumber(tagID);
        if (!isSampleNumber || m_TagTable.Contains(tagID))
        {
            return;
        }
        else 
        {

            //ʶ��һ����ǩ
            foreach (CleanItem item in finishCleanItems)
            {
                
                if (item.getATag(tagID.Substring(1, 15))&&!m_TagTable.Contains(tagID))
                {
                    m_TagTable.Add(tagID, "1");
                }

            }
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
        playLoadAni();
        Debug.Log("Э�̱����ã�");
        Dictionary<string, string> parameters = new Dictionary<string, string>(); //�����б�
        parameters.Add("params", postDataStr);
        Debug.Log(parameters.ToString());
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
            alert.setAlertInfo("���緢������");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, _request.error, "��ʾ��", 0);
        }
        else
        {
            stopLoadAni();
            Debug.Log(_request.downloadHandler.text);
            ReturnMessage returnMes = JsonMapper.ToObject<ReturnMessage>(_request.downloadHandler.text);
            if (returnMes.success)
            {
                //������ֱ�ӳɹ�
                //this.gameObject.SetActive(false);
                alert.setAlertInfo("�����ɹ���");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                baseAlertPanel.SetActive(true);
                Debug.Log("�����ɹ���");
            }
            else 
            {
                alert.setAlertInfo("����ʧ�ܣ�");
                alert.returnBtn.onClick.AddListener(() => alert.hideSelfAndReturnTonew(parentPanel,gameObject));
                baseAlertPanel.SetActive(true);
                Debug.Log("����ʧ�ܣ�");
            }
        }
        doorDevice.StopInventory();
    }

}
