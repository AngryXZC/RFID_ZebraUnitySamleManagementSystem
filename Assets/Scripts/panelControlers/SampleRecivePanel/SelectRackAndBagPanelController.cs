using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;

public class SelectRackAndBagPanelController : BasePanel
{
    public GameObject patchPanel;
    /// <summary>
    /// �ⷿ���ܴ���������
    /// </summary>
    public Dropdown roomDropdown;
    public Dropdown rackDropdown;
    public Dropdown bagDropdown;
    /// <summary>
    /// ���������ݰ�
    /// </summary>
    private List<string> roomList=new List<string>();
    private List<string> rackList=new List<string>();
    private List<string> bagList=new List<string>();
    /// <summary>
    /// ȷ�ϰ�ť
    /// </summary>
    public Button confirmBtn;
    /// <summary>
    /// ��������Э��
    /// </summary>
    private Coroutine coroutine;
    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    //������Ʒ����ҳ��
    public GameObject singleRecivePanel;
    /// <summary>
    /// �����������ݿ���
    /// </summary>
    SingleSampleReciveController singleSampleReciveController;
    /// <summary>
    /// ��ǰ������Ϣ
    /// </summary>
    private SampleInfo sampleInfo;
    /// <summary>
    /// ��ǰ������
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    /// ��ǰ��������
    /// </summary>
    private string currentPatchTitle;
    /// <summary>
    /// ��������
    /// </summary>
    public ReciveEnum reciveType;
    /// <summary>
    /// �ⷿ��Ϣ
    /// </summary>
    WarehouseAndRackRoot warehouseAndRackRoot;
    private void OnEnable()
    {
        parentPanel=patchPanel.gameObject;
        confirmBtn.onClick.AddListener(()=> confimBtnClick());
        singleSampleReciveController = singleRecivePanel.GetComponent<SingleSampleReciveController>();
        if (roomDropdown!=null&&rackDropdown!=null&&bagDropdown!=null)
        {
            roomDropdown.options.Clear();
            rackDropdown.options.Clear();
            bagDropdown.options.Clear();
        }
        
        initLoadPanel();
        ///����alert����ڱ�ҳ��һ�����ؾ��Ƿ��ص����ν����б���
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
        returnBtn.onClick.AddListener(()=> returnBtnClick());
        coroutine= StartCoroutine(UntiyGet(IPAddressConfig.warehouseAndrack,""));
        playLoadAni();
    }

    private void OnDisable()
    {
        confirmBtn.onClick.RemoveAllListeners();
        roomList.Clear();
        rackList.Clear();
        bagList.Clear();
        if (coroutine!=null)
        {
            StopCoroutine(coroutine);
        }
        alert.returnBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveListener(() => returnBtnClick());
    }
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
        }
        else
        {
            
            try
            {
                stopLoadAni();
                //WarehouseRoot warehouseRoot= JsonMapper.ToObject<WarehouseRoot>(webRequest.downloadHandler.text);
                //Debug.Log(warehouseRoot.success);
                warehouseAndRackRoot=JsonMapper.ToObject<WarehouseAndRackRoot>(webRequest.downloadHandler.text);
                Debug.Log(warehouseAndRackRoot.success);
                foreach (var item in warehouseAndRackRoot.result.Sack) 
                {
                    bagList.Add(item.sackTitle);
                }
                bagDropdown.AddOptions(bagList);
            
            }
            catch (System.Exception ex)
            {
                //Messagebox.MessageBox(IntPtr.Zero, ex.ToString(), "��ʾ", 0);
                Debug.Log(ex.ToString());
                alert.setAlertInfo(ex.ToString());
                if (!baseAlertPanel.activeInHierarchy)
                {
                    baseAlertPanel.SetActive(true);
                }
                stopLoadAni();
            }
            Debug.Log(webRequest.downloadHandler.text);
        }

    }
    /// <summary>
    /// ��һ�ε��ȷ����ť
    /// </summary>
    private void confimBtnClick() 
    {
        singleSampleReciveController=singleRecivePanel.GetComponent<SingleSampleReciveController>();
        singleSampleReciveController.setcurrentData(currentApplyCode, sampleInfo, reciveType);
        singleSampleReciveController.setCurreneBag(bagDropdown.captionText.text);
        singleSampleReciveController.setWarehouseInfo(warehouseAndRackRoot);
        gameObject.SetActive(false);
        singleRecivePanel.gameObject.SetActive(true);
        Debug.Log( bagDropdown.captionText.text);
    }
    public void setInfo(string apply_code,string patch_title, SampleInfo info,ReciveEnum recive) 
    {
        currentApplyCode = apply_code;
        currentPatchTitle = patch_title;
        sampleInfo = info;
        info.result.patchTitle = patch_title;
        reciveType = recive;
    }
    /// <summary>
    /// �ڽ������ѡ���������
    /// </summary>
    public void changeBagByClickConfirm()
    {
        singleSampleReciveController.setCurreneBag(bagDropdown.captionText.text);
        Debug.Log("�������ӣ�" + bagDropdown.captionText.text);
        gameObject.SetActive(false);
        singleSampleReciveController.doorDevice.StartInventory();
    }

    public void returnBtnClick()
    {
        if(singleSampleReciveController.doorDevice != null) 
        {
            singleSampleReciveController.doorDevice.StartInventory();
        }
       
        onReturnBtnlick();
    }

}
