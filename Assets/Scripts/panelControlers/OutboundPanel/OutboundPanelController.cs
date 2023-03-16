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
    /// 出库人员
    /// </summary>
    public Text outboundManText;
    /// <summary>
    /// 出库日期
    /// </summary>
    public Text outboundDateTet;
    /// <summary>
    /// 受理单编号
    /// </summary>
    public Text applyCode;
    /// <summary>
    /// 送检单位
    /// </summary>
    public Text inspectDepartment;
    /// <summary>
    /// 出库统计
    /// </summary>
    public Text outboundSampleCount;
    /// <summary>
    /// 出库人员
    /// </summary>
    private OutboundResult outnoundMan;
    /// <summary>
    /// 设备
    /// </summary>
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 列表父节点
    /// </summary>
    public Transform content;
    /// <summary>
    /// 子项
    /// </summary>
    public GameObject outboundItem;
    /// <summary>
    /// 子项数据绑定
    /// </summary>
    public List<OutboundItemController> outboundItemDatas;
    /// <summary>
    /// 出库返回
    /// </summary>
    public FinishOutboundRes outboundFinisheData=new FinishOutboundRes();
    /// <summary>
    /// 样品哈希表
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    /// <summary>
    /// 牧民哈希表
    /// </summary>
    private Hashtable m_herdsmanTable = new Hashtable();


    private void OnEnable()
    {
       
        outboundSampleCount.text = "出库样品统计：0";
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


    //构建Item
    private void Update()
    {
        readManTags();
    }
    //更新Item
    private void FixedUpdate()
    {

        outboundSampleCount.text= "出库样品统计:" + m_TagTable.Count.ToString();
        readSampleTag();
    }
    public void  setOutboundMan(OutboundResult man)
    {
        this.outnoundMan = man;
        outboundManText.text = "出库人员:"+man.manName.ToString();
        
        outboundFinisheData.outboundFinisheDatas = new List<OutboundListRes>();
        outboundFinisheData.operatormanPhone=man.manPhoneNum.ToString();
        outboundDateTet.text = "出库日期："+ string.Format("{0:D}", System.DateTime.Now);//2005-11-5;
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
                if (tag.TagID.Length > 16)
                {
                    //判断当前标签位是否为样品编号码
                    bool isNumber = DataCheck.isSampleNumber(tag.TagID);
                    if (m_herdsmanTable.Contains(tag.TagID.Substring(5, 7)) || !isNumber)
                    {
                        continue;
                    }
                    else
                    {
                        //新标签
                        if (!m_herdsmanTable.Contains(tag.TagID.Substring(5, 7)))
                        {

                            string res = string.Empty;
                            try
                            {
                                res = NetTools.HttpGet(IPAddressConfig.outboundQuery, "sampleCode=" + tag.TagID.Substring(1, 15));
                               
                            }
                            catch (System.Exception)
                            {

                                Messagebox.MessageBox(IntPtr.Zero, "网络请求发生错误！", "提示框", 0);
                                //不知道阿爸
                                this.enabled = false;
                                return;
                            }
                            Debug.Log(res);
                            OutboundListRoot outboundListRoot = JsonMapper.ToObject<OutboundListRoot>(res);
                            if (outboundListRoot.success&& outboundListRoot.result.reciveSampleList!=null&& outboundListRoot.result.reciveSampleList.Count>0)
                            {
                                Debug.Log("正确数据"+tag.TagID.Substring(1, 15));
                                m_herdsmanTable.Add(tag.TagID.Substring(5, 7), "1");

                                GameObject go = Instantiate(outboundItem);
                                OutboundItemController temp = go.GetComponent<OutboundItemController>();
                                applyCode.text = "No."+outboundListRoot.result.applyCode;
                                inspectDepartment.text = "送检单位："+outboundListRoot.result.HusbandryBureauName;

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
    /// 根据识别签的情况感应状态
    /// </summary>
    private void readSampleTag()
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
                if (tag.TagID.Length > 16)
                {
                    //判断当前标签位是否为样品编号码
                    bool isNumber = DataCheck.isSampleNumber(tag.TagID);
                    if (m_TagTable.Contains(tag.TagID.Substring(1, 15)) || !isNumber)
                    {
                        continue;
                    }
                    else
                    {
                        //新标签
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
    /// 出库确认点击
    /// </summary>
    public void outboundBtnClick() 
    {
        bool hasExcepetion = false;
        foreach (OutboundItemController item in outboundItemDatas)
        {
            if (item.data.detectSampleNum - item.data.reciveSampleNum != 0)
            {
                Messagebox.MessageBox(IntPtr.Zero, "检测数量不正确不允许出库！", "提示框", 0);
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
            Debug.Log("库房管理刷卡确认！");
            OutboundConfirm temp= outboundAdminConPanel.GetComponent<OutboundConfirm>();
            temp.setData(outboundFinisheData);
            gameObject.SetActive(false);
            outboundAdminConPanel.SetActive(true);
        }
        //string json=JsonMapper.ToJson(m_TagTable);
    }
}
