using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagInfoController : BasePanel
{
    private SackData data;
    /// <summary>
    /// 列表相关
    /// </summary>
    public Transform content;
    public GameObject bagInfoItem;

    /// <summary>
    /// 
    /// </summary>
    public Text title;
    private void OnEnable()
    {
        returnBtn.onClick.AddListener(() => returnInfoToWareHouseinPanel());
    }
    private void OnDisable()
    {
        returnBtn.onClick.RemoveAllListeners();
        while (content.childCount != 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
    }

    public void setInfo(SackData arg)
    {
        int num = 0;
        data = arg;
        title.text = data.sackCode + "号袋详情:";
        foreach (var item in data.herdsmanItems)
        {
            num++;
            GameObject go = GameObject.Instantiate(bagInfoItem);

            BagInfoItemController bagInfoItemController = go.GetComponent<BagInfoItemController>();
            bagInfoItemController.setInfo(num.ToString(), item, data);
            go.transform.SetParent(content);
            go.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void returnInfoToWareHouseinPanel ()
    { 
        if (voiceController != null)
        {
            voiceController.stopVoiceCoroutine();
        }
        WareHouseInPanelController wareHouseIn = parentPanel.GetComponent<WareHouseInPanelController>();
        if (wareHouseIn != null) 
        {
            wareHouseIn.reloadInfo();
        }
        this.gameObject.SetActive(false);
        parentPanel.SetActive(true);
    }
}
