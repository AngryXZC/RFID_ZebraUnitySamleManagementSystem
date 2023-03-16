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
    /// 样品编号哈希表
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 下一个界面
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
        ///每一帧读100个
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
                    //判断前11位是否为电话号码
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
                            //添加标签
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
                                    Messagebox.MessageBox(IntPtr.Zero, "出库人员刷卡失败！", "提示框", 0);
                                }
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e.ToString());
                                Messagebox.MessageBox(IntPtr.Zero, "网络连接错误！返回菜单重新进入改界面！", "提示框", 0);
                                return;
                            }

           
                        }
                    }

                }
            }
        }

    }
}
