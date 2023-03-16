using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class LedSamController : BasePanel
{
#pragma warning disable 0414
    /// <summary>
    /// ����alert�Ľű�
    /// </summary>
    private AlertController alert;
    bool isStartCoroutine = true;
    /// <summary>
    /// ͨ�����豸
    /// </summary>
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// �洢ʶ�����Ʒ�Ĺ�ϣ��
    /// </summary>

    private Hashtable m_TagTable = new Hashtable();
    /// <summary>
    /// ����ȡ����Ʒ��Ϣ
    /// </summary>
    private CleanManIdentifyRes ledInfo;
    /// <summary>
    /// ��ʼ��ȡ��ť
    /// </summary>
    public Button ledSampleBtn;
    /// <summary>
    /// �б�����
    /// </summary>
    public GameObject ledSampleItem;
    /// <summary>
    /// �б��ڵ�
    /// </summary>
    public Transform content;
    /// <summary>
    /// �������
    /// </summary>
    private int number = 0;
    /// <summary>
    /// �б�
    /// </summary>
    private List<LedSamItem> ledSamItems = new List<LedSamItem>();
    /// <summary>
    /// �ⷿ������Աȷ�����
    /// </summary>
    public GameObject warehouseManPanel;
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(()=>onReturnBtnlick());
        doorDevice = GangwayDoorDevice.getInstance();
    }
    private void Update()
    {
        Debug.Log(m_TagTable.Count);
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveAllListeners();
        number = 0;
        //ͬ������
        while (content.childCount!=0) 
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        //�첽����
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    Destroy(content.GetChild(i).gameObject);
        //}
        m_TagTable.Clear();
        //ȡ��ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        isStartCoroutine = true;
        ledInfo = null;
        
        ledSamItems.Clear();
       
        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        ledSampleBtn.GetComponentInChildren<Text>().text = "��ʼ����";
    }
    // Update is called once per frame

    public void ledSamBtnClick()
    {
        if (ledSampleBtn.GetComponentInChildren<Text>().text == "��ʼ����")
        {
            ledSampleBtn.GetComponentInChildren<Text>().text = "�������";
            if (doorDevice.IsConnect && (!doorDevice.IsInventory))
            {
                doorDevice.StartInventory();

                //ע���ȡ�¼���������
                doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
            }

        }
        else
        {
            List<CleanHerdsmanListItem> result=new List<CleanHerdsmanListItem>();
            //doorDevice.StopInventory();
            ////ȡ��ע���ȡ�¼����������ô���������ˣ�����֮��������⡣����ԭ��Ŀǰ�������
            //doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
            Debug.Log("У�����ݣ�");
            foreach (LedSamItem item in ledSamItems)
            {
                if (item.data.detectSampleNum == item.data.reciveSampleNum)
                {
                    Debug.Log("�û����룡��");
                    result.Add(item.data);
                }
            }
            if (result.Count<=0)
            {
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
                alert.setAlertInfo("û�м�⵽������������Ϣ��");
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                return;
                //Messagebox.MessageBox(IntPtr.Zero, "��Ӧ����������ȷ��", "��ʾ��", 0);
            }
            //if (content.childCount <= 0)
            //{
            //    alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            //    alert.setAlertInfo("�������ύ�����ݣ�");
            //    if (!baseAlertPanel.activeInHierarchy)
            //    {
            //        baseAlertPanel.SetActive(true);
            //    }
            //    //Messagebox.MessageBox(IntPtr.Zero, "�������ύ�����ݣ�", "��ʾ��", 0);
            //    Debug.Log("���ݲ���ȷ��");
            //    return;
            //}
            Debug.Log("������ȷ��");
            ///�ⷿ������Աˢ��
            ///ledInfo���͹�ȥ
            ledInfo.herdmanList = result;
            warehouseManPanel.GetComponent<LedSamWarehouseConf>().setInfo(ledInfo);
            gameObject.SetActive(false);
            warehouseManPanel.SetActive(true);

        }
    }
    public void init(CleanManIdentifyRes result)
    {

        ledInfo = result;
        //�����б�
        foreach (var item in ledInfo.herdmanList)
        {
            number++;
            GameObject go = Instantiate(ledSampleItem);
            go.transform.SetParent(content);
            go.transform.localScale = new Vector3(1, 1, 0);
            LedSamItem temp = go.GetComponent<LedSamItem>();
            temp.setInfo(number.ToString(), item);
            ledSamItems.Add(temp);
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
            foreach (LedSamItem ledSamItem in ledSamItems)
            {

                if (ledSamItem.getATag(tagID.Substring(1, 15)))
                {
                    m_TagTable.Add(tagID, "1");
                }


            }

        }
    }
}
