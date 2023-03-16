using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class PatchPanelSeelectController : BasePanel
{
    /// <summary>
    /// ���շ�ʽ
    /// </summary>
    private ReciveEnum reciveType;

    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    /// <summary>
    /// ��ʾ��Ϣ
    /// </summary>
    public Text infoText;
    /// <summary>
    /// ��ֱ�����б�
    /// </summary>
    public RectTransform verticalLayout;
    /// <summary>
    /// �б��ÿһ��
    /// </summary>
    public GameObject patchItem;
    /// <summary>
    /// ��¼�ܹ��ж����б���
    /// </summary>
    int index = 0;
  
    /// <summary>
    /// ���ڽ������ݵ�ʵ��
    /// </summary>
    PatchRoot patchesInfo;
    /// <summary>
    /// �豸����
    /// </summary>
    GangwayDoorDevice doorDevice=GangwayDoorDevice.getInstance();
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        //��ʼ�������
        alert=baseAlertPanel.GetComponent<AlertController>();
        returnBtn.onClick.AddListener(onReturnBtnlick);
        if (doorDevice.IsConnect && doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }    
        //res= NetTools.HttpGet(IPAddressConfig.patchPanelAddress, "");
         StartCoroutine(UntiyGet(IPAddressConfig.patchPanelAddress,""));    
    }
  
    private void OnDisable()
    {
        alert.returnBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveAllListeners();
        stopLoadAni();
        
        for (int i = 0; i < verticalLayout.childCount; i++)
        {
            Destroy(verticalLayout.GetChild(i).gameObject);
        }
        index = 0;
        patchesInfo=null;
        StopCoroutine("UntiyGet");
    }
    /// <summary>
    /// �����ǵ������ջ�����������
    /// </summary>
    public void setReciveType(ReciveEnum reciveEnum)
    {
        reciveType = reciveEnum;
       
    }
  
    //ʹ��Unity�����෢��
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        Debug.Log(url);
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("�����������");
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "��ʾ��", 0);
            stopLoadAni();
        }
        else
        {
            patchesInfo = JsonMapper.ToObject<PatchRoot>(webRequest.downloadHandler.text);
            if (patchesInfo.success)
            {
                stopLoadAni();
                foreach (var item in patchesInfo.result)
                {
                    index++;
                    GameObject ga = Instantiate(patchItem);
                    ga.SetActive(true);
                    //����Ϊ�б��µ�����
                    ga.transform.SetParent(verticalLayout);

                    //Ϊ�������ò���
                    PatchItem element=ga.GetComponent<PatchItem>();
                    element.patchResultItem = item;
                    element.reciveEnum = reciveType;
                    //�����ı�
                    ga.transform.Find("PatchInfo").Find("info").GetComponent<Text>().text = item.title;
                    ga.transform.Find("PatchInfo").Find("num").GetComponent<Text>().text = index.ToString();
                }

            }
            else
            {
                Debug.Log("�ӿڷ���ֵ�쳣��");
            }
            Debug.Log(webRequest.downloadHandler.text);      
        }
          
        

    }
}
