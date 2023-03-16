using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;
using Symbol.RFID3;

public class OutboundPatchInfoController : BasePanel
{
    /// <summary>
    /// ����alert�Ľű�
    /// </summary>
    AlertController alert;
    /// <summary>
    /// ��ǰҳ����Ϣ
    /// </summary>
    private OutboundPatchInfoRes infoRes;
    /// <summary>
    /// ����Աȷ��ˢ������
    /// </summary>
    public GameObject administratorConfirmPan;
    /// <summary>
    /// �ͼ쵥λ
    /// </summary>
    public Text inspectionDepartment;
    /// <summary>
    /// ���ⰴť
    /// </summary>
    public Button outboundBtn;
    /// <summary>
    /// �б���
    /// </summary>
    public Transform content;
    /// <summary>
    /// �б�����
    /// </summary>
    public GameObject listItem;
    /// <summary>
    /// �б���������
    /// </summary>
    private List<OutboundSampleItemController> listDatas = new List<OutboundSampleItemController>();
    /// <summary>
    /// ��ǰ���α��
    /// </summary>
    private string applyCode = string.Empty;
    /// <summary>
    /// �豸����
    /// </summary>
    GangwayDoorDevice doorDevice;
    /// <summary>
    /// �洢ʶ�����Ʒ�Ĺ�ϣ��
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        doorDevice = GangwayDoorDevice.getInstance();
        if (doorDevice.IsConnect)
        {
            doorDevice.StartInventory();
            //ע���ȡ�¼���������
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
       
        if (outboundBtn != null)
        {
            outboundBtn.onClick.AddListener(outboundBtnClick);
        }
        returnBtn.onClick.AddListener(onReturnBtnlick);
        StartCoroutine(UntiyGet(IPAddressConfig.getOutboundPatchInfo, "applyCode=" + applyCode));
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        if (doorDevice.IsConnect)
        {
            doorDevice.StopInventory();
            //ȡ��ע���ȡ�¼���������
            doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        }
        m_TagTable.Clear();
        listDatas.Clear();
        outboundBtn?.onClick.RemoveListener(outboundBtnClick);
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        applyCode = string.Empty;
        returnBtn.onClick.RemoveAllListeners();
        StopAllCoroutines();
    }
    public void init(string apply_code)
    {
        applyCode = apply_code;

    }
    //ʹ��Unity�����෢��
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("�������!");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //���°󶨵����ĵ���¼�
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
            stopLoadAni();
        }
        else
        {
            stopLoadAni();
            OutboundInfoRoot outboundInfoRoot = JsonMapper.ToObject<OutboundInfoRoot>(webRequest.downloadHandler.text);
            infoRes = outboundInfoRoot.result;
            infoRes.applyCode = applyCode;
            int num = 0;
            foreach (OutboundHerdsmanListItem item in infoRes.herdsmanList)
            {
                inspectionDepartment.text = "��Ʒ��Դ��" + item.HusbandryBureauName;
                num++;
                GameObject go = Instantiate(listItem);
                OutboundSampleItemController outboundSampleItemController = go.GetComponent<OutboundSampleItemController>();
                outboundSampleItemController.init(num, item);
                listDatas.Add(outboundSampleItemController);
                go.transform.SetParent(content);
            }
            Debug.Log(outboundInfoRoot.result.herdsmanList.Count);
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
    /// <summary>
    /// ���ⰴť���
    /// </summary>
    public void outboundBtnClick()
    {
        Debug.Log("���ⰴť�����" + listDatas.Count);
        int num = 0;
        foreach (var item in listDatas)
        {
            if (item.data.detectSampleNum - item.data.abnormalSampleNum - item.data.hasInspectedNum != 0)
            {
                num++;
               
            }
        }
        alert.setAlertInfo("����"+num+"�������ݲ�ȫ��");
        if (!baseAlertPanel.activeInHierarchy)
        {
            baseAlertPanel.SetActive(true);
            //���°󶨵����ĵ���¼�
            alert.returnBtn.onClick.AddListener(() =>alert.hideSelf());
            alert.continueBtn.onClick.AddListener(() =>confirmOut());
        }
        //Messagebox.MessageBox(IntPtr.Zero, "��Ӧ��������ȷ�������ύ��", "��ʾ��", 0);
        
        Debug.Log("�ɷ��ͣ�");
    }
    //ȷ�ϳ����װ
    private void confirmOut() 
    {
        administratorConfirmPan.GetComponent<OutboundPatchConfirm>().initResultInfo(infoRes);
        gameObject.SetActive(false);
        administratorConfirmPan.SetActive(true);
        alert.gameObject.SetActive(false);
    }
    //����
    public new void  initLoadPanel() 
    {
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        loadPanel = GameObject.FindGameObjectWithTag("AnimationPanel").transform.Find("LoadPanel").gameObject;
        baseAlertPanel = GameObject.FindGameObjectWithTag("AlertPanelAnchor").transform.Find("OutAlertPanel").gameObject;
        //��ʼ��ʱ����ť���м����¼��Ƴ������ж�ָ̬����ť�Ļص�����

        if (returnBtn != null)
        {
            returnBtn.onClick.RemoveAllListeners();
        }
    }
    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        bool isSampleNumber = DataCheck.isSampleNumber(tagID);
        if (m_TagTable.Contains(tagID) || !isSampleNumber)
        {
            return;
        }
        else
        {
            foreach (var item in listDatas)
            {
               
                if (item.getAtag(tagID.Substring(1, 15)))
                {
                    m_TagTable.Add(tagID, "1");
                }
            }
        }
    }
}
