using System;
using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class SampleBackController : BasePanel
{
    /// <summary>
    /// ����alert�Ľű�
    /// </summary>
    AlertController alert;
    /// <summary>
    /// ����Աˢ��ȷ��
    /// </summary>
    public GameObject admiConfirm;

    /// <summary>
    /// ͨ�����豸
    /// </summary>
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// �б��ڵ�
    /// </summary>
    public Transform content;
    /// <summary>
    /// �б�����
    /// </summary>
    public GameObject backItem;
    /// <summary>
    /// ���������ť
    /// </summary>
    public Button finishBtn;
    /// <summary>
    /// �˻���Ʒ������Ϣ
    /// </summary>
    private BackSamRes backSamInfo;
    //�������ݰ�
    private List<BackSampleItem> backSampleItemList = new List<BackSampleItem>();
    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void initInfo(BackSamRes res)
    {
        
        backSamInfo= res;
        
        generateItem();
    }
    private void generateItem()
    {

        if (backSamInfo != null)
        {
            foreach (var item in backSamInfo.herdmanList)
            {
                GameObject go = Instantiate(backItem);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);
                BackSampleItem temp = go.GetComponent<BackSampleItem>();
                backSampleItemList.Add(temp);
                temp.initItem(backSampleItemList.Count, item, "");
            }

        }
    }

    private void OnDisable()
    {
        backSamInfo = null;
        alert.returnBtn.onClick.RemoveAllListeners();
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
        StopAllCoroutines();
        finishBtn.onClick.RemoveAllListeners();
        backSampleItemList.Clear();
        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        //ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        
       
    }
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();


        finishBtn.onClick.AddListener(confirmBackBtn);
        doorDevice = GangwayDoorDevice.getInstance();
        if (doorDevice.IsConnect && (!doorDevice.IsInventory))
        {
            doorDevice.StartInventory();
            //ע���ȡ�¼���������
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
    }


    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        bool isSampleNum=DataCheck.isSampleNumber(tagID);
        if (isSampleNum) 
        {
            var tag_data = tagID.Substring(1,15);
            foreach (BackSampleItem item in backSampleItemList)
            {
                item.getATag(tag_data);
            }
        }
       
    }

    public void confirmBackBtn() 
    {
        List<BackHerdsmanListItem> result= new List<BackHerdsmanListItem>();
       
        foreach (BackSampleItem item in backSampleItemList) 
        {
            if (item.myData.detectSampleNum - item.myData.abnormalBackSampleNum - item.myData.cleanSampleNum == 0)
            {

                result.Add(item.myData);
            }
          
        }
        //���ؽ���б�Ϊ�������ݲ���ȷ
        if (result.Count <= 0)
        {
            alert.setAlertInfo("û�м�⵽������������");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //���°󶨵����ĵ���¼�
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, "û��������������Ϣ�������ύ��", "��ʾ��", 0);
        }
        else 
        {
           //�����ύ
            var temp = admiConfirm.GetComponent<SampleBackAdminConfirm>();
            backSamInfo.herdmanList = result;
            temp.initData(backSamInfo);
           
            this.gameObject.SetActive(false);
            admiConfirm.SetActive(true);

        }
       
    }
}
