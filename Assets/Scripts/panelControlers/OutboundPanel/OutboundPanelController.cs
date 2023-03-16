using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class OutboundPanelController : BasePanel
{
    //UI
    public GameObject outboundAdminConPanel;
    /// <summary>
    /// ������Ա
    /// </summary>
    public Text outboundManText;
    /// <summary>
    /// ��������
    /// </summary>
    public Text outboundDateTet;
    /// <summary>
    /// �������
    /// </summary>
    public Text applyCode;
    /// <summary>
    /// �ͼ쵥λ
    /// </summary>
    public Text inspectDepartment;
    /// <summary>
    /// ����ͳ��
    /// </summary>
    public Text outboundSampleCount;
    /// <summary>
    /// ������Ա
    /// </summary>
    private OutboundResult outnoundMan;
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
    public GameObject outboundItem;
    /// <summary>
    /// �������ݰ�
    /// </summary>
    public List<OutboundItemController> outboundItemDatas;
    /// <summary>
    /// ���ⷵ��
    /// </summary>
    public FinishOutboundRes outboundFinisheData=new FinishOutboundRes();
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
       
        outboundSampleCount.text = "������Ʒͳ�ƣ�0";
        doorDevice = GangwayDoorDevice.getInstance();
        doorDevice.StartInventory();
    }
    private void OnDisable()
    {
        outboundItemDatas.Clear();
        
        m_herdsmanTable.Clear();
        m_TagTable.Clear();
        if (doorDevice != null)
        {
            if (doorDevice.IsInventory)
            {
                doorDevice.StopInventory();
            }
        }
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }


    //����Item
    private void Update()
    {
        readManTags();
    }
    //����Item
    private void FixedUpdate()
    {

        outboundSampleCount.text= "������Ʒͳ��:" + m_TagTable.Count.ToString();
        readSampleTag();
    }
    public void  setOutboundMan(OutboundResult man)
    {
        this.outnoundMan = man;
        outboundManText.text = "������Ա:"+man.manName.ToString();
        
        outboundFinisheData.outboundFinisheDatas = new List<OutboundListRes>();
        outboundFinisheData.operatormanPhone=man.manPhoneNum.ToString();
        outboundDateTet.text = "�������ڣ�"+ string.Format("{0:D}", System.DateTime.Now);//2005-11-5;
    }
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
                                res = NetTools.HttpGet(IPAddressConfig.outboundQuery, "sampleCode=" + tag.TagID.Substring(1, 15));
                               
                            }
                            catch (System.Exception)
                            {

                                Messagebox.MessageBox(IntPtr.Zero, "��������������", "��ʾ��", 0);
                                //��֪������
                                this.enabled = false;
                                return;
                            }
                            Debug.Log(res);
                            OutboundListRoot outboundListRoot = JsonMapper.ToObject<OutboundListRoot>(res);
                            if (outboundListRoot.success&& outboundListRoot.result.reciveSampleList!=null&& outboundListRoot.result.reciveSampleList.Count>0)
                            {
                                Debug.Log("��ȷ����"+tag.TagID.Substring(1, 15));
                                m_herdsmanTable.Add(tag.TagID.Substring(5, 7), "1");

                                GameObject go = Instantiate(outboundItem);
                                OutboundItemController temp = go.GetComponent<OutboundItemController>();
                                applyCode.text = "No."+outboundListRoot.result.applyCode;
                                inspectDepartment.text = "�ͼ쵥λ��"+outboundListRoot.result.HusbandryBureauName;

                                outboundItemDatas.Add(temp);
                                temp.init(outboundItemDatas.Count.ToString(), outboundListRoot.result);
                                go.transform.SetParent(content);
                            }
                        }

                    }
                }
            }

        }



    }


    /// <summary>
    /// ����ʶ��ǩ�������Ӧ״̬
    /// </summary>
    private void readSampleTag()
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
                    if (m_TagTable.Contains(tag.TagID.Substring(1, 15)) || !isNumber)
                    {
                        continue;
                    }
                    else
                    {
                        //�±�ǩ
                        if (!m_TagTable.Contains(tag.TagID.Substring(1, 15)))
                        {
                            
                            if (outboundItemDatas.Count > 0) 
                            {
                                foreach (OutboundItemController item in outboundItemDatas)
                                {
                                   // Debug.Log(tag.TagID.Substring(1, 15));
                                    if (item.getAtag(tag.TagID.Substring(1, 15)))
                                    {
                                        m_TagTable.Add(tag.TagID.Substring(1, 15), "1");

                                    }
                                }
                            }
                                
                        }

                    }
                }
            }

        }

    }
    /// <summary>
    /// ����ȷ�ϵ��
    /// </summary>
    public void outboundBtnClick() 
    {
        bool hasExcepetion = false;
        foreach (OutboundItemController item in outboundItemDatas)
        {
            if (item.data.detectSampleNum - item.data.reciveSampleNum != 0)
            {
                Messagebox.MessageBox(IntPtr.Zero, "�����������ȷ��������⣡", "��ʾ��", 0);
                hasExcepetion = true;
                return;
            }
            else 
            {
                
                outboundFinisheData.outboundFinisheDatas.Add(item.data);
          
            }
        }
        if (!hasExcepetion) 
        {
            Debug.Log("�ⷿ����ˢ��ȷ�ϣ�");
            OutboundConfirm temp= outboundAdminConPanel.GetComponent<OutboundConfirm>();
            temp.setData(outboundFinisheData);
            gameObject.SetActive(false);
            outboundAdminConPanel.SetActive(true);
        }
        //string json=JsonMapper.ToJson(m_TagTable);
    }
}
