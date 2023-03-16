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
    /// 样品编号哈希表
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
        //在扫描页构建在确认页销毁
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
                               string json=Regex.Unescape(JsonMapper.ToJson(data));
                               Debug.Log(json);
                               res = NetTools.HttpPostFin(IPAddressConfig.outboundConfirm,json);
                               OutboundAdminConRoot outboundAdminConRoot= JsonMapper.ToObject<OutboundAdminConRoot>(res);
                                if (outboundAdminConRoot.success)
                                {
                                    Messagebox.MessageBox(IntPtr.Zero, "出库成功！", "提示框", 0);
                                }
                                else 
                                {
                                    Messagebox.MessageBox(IntPtr.Zero, "出库失败！"+outboundAdminConRoot.message, "提示框", 0);
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
