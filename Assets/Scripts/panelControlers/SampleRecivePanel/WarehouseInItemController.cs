using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseInItemController : MonoBehaviour
{
    /// <summary>
    /// ���ݰ�
    /// </summary>
    
    public SackData data;
    public Toggle selectToggle;
    public Text sackCode;
    public Text sampleSum;
    public Text inspectedSample;
    public Text detectSamSum;
    public Text lackSamSum;
    public Text noEarCodeSamSum;
    public Text noTagSum;
    /// <summary>
    /// ���ܱ��
    /// </summary>
    public Text rackCode;
    public GameObject rackSelectPanel;
    /// <summary>
    /// ��������
    /// </summary>
    public GameObject sackInfoPanel;
    /// <summary>
    /// �������
    /// </summary>
    public GameObject recivePanel;
    public void initInfo(SackData sackData)
    {
        
        data = sackData;
        selectToggle.isOn = false;
        sackCode.text=data.sackCode;
        sampleSum.text = data.sampleSum.ToString();
        inspectedSample.text=data.inspectSamSum.ToString();
        detectSamSum.text=data.detectSamSum.ToString();
        lackSamSum.text=data.lackSamNum.ToString();
        noEarCodeSamSum.text=data.noEarcodeSum.ToString();
        noTagSum.text=data.noTagSamSum.ToString();
        rackCode.text=data.rackCode != null?data.rackCode:"����˴�ѡ�����";
        rackSelectPanel = GameObject.Find("BagCodeAnchor").transform.Find("RackCodePanel").gameObject;
       
        rackCode.GetComponent<Button>().onClick.AddListener(()=>selectRackBtnOnClick());
        sackInfoPanel = GameObject.Find("BagCodeAnchor").transform.Find("BagInfoPanel").gameObject;
        sackCode.GetComponent<Button>().onClick.AddListener(() => watchRackInfoBtnOnClick());
    }
    /// <summary>
    /// �����������
    /// </summary>
    public void selectRackBtnOnClick()
    {
        SelectRackPanelController rackPanelController =rackSelectPanel.GetComponent<SelectRackPanelController>();
        rackPanelController.herdsmanItemIn(this);
        rackSelectPanel.SetActive(true);
    }
    /// <summary>
    /// ����鿴����
    /// </summary>
    public void watchRackInfoBtnOnClick() 
    {
        Debug.Log(LitJson.JsonMapper.ToJson(data));
        BagInfoController bagInfo =sackInfoPanel.GetComponent<BagInfoController>();
        bagInfo.setInfo(data);
        sackInfoPanel.SetActive(true);
    }
    private void OnDestroy()
    {
        rackCode.GetComponent<Button>().onClick.RemoveAllListeners();
        sackCode.GetComponent <Button>().onClick.RemoveAllListeners();
    }
   
}
