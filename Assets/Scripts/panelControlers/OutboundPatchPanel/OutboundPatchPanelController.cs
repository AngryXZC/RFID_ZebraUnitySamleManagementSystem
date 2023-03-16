using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
public class OutboundPatchPanelController : BasePanel
{
    /// <summary>
    /// 控制alert的脚本
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 列表父节点
    /// </summary>
    public Transform content;
    /// <summary>
    /// 列表子项
    /// </summary>
    public GameObject listItem;
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(onReturnBtnlick);
        StartCoroutine(UntiyGet(IPAddressConfig.getOutboundPatchPanel,""));
    }
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        //清除列表
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        StopAllCoroutines();
        stopLoadAni();
        returnBtn.onClick.RemoveAllListeners();
    }
    //使用Unity发送类发送
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("网络错误!");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //重新绑定弹窗的点击事件
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
            stopLoadAni();
        }
        else
        {

            stopLoadAni();
            OutboundPatchRoot<OutboundPatchInfo> root = JsonMapper.ToObject<OutboundPatchRoot<OutboundPatchInfo>>(webRequest.downloadHandler.text);
            Debug.Log(root.result.Count);
            int i = 0;
            foreach (OutboundPatchInfo patch in root.result) 
            {
                i++;
                GameObject go = Instantiate(listItem);
                OutboundPatchItemController temp=go.GetComponent<OutboundPatchItemController>();
                temp.initInfo(i,patch);
                go.transform.SetParent(content);
            }
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
}
