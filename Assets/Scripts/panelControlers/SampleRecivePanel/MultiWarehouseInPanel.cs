using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Text.RegularExpressions;

public class MultiWarehouseInPanel : BasePanel
{
    /// <summary>
    /// ����Աȷ��ˢ������
    /// </summary>
    public GameObject confrimPanel;
    /// <summary>
    /// ѡ����ţ����ܽ���
    /// </summary>
    public GameObject selectRackAndSackPanel;
    //ͨ�����豸
    GangwayDoorDevice doorDevice;
    /// <summary>
    /// ���ݰ�
    /// </summary>
    private InstitutionInfo sendResult;
    /// <summary>
    /// ��������
    /// </summary>
    public Text herdsManSum;
    /// <summary>
    /// ��Ʒ����
    /// </summary>
    public Text sampleSum;
    /// <summary>
    /// �ͼ쵥λ
    /// </summary>
    public Text inspectedDepart;
    /// <summary>
    /// ��ǰ����
    /// </summary>
    public Text currentWarehouseInPatchName;
    /// <summary>
    /// �б����
    /// </summary>
    public Transform context;
    public GameObject listItem;
    /// <summary>
    /// �ײ���ť
    /// </summary>
    public Button selectRackAndSackBtn;
    public Button warehouseInBtn;
    /// <summary>
    /// ����alert�Ľű�
    /// </summary>
    private AlertController alert;
    private void OnEnable()
    {
        initLoadPanel();
        doorDevice = GangwayDoorDevice.getInstance();
        alert = baseAlertPanel.GetComponent<AlertController>();

        this.returnBtn.onClick.AddListener(() => backBtnClick());
        this.selectRackAndSackBtn.onClick.AddListener(() => selectRackSackBtnClick());
        this.warehouseInBtn.onClick.AddListener(() => sampleBoundInBtnClick());
    }

    private void OnDisable()
    {
        while (context.childCount > 0)
        {
            DestroyImmediate(context.GetChild(0).gameObject);
        }
        if (this.returnBtn != null)
        {
            returnBtn.onClick.RemoveListener(() => backBtnClick());
        }
        this.selectRackAndSackBtn.onClick.RemoveListener(() => selectRackSackBtnClick());
        this.warehouseInBtn.onClick.RemoveListener(() => sampleBoundInBtnClick());

    }
    private void backBtnClick()
    {

        if (!doorDevice.IsInventory)
        {
            doorDevice.StartInventory();
        }
        this.gameObject.SetActive(false);
    }
    public void initPanel(InstitutionInfo res)
    {
        sendResult = res;
        foreach (HerdsmanListItem key in sendResult.herdsmanList)
        {

            sendResult.actualSampleSum = sendResult.actualSampleSum + key.actualSampleNum;

        }
        herdsManSum.text = res.detectHermanNum.ToString();
        sampleSum.text = res.actualSampleSum.ToString();
        inspectedDepart.text = res.sampleSource.ToString();
        currentWarehouseInPatchName.text = res.patchTitle.ToString();
        foreach (HerdsmanListItem key in sendResult.herdsmanList)
        {
            if (key.detectSampleNum > 0 || key.actualSampleNum > 0 || key.earCodeLackSampleNum > 0)
            {
                GameObject go = Instantiate(listItem);
                MutiWarehouseInItemController mutiWarehouseInItem = go.GetComponent<MutiWarehouseInItemController>();
                mutiWarehouseInItem.initData(key);
                go.transform.SetParent(context);
                go.transform.localScale = new Vector3(1, 1, 0);
            }

        }
        Debug.Log(Regex.Unescape(JsonMapper.ToJson(sendResult)));
    }
    /// <summary>
    /// ѡ����ܴ��Ű�ť
    /// </summary>
    public void selectRackSackBtnClick()
    {
        bool canSelcet = false;
        for (int i = 0; i < context.childCount; i++)
        {
            MutiWarehouseInItemController item = context.GetChild(i).GetComponent<MutiWarehouseInItemController>();
            if (item.selectRackAndSack.isOn)
            {
                canSelcet = true;
            }
        }
        if (canSelcet)
        {
            selectRackAndSackPanel.SetActive(true);
        }
        else
        {
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            alert.setAlertInfo("������ѡ��һ������������źͻ��ܣ�");
            baseAlertPanel.SetActive(true);
        }

        //selectRackAndSackPanel.SetActive(true);
    }
    /// <summary>
    /// ��Ʒ��ⰴť���
    /// </summary>
    public void sampleBoundInBtnClick()
    {
        bool canBoundIn = true;
        for (int i = 0; i < context.childCount; i++)
        {
            MutiWarehouseInItemController item = context.GetChild(i).GetComponent<MutiWarehouseInItemController>();
            if (item.rackCode.text=="��ѡ��"||item.sackCode.text=="��ѡ��")
            {
                canBoundIn = false;
            }
        }
        if (canBoundIn)
        {
            Debug.Log("����ˢ������");
            ConfirmPanelController confirmPanelController = confrimPanel.GetComponent<ConfirmPanelController>();
            confirmPanelController.initPanel(sendResult);
            this.gameObject.SetActive(false);
            confrimPanel.SetActive(true);
        }
        else {
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            alert.setAlertInfo("�뽫�����������ڴ��Ӻͻ����");
            baseAlertPanel.SetActive(true);
        }
    }

    public void setRacklAndSack(string sackCode, string rackCode)
    {
        for (int i = 0; i < context.childCount; i++)
        {

            MutiWarehouseInItemController item = context.GetChild(i).GetComponent<MutiWarehouseInItemController>();
            if (item.selectRackAndSack.isOn)
            {
                item.data.rackCode = rackCode;
                item.data.bagCode = sackCode;
                item.sackCode.text = sackCode;
                item.rackCode.text = rackCode;
                item.selectRackAndSack.isOn = false;
            }
        }
    }
}
