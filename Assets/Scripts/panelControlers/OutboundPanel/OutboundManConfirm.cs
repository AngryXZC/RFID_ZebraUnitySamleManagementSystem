using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;

public class OutboundManConfirm : BasePanel
{
    /// <summary>
    /// ��Ʒ��Ź�ϣ��
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// ��һ������
    /// </summary>
    public GameObject outboundPanel;

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
    }


    private void Update()
    {
        readManTags();
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
                                res = NetTools.HttpGet(IPAddressConfig.getManNameByPhone, "executor_id=" + tag.TagID.Substring(0, 11));
                                OutboundRoot outboundRoot = JsonMapper.ToObject<OutboundRoot>(res);
                                Debug.Log(res);
                                if (outboundRoot.success)
                                {
                                    
                                    OutboundPanelController outbound =outboundPanel.GetComponent<OutboundPanelController>();
                                    Debug.Log(outbound);
                                    outbound.setOutboundMan(outboundRoot.result);
                                    gameObject.SetActive(false);
                                    outboundPanel.GetComponent<OutboundPanelController>().enabled = true;
                                    outboundPanel.SetActive(true);
                                }
                                else
                                {
                                    Messagebox.MessageBox(IntPtr.Zero, "������Աˢ��ʧ�ܣ�", "��ʾ��", 0);
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
