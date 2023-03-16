using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class WareHouseInPanelController : BasePanel
{
    /// <summary>
    /// �ⷿ����Աȷ��ˢ��
    /// </summary>
    public GameObject confirmPanel;
    /// <summary>
    /// ��ⰴť
    /// </summary>
    public Button warehouseInBtn;
    /// <summary>
    /// �����
    /// </summary>
    AlertController alert;
    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public GameObject rackSelectPanel;
    private SelectRackPanelController selectRackPanelController;

    /// <summary>
    /// �������������
    /// </summary>
    public Text warehouseInHersmanSum;
    /// <summary>
    /// �����Ʒ������
    /// </summary>
    public Text warehouseInSampleSum;
    /// <summary>
    /// ��ǰ�ͼ쵥λ
    /// </summary>
    public Text currentInpectedDepartment;
    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public Text currentInspectPatchName;
    /// <summary>
    /// �б����
    /// </summary>
    public Transform content;
    public GameObject sackItem;
    private List<SackData> sackDatas;
    /// <summary>
    /// �ⷿ��Ϣ�͵�ǰ������Ʒ����Ϣ
    /// </summary>
    WarehouseAndRackRoot warehouseAndRack;
    SampleInfo sampleInfo;
    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public Button multiSelectRackBtn;
    /// <summary>
    /// ��ⰴť
    /// </summary>
    // Start is called before the first frame update
    private void OnEnable()
    {
        initLoadPanel();
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
        multiSelectRackBtn.onClick.AddListener(() => multiHerdsRackBtnClick());
        warehouseInBtn.onClick.AddListener(() => sampleWarehouseIn());
        returnBtn.onClick.AddListener(() => onReturnBtnlick());
    }


    private void OnDisable()
    {
        sampleInfo = null;
        warehouseInBtn.onClick.RemoveAllListeners();
        sackDatas.Clear();
        if (alert.returnBtn.onClick != null)
        {
            alert.returnBtn.onClick.RemoveAllListeners();
        }


        multiSelectRackBtn.onClick.RemoveAllListeners();
        while (content.childCount != 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        returnBtn.onClick.RemoveAllListeners();
    }
    /// <summary>
    /// �������ĳ�ʼ����Ϣ
    /// </summary>
    public void initInfo(WarehouseAndRackRoot warehouseArg, SampleInfo sampleInfoArg, List<SackData> sacks)
    {
        sackDatas = sacks;
        sampleInfo = sampleInfoArg;
        warehouseAndRack = warehouseArg;
        int samsum = 0;
        foreach (var item in sacks)
        {
            samsum += item.sampleSum;
            GameObject go = GameObject.Instantiate(sackItem);
            go.GetComponent<WarehouseInItemController>().initInfo(item);
            go.transform.SetParent(content);
            go.transform.localScale = new Vector3(1, 1, 1);

        }
        warehouseInHersmanSum.text = sampleInfoArg.result.detectHermanNum.ToString();
        warehouseInSampleSum.text = samsum.ToString();
        currentInpectedDepartment.text = sampleInfoArg.result.sampleSource;
        currentInspectPatchName.text = sampleInfoArg.result.patchTitle;
        selectRackPanelController = rackSelectPanel.GetComponent<SelectRackPanelController>();
        selectRackPanelController.initInfo(warehouseArg);
        Debug.Log("���" + Regex.Unescape(LitJson.JsonMapper.ToJson(sampleInfo)));
        Debug.Log("�ⷿ��Ϣ" + Regex.Unescape(LitJson.JsonMapper.ToJson(warehouseAndRack)));
        Debug.Log("�ڷŽ����" + Regex.Unescape(LitJson.JsonMapper.ToJson(sacks)));
    }
    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public void multiHerdsRackBtnClick()
    {
        /// <summary>
        /// �����洢��ǰ�ж�������ѡȡ
        /// </summary>
        List<WarehouseInItemController> warehouseInItems = new List<WarehouseInItemController>();
        for (int i = 0; i < content.childCount; i++)
        {
            WarehouseInItemController temp = content.GetChild(i).GetComponent<WarehouseInItemController>();
            if (temp.selectToggle.isOn)
            {
                warehouseInItems.Add(temp);
            }
        }
        if (warehouseInItems.Count < 1)
        {
            alert = baseAlertPanel.GetComponent<AlertController>();
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            alert.setAlertInfo("��ѡ������һ�����������������ܣ�");
            baseAlertPanel.SetActive(true);
            return;
        }
        selectRackPanelController.multiHerdsmanIn(warehouseInItems);
        //��ʾѡ��������
        rackSelectPanel.SetActive(true);
    }

    /// <summary>
    /// ����һ���������Ϣ�����仯������Ⱦ�������
    /// </summary>
    public void reloadInfo()
    {
        while (content.childCount != 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }
        ///�������д���
        //////Ĭ��û�з����仯
        bool hasChange = false;
        int originalScale = sackDatas.Count;
        for (int i = 0; i < originalScale; i++)
        {
            var item = sackDatas[i];
            for (int j = 0; j < item.herdsmanItems.Count; j++)
            {
                var item2 = item.herdsmanItems[j];
                if (item2.bagCode != item.sackCode)
                {
                    hasChange = true;
                    // �����˴���
                    //����һ���µĴ�������װ�������
                    //����¹��������Ĵ��ӻ�Ҫ��ԭ�е����ݽ��бȶ�������ظ��ľ�Ҫ���ݺϲ�
                    SackData sackData = new SackData();
                    sackData.sampleSum = item2.actualSampleNum + item2.detectSampleNum + item2.earCodeLackSampleNum;
                    sackData.sackCode = item2.bagCode;
                    sackData.noEarcodeSum = item2.earCodeLackSampleNum;
                    sackData.inspectSamSum = item2.sampleQuantity;
                    sackData.noTagSamSum = item2.actualSampleNum;
                    sackData.lackSamNum = item2.lackSampleNum;
                    sackData.detectSamSum = item2.detectSampleNum;
                    sackData.herdsmanItems=new List<HerdsmanListItem>();
                    sackData.herdsmanItems.Add(item2);
                    //������������޳��������
                    //����ԭ�д��ӵ�����
                    item.sampleSum -= sackData.sampleSum;
                    item.inspectSamSum -= sackData.inspectSamSum;
                    item.detectSamSum -= sackData.detectSamSum;
                    item.lackSamNum -= sackData.lackSamNum;
                    item.noEarcodeSum -= sackData.noEarcodeSum;
                    item.noTagSamSum -= sackData.noTagSamSum;
                    item.herdsmanItems.Remove(item2);
                    //����һ������
                    sackDatas.Add(sackData);
                    

                }
            }
        }
        if (hasChange)
        {
            Debug.Log("���ŷ����仯��"+sackDatas.Count);
            //�Ƴ��Ѿ�װ��Ĵ���
            for (int i = 0; i < sackDatas.Count; i++)
            {
                if (sackDatas[i].sampleSum==0)
                {
                    sackDatas.RemoveAt(i);
                }
            }
            //�Ƚ��кϲ�
            combineSack(sackDatas);
            //չʾ�б�
            int samsum = 0;
            foreach (var item in sackDatas)
            {
                samsum += item.sampleSum;
                GameObject go = GameObject.Instantiate(sackItem);
                go.GetComponent<WarehouseInItemController>().initInfo(item);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);

            }

        }
        else
        {
            int samsum = 0;
            foreach (var item in sackDatas)
            {
                samsum += item.sampleSum;
                GameObject go = GameObject.Instantiate(sackItem);
                go.GetComponent<WarehouseInItemController>().initInfo(item);
                go.transform.SetParent(content);
                go.transform.localScale = new Vector3(1, 1, 1);

            }
        }
        Debug.Log(LitJson.JsonMapper.ToJson(sackDatas));
    }

    /// <summary>
    /// �ϲ���ͬ����
    /// </summary>
    private  void combineSack (List<SackData> sacks)
    {
       
        for (int i = 0; i < sacks.Count; i++)
        {
            
            for (int j = i+1; j < sacks.Count; j++)
            {
                if (sacks[i].sackCode==sacks[j].sackCode)
                {
                  
                  //�ϲ�����
                    sacks[i].sampleSum+=sacks[j].sampleSum;
                    sacks[i].inspectSamSum+=sacks[j].inspectSamSum;
                    sacks[i].detectSamSum+=sacks[j].detectSamSum;
                    sacks[i].lackSamNum+=sacks[j].lackSamNum;
                    sacks[i].noEarcodeSum+=sacks[j].noEarcodeSum;
                    sacks[i].noTagSamSum+=sacks[j].noTagSamSum;
                    sacks[i].rackCode = null;
                    sacks[i].herdsmanItems=sacks[i].herdsmanItems.Concat(sacks[j].herdsmanItems).ToList();
                    sacks.Remove(sacks[j]);
                    Debug.Log("�ϲ�");
                }
            }
        } 
     
    }
    /// <summary>
    /// ��Ʒ���
    /// </summary>
    public void sampleWarehouseIn() 
    {
        Debug.Log("���" + Regex.Unescape(LitJson.JsonMapper.ToJson(sampleInfo)));
        Debug.Log("�ڷŽ����" + Regex.Unescape(LitJson.JsonMapper.ToJson(sackDatas)));
        foreach (var item in sampleInfo.result.herdsmanList)
        {
            foreach (var item2 in sackDatas)
            {
                if (item.bagCode==item2.sackCode)
                {
                    item.rackCode = item2.rackCode;
                }
            }
        }
        ConfirmPanelController confirmPanelController= confirmPanel.GetComponent<ConfirmPanelController>();
        confirmPanelController.initPanel(sampleInfo.result);
        this.gameObject.SetActive(false);
        confirmPanel.gameObject.SetActive(true);
      
    }
}
