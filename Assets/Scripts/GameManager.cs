using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    

    /// <summary>
    /// 所有的界面
    /// </summary>
    public List<Canvas> allPanels;
    /// <summary>
    /// 通道门设备
    /// </summary>
    private GangwayDoorDevice doorDevice = GangwayDoorDevice.getInstance();


    private void Awake()
    {
        Logger.Init();
    }
    /// <summary>
    /// 点击批量样品接收按钮
    /// </summary>
    public void multiSampleReciveBtnCick()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "patchPanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
                item.GetComponent<PatchPanelSeelectController>().setReciveType(ReciveEnum.MultiRecive);
            }
        }
    }
    /// <summary>
    /// 点击单户样品接收按钮
    /// </summary>
    public void SingleSampleReciveBtnCick()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "patchPanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
                item.GetComponent<PatchPanelSeelectController>().setReciveType(ReciveEnum.SingeleRcive);
            }
        }
    }

    /// <summary>
    /// 领取样品
    /// </summary>
    public void ledSamplePanel()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "ledSampleConfirm")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 交接样品
    /// </summary>
    public void passSamplePanel()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "passSamplePanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 完成清洗
    /// </summary>
    public void finishCleanSamplePanel()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "cleanManIdentifyPanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 样品清点
    /// </summary>
    public void sampleCountClick()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "sampleCount")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
                item.GetComponent<SampleCountPanelController>().enabled = true;    
            }
        }
    }
    public void returenTomenu()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "menuPanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 样品出库
    /// </summary>
    public void outboundPanel()
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "OutboundPanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 样品退还点击
    /// </summary>
    public void sampleBackPanel() 
    {
        foreach (var item in allPanels)
        {
            if (item.tag != "sampleBackPanel")
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    
}
