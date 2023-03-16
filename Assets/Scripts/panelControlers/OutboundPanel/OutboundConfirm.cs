using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;

public class OutboundConfirm : BasePanel
{
    private FinishOutboundRes data;
    /// <summary>
    /// ��Ʒ��Ź�ϣ��
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private GangwayDoorDevice doorDevice;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {

        doorDevice = GangwayDoorDevice.getInstance();
        if (doorDevice.IsConnect)
        {
            doorDevice.StartInventory();
        }
    }
    private void OnDisable()
    {
        m_TagTable.Clear();
        if (doorDevice.IsInventory) 
        {
            doorDevice.StopInventory();
        }
        //��ɨ��ҳ������ȷ��ҳ����
        data.outboundFinisheDatas.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        readManTags();
    }
    public void  setData(FinishOutboundRes res) 
    {
        data = res;
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
                if (tag.TagID.Length > 11)
                {
                    //�ж�ǰ11λ�Ƿ�Ϊ�绰����
                    string targetString = tag.TagID.Substring(0, 11);
                    bool isPhone = DataCheck.isHandset(targetString);
                    if (m_TagTable.Contains(tag.TagID) || !isPhone)
                    {
                        continue;
                    }
                    else
                    {
                        if (!m_TagTable.Contains(tag.TagID))
                        {
                            //��ӱ�ǩ
                            m_TagTable.Add(tag.TagID, "1");


                            string res = string.Empty;
                            try
                            {
                               string json=Regex.Unescape(JsonMapper.ToJson(data));
                               Debug.Log(json);
                               res = NetTools.HttpPostFin(IPAddressConfig.outboundConfirm,json);
                               OutboundAdminConRoot outboundAdminConRoot= JsonMapper.ToObject<OutboundAdminConRoot>(res);
                                if (outboundAdminConRoot.success)
                                {
                                    Messagebox.MessageBox(IntPtr.Zero, "����ɹ���", "��ʾ��", 0);
                                }
                                else 
                                {
                                    Messagebox.MessageBox(IntPtr.Zero, "����ʧ�ܣ�"+outboundAdminConRoot.message, "��ʾ��", 0);
                                }
                          
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e.ToString());
                                Messagebox.MessageBox(IntPtr.Zero, "�������Ӵ��󣡷��ز˵����½���Ľ��棡", "��ʾ��", 0);
                                return;
                            }


                        }
                    }

                }
            }
        }

    }
}
