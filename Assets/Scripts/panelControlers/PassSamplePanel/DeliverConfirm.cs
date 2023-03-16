using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;
using System;

public class DeliverConfirm : BasePanel
{
    /// <summary>
    /// 
    /// </summary>
    private Hashtable m_TagTable=new Hashtable();
    private GangwayDoorDevice doorDevice;
    //列表数据结果
    private PassSampleRes sampleRes;

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
        sampleRes=null;
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

    public void setInfo(PassSampleRes info)
    {
        this.sampleRes = info;
        Debug.Log(sampleRes);
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
                            sampleRes.deliverManPhone = tag.TagID.Substring(0, 11);
                            Debug.Log("发送数据！");
                            if (sampleRes != null)
                            {
                                string json = Regex.Unescape(JsonMapper.ToJson(sampleRes));
                                Debug.Log(json);
                                string res = string.Empty;
                                try
                                {
                                    res = NetTools.HttpPostFin(IPAddressConfig.sampleDeliverConfirm,json);
                                    Debug.Log(res);
                                    PassResRoot passResRoot = JsonMapper.ToObject<PassResRoot>(res);
                                    if (passResRoot.result.isLedSampleSuccess)
                                    {
                                        Messagebox.MessageBox(IntPtr.Zero, "转交成功！", "提示框", 0);
                                    }
                                    else
                                    {
                                        Messagebox.MessageBox(IntPtr.Zero, "转交失败！", "提示框", 0);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Debug.Log(ex);
                                    Messagebox.MessageBox(IntPtr.Zero, "转交失败！返回菜单重新进入改界面！", "提示框", 0);
                                    throw;
                                }

                                Debug.Log(res);
                            }
                        }
                       
                    }
                }
            }

        }
    }
}
