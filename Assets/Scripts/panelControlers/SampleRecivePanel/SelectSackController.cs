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
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    /// <summary>
    /// �ⷿ��Ϣ
    /// </summary>
    WarehouseAndRackRoot warehouseAndRackRoot;
    /// <summary>
    /// ��������Э��
    /// </summary>
    private Coroutine coroutine;
    /// <summary>
    /// ����������
    /// </summary>
    public Dropdown bagDropdown;
    private List<string> bagList = new List<string>();
    /// <summary>
    /// ȷ����ť
    /// </summary>
    public Button confirmBtn;
    private void OnEnable()
    {
        bagDropdown.options.Clear();
        initLoadPanel();
        ///����alert���
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
    /// �󶨷��ذ�ť
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
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
           alert.setAlertInfo(webRequest.error);
           baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
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
}
