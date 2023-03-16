using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
public class OutboundPatchPanelController : BasePanel
{
    /// <summary>
    /// ����alert�Ľű�
    /// </summary>
    AlertController alert;
    /// <summary>
    /// �б��ڵ�
    /// </summary>
    public Transform content;
    /// <summary>
    /// �б�����
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
        //����б�
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        StopAllCoroutines();
        stopLoadAni();
        returnBtn.onClick.RemoveAllListeners();
    }
    //ʹ��Unity�����෢��
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("�������!");
            if (!baseAlertPanel.activeInHierarchy)
            {
                baseAlertPanel.SetActive(true);
                //���°󶨵����ĵ���¼�
                alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            }
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
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
