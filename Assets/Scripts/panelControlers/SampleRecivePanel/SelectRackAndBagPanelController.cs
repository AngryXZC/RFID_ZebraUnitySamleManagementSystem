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
    /// 库房货架袋号下拉框
    /// </summary>
    public Dropdown roomDropdown;
    public Dropdown rackDropdown;
    public Dropdown bagDropdown;
    /// <summary>
    /// 下拉框数据绑定
    /// </summary>
    private List<string> roomList=new List<string>();
    private List<string> rackList=new List<string>();
    private List<string> bagList=new List<string>();
    /// <summary>
    /// 确认按钮
    /// </summary>
    public Button confirmBtn;
    /// <summary>
    /// 发送请求协程
    /// </summary>
    private Coroutine coroutine;
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    //单个样品接收页面
    public GameObject singleRecivePanel;
    /// <summary>
    /// 单户接收数据控制
    /// </summary>
    SingleSampleReciveController singleSampleReciveController;
    /// <summary>
    /// 当前批次信息
    /// </summary>
    private SampleInfo sampleInfo;
    /// <summary>
    /// 当前受理单号
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    /// 当前批次名字
    /// </summary>
    private string currentPatchTitle;
    /// <summary>
    /// 接收类型
    /// </summary>
    public ReciveEnum reciveType;
    /// <summary>
    /// 库房信息
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
        ///设置alert相关在本页面一旦返回就是返回到批次接收列表了
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
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
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
                //Messagebox.MessageBox(IntPtr.Zero, ex.ToString(), "提示", 0);
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
    /// 第一次点击确定按钮
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
    /// 在接收面板选择更换袋号
    /// </summary>
    public void changeBagByClickConfirm()
    {
        singleSampleReciveController.setCurreneBag(bagDropdown.captionText.text);
        Debug.Log("更换袋子！" + bagDropdown.captionText.text);
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
