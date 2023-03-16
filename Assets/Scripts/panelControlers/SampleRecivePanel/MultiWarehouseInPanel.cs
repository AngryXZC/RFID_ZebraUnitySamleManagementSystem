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
    /// 管理员确认刷卡界面
    /// </summary>
    public GameObject confrimPanel;
    /// <summary>
    /// 选择袋号，货架界面
    /// </summary>
    public GameObject selectRackAndSackPanel;
    //通道们设备
    GangwayDoorDevice doorDevice;
    /// <summary>
    /// 数据绑定
    /// </summary>
    private InstitutionInfo sendResult;
    /// <summary>
    /// 牧民总数
    /// </summary>
    public Text herdsManSum;
    /// <summary>
    /// 样品总数
    /// </summary>
    public Text sampleSum;
    /// <summary>
    /// 送检单位
    /// </summary>
    public Text inspectedDepart;
    /// <summary>
    /// 当前批次
    /// </summary>
    public Text currentWarehouseInPatchName;
    /// <summary>
    /// 列表相关
    /// </summary>
    public Transform context;
    public GameObject listItem;
    /// <summary>
    /// 底部按钮
    /// </summary>
    public Button selectRackAndSackBtn;
    public Button warehouseInBtn;
    /// <summary>
    /// 控制alert的脚本
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
    /// 选择货架袋号按钮
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
            alert.setAlertInfo("请至少选择一个牧户放入袋号和货架！");
            baseAlertPanel.SetActive(true);
        }

        //selectRackAndSackPanel.SetActive(true);
    }
    /// <summary>
    /// 样品入库按钮点击
    /// </summary>
    public void sampleBoundInBtnClick()
    {
        bool canBoundIn = true;
        for (int i = 0; i < context.childCount; i++)
        {
            MutiWarehouseInItemController item = context.GetChild(i).GetComponent<MutiWarehouseInItemController>();
            if (item.rackCode.text=="请选择"||item.sackCode.text=="请选择")
            {
                canBoundIn = false;
            }
        }
        if (canBoundIn)
        {
            Debug.Log("可以刷卡啦！");
            ConfirmPanelController confirmPanelController = confrimPanel.GetComponent<ConfirmPanelController>();
            confirmPanelController.initPanel(sendResult);
            this.gameObject.SetActive(false);
            confrimPanel.SetActive(true);
        }
        else {
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
            alert.setAlertInfo("请将所有牧户放在袋子和货架里！");
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
