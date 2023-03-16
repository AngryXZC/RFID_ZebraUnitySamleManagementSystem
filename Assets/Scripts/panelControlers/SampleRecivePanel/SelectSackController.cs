using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectSackController : BasePanel
{
    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 库房信息
    /// </summary>
    WarehouseAndRackRoot warehouseAndRackRoot;
    /// <summary>
    /// 发送请求协程
    /// </summary>
    private Coroutine coroutine;
    /// <summary>
    /// 袋号下拉框
    /// </summary>
    public Dropdown bagDropdown;
    private List<string> bagList = new List<string>();
    /// <summary>
    /// 确定按钮
    /// </summary>
    public Button confirmBtn;
    private void OnEnable()
    {
        bagDropdown.options.Clear();
        initLoadPanel();
        ///设置alert相关
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.hideSelf());

        coroutine = StartCoroutine(UntiyGet(IPAddressConfig.warehouseAndrack, ""));
        playLoadAni();
    }
    private void OnDisable()
    {
        bagList.Clear();
        bagDropdown?.options.Clear();
        alert.returnBtn?.onClick.RemoveAllListeners();
        returnBtn?.onClick.RemoveAllListeners();
        confirmBtn?.onClick.RemoveAllListeners();
    }
    /// <summary>
    /// 绑定返回按钮
    /// </summary>
    public void bindReturnButton() 
    {
        gameObject.SetActive(false);
    }

    public void bindConfirmBtntton(BagInfoItemController herdsman,SackData currentSack)
    {
        if (currentSack.sackCode == bagDropdown.captionText.text)
        {
            herdsman.data.bagCode = bagDropdown.captionText.text;
            herdsman.bagcode.text = bagDropdown.captionText.text;
            herdsman.bagcode.color = Color.white;
            gameObject.SetActive(false);
        }
        else 
        {
            herdsman.data.bagCode = bagDropdown.captionText.text;
            herdsman.bagcode.text = bagDropdown.captionText.text;
            herdsman.bagcode.color = Color.red;
            gameObject.SetActive(false);
        }
        
    }
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
           alert.setAlertInfo(webRequest.error);
           baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
        }
        else
        {

            try
            {
                stopLoadAni();
                //WarehouseRoot warehouseRoot= JsonMapper.ToObject<WarehouseRoot>(webRequest.downloadHandler.text);
                //Debug.Log(warehouseRoot.success);
                warehouseAndRackRoot = JsonMapper.ToObject<WarehouseAndRackRoot>(webRequest.downloadHandler.text);
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
}
