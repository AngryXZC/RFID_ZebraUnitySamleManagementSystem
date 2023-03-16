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
    /// 接收方式
    /// </summary>
    private ReciveEnum reciveType;

    /// <summary>
    /// 警告框信息
    /// </summary>
    AlertController alert;
    /// <summary>
    /// 提示信息
    /// </summary>
    public Text infoText;
    /// <summary>
    /// 垂直滚动列表
    /// </summary>
    public RectTransform verticalLayout;
    /// <summary>
    /// 列表的每一项
    /// </summary>
    public GameObject patchItem;
    /// <summary>
    /// 记录总共有多少列表项
    /// </summary>
    int index = 0;
  
    /// <summary>
    /// 用于接收数据的实体
    /// </summary>
    PatchRoot patchesInfo;
    /// <summary>
    /// 设备引用
    /// </summary>
    GangwayDoorDevice doorDevice=GangwayDoorDevice.getInstance();
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        //初始化警告框
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
    /// 区分是单户接收还是批量接收
    /// </summary>
    public void setReciveType(ReciveEnum reciveEnum)
    {
        reciveType = reciveEnum;
       
    }
  
    //使用Unity发送类发送
    IEnumerator UntiyGet(string url, string getDataStr)
    {
        Debug.Log(url);
        playLoadAni();
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            alert.setAlertInfo("网络请求错误！");
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            baseAlertPanel.SetActive(true);
            //Messagebox.MessageBox(IntPtr.Zero, webRequest.error, "提示框", 0);
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
                    //设置为列表下的子项
                    ga.transform.SetParent(verticalLayout);

                    //为单项设置参数
                    PatchItem element=ga.GetComponent<PatchItem>();
                    element.patchResultItem = item;
                    element.reciveEnum = reciveType;
                    //设置文本
                    ga.transform.Find("PatchInfo").Find("info").GetComponent<Text>().text = item.title;
                    ga.transform.Find("PatchInfo").Find("num").GetComponent<Text>().text = index.ToString();
                }

            }
            else
            {
                Debug.Log("接口返回值异常！");
            }
            Debug.Log(webRequest.downloadHandler.text);      
        }
          
        

    }
}
