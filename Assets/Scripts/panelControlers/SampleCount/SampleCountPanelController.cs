using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using System.Threading;
using System;

public class SampleCountPanelController : MonoBehaviour
{
    /// <summary>
    /// �豸
    /// </summary>
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// �б��ڵ�
    /// </summary>
    public Transform content;
    /// <summary>
    /// ����
    /// </summary>
    public GameObject countItem;
    /// <summary>
    /// �������ݰ�
    /// </summary>
    public List<SampleCountItem> countItemList;
    /// <summary>
    /// ��Ʒ��ϣ��
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    /// <summary>
    /// �����ϣ��
    /// </summary>
    private Hashtable m_herdsmanTable = new Hashtable();

    private void OnEnable()
    {
        doorDevice = GangwayDoorDevice.getInstance();
        doorDevice.StartInventory();
        //ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(itemEvents_ReadNotify);


    }
    private void OnDisable()
    {
        StopAllCoroutines();
        //ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(itemEvents_ReadNotify);

        countItemList.Clear();

        m_herdsmanTable.Clear();
        m_TagTable.Clear();
        if (doorDevice != null)
        {
            if (doorDevice.IsInventory)
            {
                doorDevice.StopInventory();
            }
        }
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

    }
    //����Item
    private void LateUpdate()
    {

        readManTags();
    }
   
    /// <summary>
    /// ���ݱ�ǩʶ���״̬���ɲ�ͬ������Item����
    /// </summary>
    private void readManTags()
    {
        ///<summary>
        ///ÿһ֡��100��
        ///</summary>
        TagData[] manTagData = manTagData = doorDevice.ReaderAPI.Actions.GetReadTags(100);
        if (manTagData == null)
            return;
        for (int nIndex = 0; nIndex < manTagData.Length; nIndex++)
        {
            TagData tag = null;
            if (manTagData[nIndex] == null)
            {
                continue;
            }
            else
            {
                tag = manTagData[nIndex];
                if (tag.TagID.Length > 16)
                {
                    //�жϵ�ǰ��ǩλ�Ƿ�Ϊ��Ʒ�����
                    bool isNumber = DataCheck.isSampleNumber(tag.TagID);
                    if (m_herdsmanTable.Contains(tag.TagID.Substring(5, 7)) || !isNumber)
                    {
                        continue;
                    }
                    else
                    {
                        //�±�ǩ
                        if (!m_herdsmanTable.Contains(tag.TagID.Substring(5, 7)))
                        {

                            string res = string.Empty;
                            try
                            {
                                res = NetTools.HttpGet(IPAddressConfig.getAtagInfo, "sampleCode=" + tag.TagID.Substring(1, 15));
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e);
                                Messagebox.MessageBox(IntPtr.Zero, "��������������", "��ʾ��", 0);
                                this.enabled = false;
                                return;
                            }
                            try
                            {
                                CountSampleRoot countSampleRoot = JsonMapper.ToObject<CountSampleRoot>(res);
                                if (countSampleRoot.success && countSampleRoot.result.Count > 0)
                                {
                                    foreach (var temp in countSampleRoot.result)
                                    {
                                        if (temp.reciveSampleList != null && temp.reciveSampleList.Count > 0)
                                        {
                                            Debug.Log("���У�");
                                            if (!m_herdsmanTable.Contains(tag.TagID.Substring(5, 7)))
                                            {
                                                m_herdsmanTable.Add(tag.TagID.Substring(5, 7), "1");
                                            }

                                            GameObject go = Instantiate(countItem);
                                            SampleCountItem item = go.GetComponent<SampleCountItem>();
                                            countItemList.Add(item);
                                            item.init(countItemList.Count.ToString(), temp);
                                            go.transform.SetParent(content);
                                        }
                                    }

                                }
                            }
                            catch (Exception)
                            {
                                return;
                              
                            }
                           
                            //if (countSampleRoot.success && countSampleRoot.result.reciveSampleList != null && countSampleRoot.result.reciveSampleList.Count > 0)
                            //{
                            //    Debug.Log("���У�");
                            //    m_herdsmanTable.Add(tag.TagID.Substring(5, 7), "1");
                            //    GameObject go = Instantiate(countItem);
                            //    SampleCountItem temp = go.GetComponent<SampleCountItem>();
                            //    countItemList.Add(temp);
                            //    temp.init(countItemList.Count.ToString(), countSampleRoot.result);
                            //    go.transform.SetParent(content);
                            //}
                        }

                    }
                }
            }

        }



    }

    //����Item
    internal void itemEvents_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        // Debug.Log(tagID);
        //�жϵ�ǰ��ǩλ�Ƿ�Ϊ��Ʒ�����
        bool isNumber = DataCheck.isSampleNumber(tagID);
        if (tagID.Length > 16)
        {
            if (m_TagTable.Contains(tagID.Substring(1, 15)) || !isNumber)
            {
                return;
            }
            else
            {
                //�±�ǩ
                if (!m_TagTable.Contains(tagID.Substring(1, 15)))
                {
                    if (countItemList.Count > 0)
                        foreach (SampleCountItem item in countItemList)
                        {
                            item.getAtag(tagID.Substring(1, 15));
                        }
                }

            }
        }

    }

    public void reDetect()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }


}
