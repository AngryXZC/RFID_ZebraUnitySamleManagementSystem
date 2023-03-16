using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectSackAndRackController : MonoBehaviour
{
    public GameObject multiplyConfigePanel;

    public Dropdown sackDropDown;
    public Dropdown rackDropDown;
    private Coroutine webCorutine;
    private WarehouseAndRackRoot warehouseAndRackRoot;
    //ҳ�水ť
    public Button returnBtn;
    public Button confirmBtn;
    private List<string> sacks;
    private List<string> racks;
    private void OnEnable() {
        sacks = new List<string>();
        racks = new List<string>();
        sackDropDown.options.Clear();
        rackDropDown.options.Clear();
        webCorutine = StartCoroutine(UntiyGet(IPAddressConfig.warehouseAndrack,""));
        returnBtn.onClick.AddListener(()=>onreturnBtnClick());
        confirmBtn.onClick.AddListener(()=> onConfirmBtnClick());
    }
    private void OnDisable() {
        returnBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.RemoveAllListeners();
        StopCoroutine(webCorutine);
        sacks.Clear();
        racks.Clear();
        sackDropDown.options.Clear();
        rackDropDown.options.Clear();
    }
    public void onreturnBtnClick() {
        this.gameObject.SetActive(false);
    }
    public void onConfirmBtnClick() {
        MultiWarehouseInPanel multiWarehouse= multiplyConfigePanel.GetComponent<MultiWarehouseInPanel>();
        multiWarehouse.setRacklAndSack(sackDropDown.captionText.text,rackDropDown.captionText.text);
        this.gameObject.SetActive(false);
        Debug.Log("ȷ�������");
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
                //stopLoadAni();
                //WarehouseRoot warehouseRoot= JsonMapper.ToObject<WarehouseRoot>(webRequest.downloadHandler.text);
                //Debug.Log(warehouseRoot.success);
                warehouseAndRackRoot = JsonMapper.ToObject<WarehouseAndRackRoot>(webRequest.downloadHandler.text);
                Debug.Log(warehouseAndRackRoot.success);
                foreach (var item in warehouseAndRackRoot.result.Sack)
                {
                    sacks.Add(item.sackTitle);
                }
                sackDropDown.AddOptions(sacks);
                foreach (var item in warehouseAndRackRoot.result.rack)
                {
                    racks.Add(item.rackTitle);
                }
                rackDropDown.AddOptions(racks);

            }
            catch (System.Exception ex)
            {
                //Messagebox.MessageBox(IntPtr.Zero, ex.ToString(), "��ʾ", 0);
                Debug.Log(ex.ToString());
                //alert.setAlertInfo(ex.ToString());
                //if (!baseAlertPanel.activeInHierarchy)
                //{
                //    baseAlertPanel.SetActive(true);
                //}
                //stopLoadAni();
            }
            Debug.Log(webRequest.downloadHandler.text);
        }

    }
}
