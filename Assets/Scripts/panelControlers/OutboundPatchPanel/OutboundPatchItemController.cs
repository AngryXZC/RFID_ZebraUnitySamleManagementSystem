using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutboundPatchItemController : MonoBehaviour
{
    /// <summary>
    /// 出库单批次面板
    /// </summary>
    private GameObject outboundPanel;
    /// <summary>
    /// 出库批次选择面板
    /// </summary>
    private GameObject outboundPatchPanel;
    public Text num;
    public Text description;
    public Button button;
    private OutboundPatchInfo data;
    private void OnEnable()
    {
        outboundPatchPanel = GameObject.FindGameObjectWithTag("OutboundPanel");
        outboundPanel = GameObject.FindGameObjectWithTag("outboundPatchInfo").transform.Find("OutboundPatchInfoPanel").gameObject;
        data = null;
        if (button != null)
        {
            button.onClick.AddListener(btnClick);
        }
    }
    private void OnDisable()
    {
        data = null;
        button.onClick.RemoveAllListeners();
    }
    private void btnClick() 
    {
        
        OutboundPatchInfoController infoController= outboundPanel.GetComponent<OutboundPatchInfoController>();
        infoController.init(data.applyCode);
        outboundPatchPanel.SetActive(false);
        outboundPanel.SetActive(true);
    }
    public void initInfo(int number,OutboundPatchInfo info)
    {
        data = info;
        num.text=number.ToString();
        description.text = data.title;
    }
}
