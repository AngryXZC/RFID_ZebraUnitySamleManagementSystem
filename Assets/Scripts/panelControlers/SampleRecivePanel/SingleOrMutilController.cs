using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SingleOrMutilController : BasePanel
{
    /// <summary>
    /// 袋号选择
    /// </summary>
    /// 
    public GameObject bagCodePanel;
    /// <summary>
    /// 接收类型
    /// </summary>
    public ReciveEnum reciveType;
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;

    //单户接收按钮
    public Button singleReciveBtn;
    //批量接收按钮
    public Button multiReciveBtn;
    //批量接收样品页面
    public GameObject multiRecivePanel;
    
    //单个样品接收页面
    public GameObject singleRecivePanel;
    /// <summary>
    /// 当前受理单号
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    /// 当前批次名称
    /// </summary>
    private string currentPatchTitle;
    /// <summary>
    /// 当前批次信息
    /// </summary>
    private SampleInfo sampleInfo;
    private Coroutine coroutine;
    /// <summary>
    /// 批量接收数据控制
    /// </summary>
    SampleReciveController sampleReciveController;
    /// <summary>
    /// 选择库房货架袋号
    /// </summary>
    SelectRackAndBagPanelController selectRackAndBagPanelController;

    private void OnEnable()
    {
        sampleReciveController = multiRecivePanel.GetComponent<SampleReciveController>();
       
        bagCodePanel = GameObject.Find("BagCodeAnchor").transform.Find("BagcodePanel").gameObject;
        selectRackAndBagPanelController=bagCodePanel.GetComponent<SelectRackAndBagPanelController>();
        multiReciveBtn.onClick.AddListener(()=> multiReciveBtnClick());
        initLoadPanel();
        ///设置alert相关在本页面一旦返回就是返回到批次接收列表了
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, parentPanel));
        returnBtn.onClick.AddListener(onReturnBtnlick);
        multiRecivePanel =GameObject.Find("ReciveAnchor").transform.Find("SampleRecivePanel").gameObject;
        if (currentApplyCode != String.Empty)
        {
            playLoadAni();
            coroutine= StartCoroutine(UntiyGet(IPAddressConfig.sampleReciveAdderss, "apply_code=" + currentApplyCode));
        }
    }
    private void OnDisable()
    {
        selectRackAndBagPanelController = null;
        currentApplyCode =String.Empty;
        sampleInfo=null;
        returnBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        multiReciveBtn.onClick.RemoveAllListeners();
        if (coroutine!=null)
        {
            StopCoroutine(coroutine);
        }
        sampleReciveController = null;
      
    }
    public void setcurrentApplyCode(string apply_code,string current_patch_title)
    {
        currentApplyCode = apply_code;
        currentPatchTitle = current_patch_title;
    }
    /// <summary>
    /// 单户接收样品
    /// </summary>
    public void singleReciveBtnClick() 
    {
        selectRackAndBagPanelController.setInfo(currentApplyCode,currentPatchTitle, sampleInfo, reciveType);
        gameObject.SetActive(false);
        bagCodePanel.SetActive(true);
       
        
    }
    /// <summary>
    /// 批量接收样品
    /// </summary>
    public void multiReciveBtnClick() 
    {
        Debug.Log("批量接收！");
        sampleReciveController.setcurrentData(currentApplyCode, currentPatchTitle,sampleInfo, reciveType);
        gameObject.SetActive(false);
        multiRecivePanel.gameObject.SetActive(true);
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
            sampleInfo = JsonMapper.ToObject<SampleInfo>(webRequest.downloadHandler.text);
            stopLoadAni();
            try
            {
                //收取数据正常
                if (sampleInfo.success)
                {
                    Debug.Log("该批次接收正确！");
                    switch (reciveType)
                    {
                        case ReciveEnum.SingeleRcive:
                            //如果是单户接收需要输入袋号
                            singleReciveBtnClick();
                            break;
                        case ReciveEnum.MultiRecive:
                            multiReciveBtnClick();
                            break;
                        default:
                            break;
                    }
                  
                }
                else
                {
                    alert.setAlertInfo("该批次接收错误");
                    if (!baseAlertPanel.activeInHierarchy) 
                    {
                        baseAlertPanel.SetActive(true);
                    }
                    Debug.Log("该批次接收错误！");
                }
            }
            catch (System.Exception ex)
            {
                //Messagebox.MessageBox(IntPtr.Zero, ex.ToString(), "提示", 0);
                //Debug.Log(ex.ToString());
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

}
