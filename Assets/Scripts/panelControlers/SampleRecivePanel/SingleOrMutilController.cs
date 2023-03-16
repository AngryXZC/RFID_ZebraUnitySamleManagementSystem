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
    /// ����ѡ��
    /// </summary>
    /// 
    public GameObject bagCodePanel;
    /// <summary>
    /// ��������
    /// </summary>
    public ReciveEnum reciveType;
    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;

    //�������հ�ť
    public Button singleReciveBtn;
    //�������հ�ť
    public Button multiReciveBtn;
    //����������Ʒҳ��
    public GameObject multiRecivePanel;
    
    //������Ʒ����ҳ��
    public GameObject singleRecivePanel;
    /// <summary>
    /// ��ǰ������
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    /// ��ǰ��������
    /// </summary>
    private string currentPatchTitle;
    /// <summary>
    /// ��ǰ������Ϣ
    /// </summary>
    private SampleInfo sampleInfo;
    private Coroutine coroutine;
    /// <summary>
    /// �����������ݿ���
    /// </summary>
    SampleReciveController sampleReciveController;
    /// <summary>
    /// ѡ��ⷿ���ܴ���
    /// </summary>
    SelectRackAndBagPanelController selectRackAndBagPanelController;

    private void OnEnable()
    {
        sampleReciveController = multiRecivePanel.GetComponent<SampleReciveController>();
       
        bagCodePanel = GameObject.Find("BagCodeAnchor").transform.Find("BagcodePanel").gameObject;
        selectRackAndBagPanelController=bagCodePanel.GetComponent<SelectRackAndBagPanelController>();
        multiReciveBtn.onClick.AddListener(()=> multiReciveBtnClick());
        initLoadPanel();
        ///����alert����ڱ�ҳ��һ�����ؾ��Ƿ��ص����ν����б���
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
    /// ����������Ʒ
    /// </summary>
    public void singleReciveBtnClick() 
    {
        selectRackAndBagPanelController.setInfo(currentApplyCode,currentPatchTitle, sampleInfo, reciveType);
        gameObject.SetActive(false);
        bagCodePanel.SetActive(true);
       
        
    }
    /// <summary>
    /// ����������Ʒ
    /// </summary>
    public void multiReciveBtnClick() 
    {
        Debug.Log("�������գ�");
        sampleReciveController.setcurrentData(currentApplyCode, currentPatchTitle,sampleInfo, reciveType);
        gameObject.SetActive(false);
        multiRecivePanel.gameObject.SetActive(true);
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
            sampleInfo = JsonMapper.ToObject<SampleInfo>(webRequest.downloadHandler.text);
            stopLoadAni();
            try
            {
                //��ȡ��������
                if (sampleInfo.success)
                {
                    Debug.Log("�����ν�����ȷ��");
                    switch (reciveType)
                    {
                        case ReciveEnum.SingeleRcive:
                            //����ǵ���������Ҫ�������
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
                    alert.setAlertInfo("�����ν��մ���");
                    if (!baseAlertPanel.activeInHierarchy) 
                    {
                        baseAlertPanel.SetActive(true);
                    }
                    Debug.Log("�����ν��մ���");
                }
            }
            catch (System.Exception ex)
            {
                //Messagebox.MessageBox(IntPtr.Zero, ex.ToString(), "��ʾ", 0);
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
