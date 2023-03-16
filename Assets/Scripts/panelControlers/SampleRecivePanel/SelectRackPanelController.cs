using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRackPanelController : BasePanel
{
    /// <summary>
    /// ȷ����ť
    /// </summary>
    public Button confirmBtn;
    /// <summary>
    /// ��ǰ��������ѡ��
    /// </summary>
    WarehouseInItemController herd;
    /// <summary>
    /// ������
    /// </summary>
    public Dropdown dropdown;
    /// <summary>
    /// �ⷿ��Ϣ
    /// </summary>
    WarehouseAndRackRoot warehouseAndRackRoot;
    List<string> racks = new List<string>();
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        returnBtn.onClick.AddListener(() => onReturnBtnlick());
    }

    private void OnDisable()
    {
     
        confirmBtn.onClick.RemoveAllListeners();
        returnBtn.onClick.RemoveAllListeners();
    }
    /// <summary>
    /// �ڵ������ܵĽ���ͳ�ʼ�����ܵĲ���
    /// </summary>
    public void initInfo(WarehouseAndRackRoot args) 
    {
        racks.Clear();
        dropdown.options.Clear();
        warehouseAndRackRoot = null;
        Debug.Log("Lit"+LitJson.JsonMapper.ToJson(args));
        warehouseAndRackRoot = args;
       
        foreach (var item in warehouseAndRackRoot.result.rack)
        {
            racks.Add(item.rackTitle);
        }
        dropdown.AddOptions(racks);
    }
    /// <summary>
    /// ����������ѡ��
    /// </summary>
    public void herdsmanItemIn(WarehouseInItemController herdsItem) 
    {
        herd = herdsItem;
        confirmBtn.onClick.AddListener(()=>singleHerdManCOnfirmCilick());
    }
    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public void singleHerdManCOnfirmCilick() 
    {
        herd.rackCode.text = dropdown.captionText.text;
        herd.data.rackCode = dropdown.captionText.text;
        gameObject.SetActive(false);
    }
    /// <summary>
    /// ��ѡ����������
    /// </summary>
    /// <param name="warehouseInItems"></param>
    public void multiHerdsmanIn(List<WarehouseInItemController> warehouseInItems) 
    {
        confirmBtn.onClick.AddListener(() => multiHerdsmanConfirmCilick(warehouseInItems));
    }
    public void multiHerdsmanConfirmCilick(List<WarehouseInItemController> warehouseInItems) 
    {
        foreach (var item in warehouseInItems)
        {
            item.data.rackCode=dropdown.captionText.text;
            item.rackCode.text=dropdown.captionText.text;
        }
        gameObject.SetActive(false);
    }
}
