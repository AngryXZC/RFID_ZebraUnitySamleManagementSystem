using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class SingleSampleReciveController : BasePanel
{
    Coroutine mCoroutine;
    /// <summary>
    /// ��������
    /// </summary>
    ReciveEnum reciveType;

    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public GameObject bagCodePanel;
    /// <summary>
    /// �Ƿ�����ʶ��
    /// </summary>
    bool isRecongenizing = false;
    /// <summary>
    /// ��ǰʶ������
    /// </summary>
    public Text currentHerdsManText;
    /// <summary>
    /// ��ǰ�������Ʒ���
    /// </summary>
    public Text currentHerdsManSampleText;
    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    /// <summary>
    /// �洢ʶ�����Ʒ�Ĺ�ϣ��
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    [System.NonSerialized]
    public GangwayDoorDevice doorDevice;

    /// <summary>
    /// ��ǰ������Ϣ
    /// </summary>
    private SampleInfo sampleInfo;
    /// <summary>
    /// ��ǰ������
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    /// ��Ʒ��Դ
    /// </summary>
    public Text sampleSourceText;
    /// <summary>
    /// �������Text
    /// </summary>
    public Text herdsmanNumText;

    /// <summary>
    /// �����Ʒ���
    /// </summary>
    public Text sampleNumText;
    /// <summary>
    /// �б����
    /// </summary>
    private HerdsmanListItem currentHerdsman;
    /// <summary>
    /// ��ǰ�ⷿ����Ϣ
    /// </summary>
    WarehouseAndRackRoot warehouseAndRack;
    /// <summary>
    /// ��ǰ����
    /// </summary>
    string bagCode;
    private List<SampleItemController> sampleItemControllers = new List<SampleItemController>();
    /// <summary>
    /// �б����
    /// </summary>
    public RectTransform sampleList;
    public GameObject sampleItem;
    /// <summary>
    /// ������
    /// </summary>
    public GameObject warehouseInPanel;
    /// <summary>
    /// �ײ���ť
    /// </summary>
    public Button reciveBtn;
    public Button stopCurrenHerdsMantBtn;
    public Button changeBagBtn;
    private void OnEnable()
    {
        //ע�ᰴť�������
        reciveBtn.onClick.AddListener(()=>reciveBtnClick());
        stopCurrenHerdsMantBtn.onClick.AddListener(()=>stopCurrentHerdsmanBtnClick());
        changeBagBtn.onClick.AddListener(() => changeBagNumBtnClick());


        mCoroutine=StartCoroutine(scanCurrentHersman());
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        voiceController.startVoiceCoroutine();
        initLoadPanel();
        ///����alert���
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(() => alert.patchAlert(gameObject, parentPanel));
        //
        returnBtn.onClick.AddListener(onReturnBtnlick);
        doorDevice = GangwayDoorDevice.getInstance();
        if (!doorDevice.IsConnect)
        {
            baseAlertPanel.SetActive(true);
            alert.setAlertInfo("�豸δ�����뷵�ز˵�ҳ�����´�Ӧ��!");
            alert.returnBtn.onClick.AddListener(() => alert.hideSelf());
        }
        else
        {

            if (!doorDevice.IsInventory)
            {
                doorDevice.StartInventory();
            }

            //ע���ȡ�¼���������
            doorDevice.ReaderAPI.Events.ReadNotify += new Events.ReadNotifyHandler(events_ReadNotify);
        }
    }
    private void Update()
    {
        if (currentHerdsman != null)
        {
            currentHerdsManText.text = currentHerdsman.herdsmanName;
        }
        else 
        {
            currentHerdsManText.text = "δ��⵽����";
            currentHerdsManSampleText.text = "<size=80>0</size>" + "/<size=65>0</size>"; ;
        }
        herdsmanNumText.text = "<size=80>"+sampleItemControllers.Count+"</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
        sampleNumText.text = "<size=80>"+ m_TagTable.Count + "</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
    }
    private void OnDisable()
    {
        //�Ƴ���ť����¼�
        reciveBtn.onClick.RemoveAllListeners();
        stopCurrenHerdsMantBtn.onClick.RemoveAllListeners();
        changeBagBtn.onClick.RemoveAllListeners();

        currentHerdsManText.text = "δ��⵽����";
        isRecongenizing =false;
        currentHerdsman = null;
        sampleItemControllers.Clear();
        if (mCoroutine!=null)
        {
            StopCoroutine(mCoroutine);
        }
        currentApplyCode = string.Empty;
        voiceController.stopVoiceCoroutine();
        alert.returnBtn.onClick.RemoveAllListeners();
        doorDevice.StopInventory();
        //ȡ��ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        m_TagTable.Clear();
        //����б�����
        while (sampleList.childCount > 0)
        {
            DestroyImmediate(sampleList.GetChild(0).gameObject);
        }
    }
    internal void events_ReadNotify(object sender, Events.ReadEventArgs e)
    {

        string tagID = e.ReadEventData.TagData.TagID;

        bool isSampleNumber = DataCheck.isSampleNumber(tagID);
        if (m_TagTable.Contains(tagID) || !isSampleNumber)
        {
            return;
        }
        else
        {

            string herdsmanCodeTemp = tagID.Substring(5, 7);
            //��������
            if (sampleInfo != null)
            {
                //�Աȱ��������е�������Ϣ
                foreach (HerdsmanListItem item in sampleInfo.result.herdsmanList)
                {
                    //�����ǰ��ǩ������������������С����ҵ�ǰû����ʶ���κ�����
                    if (herdsmanCodeTemp == item.herdsmanCode && currentHerdsman == null)
                    {
                        Debug.LogWarning("����������" + item.herdsmanName);
                        Debug.LogWarning("������ǰʶ��������" + item.herdsmanName);
                        //�����������Ա�ʶ�����ֱ���Թ������ǩ
                        foreach (SampleItemController item2 in sampleItemControllers) 
                        {
                            if (item2.data.herdsmanCode == herdsmanCodeTemp) 
                            {
                                return;
                            }
                        }
                        currentHerdsman = item;
                    }
                }
                //ʶ����Ʒ���ж������ǩ�Ƿ�����ʶ����б����ҵ�
                foreach (var item in sampleItemControllers)
                {
                    //�����Ʒ��ǩ
                    if (item.detectOneSample(tagID.Substring(1, 15)))
                    {
                        m_TagTable.Add(tagID, "1");
                    }
                }
                //�жϵ�ǰ�����Ƿ�ʶ�����
                foreach (var item in sampleItemControllers)
                {

                    if (isRecongenizing && item.data == currentHerdsman && item.hasFinished())
                    {
                        Debug.LogWarning(item.data.herdsmanName + "ʶ����ϣ�");
                        currentHerdsman = null;
                        isRecongenizing = false;
                    }
                }
            }


        }
    }

    public void setcurrentData(string apply_code, SampleInfo info,ReciveEnum reciveEnum)
    {
        reciveType = reciveEnum;
        Debug.Log(gameObject.name+reciveType);
        sampleInfo = info;
        currentApplyCode = apply_code;
        //��ȡ��������
        if (sampleInfo.success)
        {
            sampleSourceText.text = "��Ʒ��Դ��" + sampleInfo.result.sampleSource;
            herdsmanNumText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
            sampleNumText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
        }

    }

    /// <summary>
    /// ���õ�ǰװ��Ĵ���
    /// </summary>
    public void setCurreneBag(string arg)
    {
         bagCode = arg;
    }
    /// <summary>
    /// ��ʼ����ǰ�ⷿ��Ϣ
    /// </summary>
    /// <param name="arg"></param>
    public void setWarehouseInfo(WarehouseAndRackRoot arg)
    {
        warehouseAndRack = arg;
    }
    IEnumerator scanCurrentHersman()
    {
        while (true)
        {
            if (currentHerdsman != null && !isRecongenizing)
            {
                GameObject go = Instantiate(sampleItem);
                SampleItemController sampleItemController = go.GetComponent<SampleItemController>();
                sampleItemController.setInfo(bagCode, currentHerdsman,reciveType);
                go.transform.SetParent(sampleList);
                go.transform.localScale = new Vector3(1, 1, 1);
                sampleItemController.isReconizing = true;
                //go.transform.SetAsFirstSibling();
                sampleItemControllers.Add(sampleItemController);
                isRecongenizing = true;
            }

            yield return new WaitForSeconds(1f);
        }
    }
    /// <summary>
    /// ���ĵ�ǰ������Ϣ
    /// </summary>
    /// <param name="currentSampleNum"></param>
    /// <param name="currentSampleSum"></param>
    public void setcurrentHerdsmanText(string currentSampleNum,string currentSampleSum)
    {
        
        currentHerdsManSampleText.text = "<size=80>" + currentSampleNum + "</size>" + "/<size=65>" + currentSampleSum + "</size>";
    }
    //���ְ�ť����¼��Ĵ�������

    /// <summary>
    /// ��ⰴť���
    /// </summary>
    public void reciveBtnClick() 
    {
        List<SackData> sacks = new List<SackData>();
        WareHouseInPanelController wareHouseInPanelController=warehouseInPanel.GetComponent<WareHouseInPanelController>();
        foreach (var item in sampleInfo.result.herdsmanList)
        {
            //������ǰ����÷ŵĴ���
            SackData sackData = new SackData();
            sackData.herdsmanItems=new List<HerdsmanListItem>();
            //��ǰ�����ʵ����������Ʒ����=�ޱ�ǩ��+�������+ȱ�ٶ�����Ʒ����
            int samsum = item.actualSampleNum + item.detectSampleNum+item.earCodeLackSampleNum;
            sampleInfo.result.actualSampleSum+=samsum;
            //�жϸ�������û����Ʒ��1.��⵽������Ʒ��2.����ϵͳ�е����б�ǩ����Ʒ��3.�ޱ�ǩ����Ʒ��
            if (item.detectSampleNum > 0||item.earCodeLackSampleNum>0||item.actualSampleNum>0) 
            {
                sampleInfo.result.detectHermanNum++;
                sackData.sackCode = item.bagCode;
                sackData.sampleSum = samsum;
                sackData.inspectSamSum = item.sampleQuantity;
                sackData.detectSamSum = item.detectSampleNum;
                sackData.lackSamNum = item.lackSampleNum;
                sackData.noEarcodeSum = item.earCodeLackSampleNum;
                sackData.noTagSamSum = item.actualSampleNum;
                sackData.lackSamNum = item.lackSampleNum;
                sackData.herdsmanItems.Add(item);
                //�жϵ�ǰ���������û�зŶ���
                bool newSack = true;
                foreach (var sackItem in sacks)
                {
                    if (sackItem.sackCode == sackData.sackCode)
                    {
                        newSack = false;
                        Debug.Log("�Ƿ����´���"+newSack);
                        sackItem.sampleSum += sackData.sampleSum;
                        sackItem.inspectSamSum += sackData.inspectSamSum;
                        sackItem.detectSamSum += sackData.detectSamSum;
                        sackItem.lackSamNum += sackData.lackSamNum;
                        sackItem.noEarcodeSum += sackData.noEarcodeSum;
                        sackItem.noTagSamSum += sackData.noTagSamSum;
                        sackItem.herdsmanItems.Add(item);
                    }
                }
                if (newSack) 
                {
                    sacks.Add(sackData);
                }
            }
           
            
        }
        wareHouseInPanelController.initInfo(warehouseAndRack,sampleInfo, sacks);
        gameObject.SetActive(false);
        warehouseInPanel.SetActive(true);
    }
    /// <summary>
    /// ֹͣ��ǰ����ʶ��
    /// </summary>
    public void stopCurrentHerdsmanBtnClick() 
    {
        Debug.Log("ֹͣʶ��ǰ����");
        currentHerdsman=null;
        isRecongenizing=false;
    }
    /// <summary>
    /// ��������
    /// </summary>
    public void changeBagNumBtnClick()
    {
        doorDevice.StopInventory();
        bagCodePanel.SetActive(true);
        SelectRackAndBagPanelController selectRack=bagCodePanel.GetComponent<SelectRackAndBagPanelController>();    
        selectRack.confirmBtn.onClick.RemoveAllListeners();
        selectRack.confirmBtn.onClick.AddListener(()=>selectRack.changeBagByClickConfirm());
        selectRack.parentPanel = this.gameObject; 
    }
}
