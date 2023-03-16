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
    public GameObject countItem;
    /// <summary>
    /// 子项数据绑定
    /// </summary>
    public List<SampleCountItem> countItemList;
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
        doorDevice = GangwayDoorDevice.getInstance();
        doorDevice.StartInventory();
        //注册读取事件监听函数
        doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(itemEvents_ReadNotify);


    }
    private void OnDisable()
    {
        StopAllCoroutines();
        //注册读取事件监听函数
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
        //同步销毁
        while (content.childCount!=0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        //异步销毁
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    Destroy(content.GetChild(i).gameObject);
        //}

    }
    //构建Item
    private void LateUpdate()
    {

        readManTags();
    }
   
    /// <summary>
    /// 根据标签识别的状态生成不同的牧民Item数量
    /// </summary>
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
                                res = NetTools.HttpGet(IPAddressConfig.getAtagInfo, "sampleCode=" + tag.TagID.Substring(1, 15));
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e);
                                Messagebox.MessageBox(IntPtr.Zero, "网络请求发生错误！", "提示框", 0);
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
                                            Debug.Log("命中！");
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
                            //    Debug.Log("命中！");
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

    //更新Item
    internal void itemEvents_ReadNotify(object sender, Events.ReadEventArgs e)
    {
        string tagID = e.ReadEventData.TagData.TagID;
        // Debug.Log(tagID);
        //判断当前标签位是否为样品编号码
        bool isNumber = DataCheck.isSampleNumber(tagID);
        if (tagID.Length > 16)
        {
            if (m_TagTable.Contains(tagID.Substring(1, 15)) || !isNumber)
            {
                return;
            }
            else
            {
                //新标签
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
