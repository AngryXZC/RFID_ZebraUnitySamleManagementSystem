using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    

    /// <summary>
    /// ���еĽ���
    /// </summary>
    public List<Canvas> allPanels;
    /// <summary>
    /// ͨ�����豸
    /// </summary>
    private GangwayDoorDevice doorDevice = GangwayDoorDevice.getInstance();


    private void Awake()
    {
        Logger.Init();
    }
    /// <summary>
    /// ���������Ʒ���հ�ť
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
    /// ���������Ʒ���հ�ť
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
    /// ��ȡ��Ʒ
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
    /// ������Ʒ
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
    /// �����ϴ
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
    /// ��Ʒ���
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
    /// ��Ʒ����
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
    /// ��Ʒ�˻����
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
