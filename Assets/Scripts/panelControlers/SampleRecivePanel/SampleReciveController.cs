using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SampleReciveController : BasePanel
{
    public static int detectSum = 0;
    ///
    private ReciveEnum reciveType;
    /// <summary>
    /// �������Ϣ
    /// </summary>
    AlertController alert;
    /// <summary>
    /// ȱ����Ʒ����
    /// </summary>
    public Button lackSampleBtn;
    public GameObject lackSamPanel;
    /// <summary>
    /// ʶ����ͼƬ
    /// </summary>
    public Sprite recongnizeSprite;
    public Sprite recongnizeLittleSprite;
    /// <summary>
    /// ȱ����Ʒ��ͼƬ
    /// </summary>
    public Sprite lackSprite;
    public Sprite lackLittleSprite;
    /// <summary>
    /// ������ȷ��Sprite
    /// </summary>
    public Sprite rightSprite;
    public Sprite rightLittleSprite;
    /// <summary>
    /// ��ǰ������Ϣ
    /// </summary>
    private SampleInfo sampleInfo;
    /// <summary>
    /// ��ǰ������
    /// </summary>
    private string currentApplyCode;
    /// <summary>
    ///���ˢ��ȷ�Ͻ���
    /// </summary>
    //public GameObject confirmpanel;
    
    ///<summary>
    ///������
    ///</summary>
    public GameObject warehouseInPanel;
    /// <summary>
    /// ��Ʒ��Դ
    /// </summary>
    public Text sampleSourceText;
    /// <summary>
    /// �������Text
    /// </summary>
    public Text herdsmanText;
    private Hashtable currentHerdsManCodeTable = new Hashtable();
    /// <summary>
    /// ����bg
    /// </summary>
    public Image herdsmanImage;
    public Image herdsmanLittleBG;
    /// <summary>
    /// �����Ʒ���
    /// </summary>
    public Text sampleText;
    /// <summary>
    /// ��ƷͼƬ
    /// </summary>
    public Image sampleImage;
    public Image sampleLittleBG;
    /// <summary>
    /// ȱ����Ʒ������
    /// </summary>
    public Text lackSampleNumText;
    /// <summary>
    /// ȱ����ƷͼƬ��ʾ
    /// </summary>
    public Image lackImage;
    public Image lackImageLittleBG;
    /// <summary>
    /// �洢ʶ�����Ʒ�Ĺ�ϣ��
    /// </summary>
    private Hashtable m_TagTable = new Hashtable();
    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// �б����
    /// </summary>
    public RectTransform sampleList;
    public GameObject sampleItem;
    private List<SampleItemController> sampleItemControllers = new List<SampleItemController>();
    private int num = 0;
   
    /// <summary>
    /// ��ʼ�����ݲ���ʼ�̵�
    /// </summary>
    private void OnEnable()
    {
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        voiceController.startVoiceCoroutine();
        lackSampleBtn.onClick.AddListener(()=>this.lackBtnOnclick());
        changeSprite(lackSprite,lackLittleSprite);
        initLoadPanel();
        ///����alert���
        alert = baseAlertPanel.GetComponent<AlertController>();
        alert.returnBtn.onClick.AddListener(()=>alert.patchAlert(gameObject,parentPanel));
        //
        returnBtn.onClick.AddListener(onReturnBtnlick);
        doorDevice = GangwayDoorDevice.getInstance();
        if (!doorDevice.IsConnect)
        {
            baseAlertPanel.SetActive(true);
            alert.setAlertInfo("�豸δ�����뷵�ز˵�ҳ�����´�Ӧ��!");
            alert.returnBtn.onClick.AddListener(()=>alert.hideSelf());
            //Messagebox.MessageBox(IntPtr.Zero, "�豸δ�����뷵�ز˵�ҳ�����´�Ӧ�ã�", "��ʾ", 0);
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
    private void OnDisable()
    {
        detectSum = 0;
        voiceController.stopVoiceCoroutine();
        lackSampleBtn.onClick.RemoveAllListeners();
        alert.returnBtn.onClick.RemoveAllListeners();
        sampleInfo = null;
        currentApplyCode = string.Empty;
        doorDevice.StopInventory();
        //ȡ��ע���ȡ�¼���������
        doorDevice.ReaderAPI.Events.ReadNotify -= new Events.ReadNotifyHandler(events_ReadNotify);
        m_TagTable.Clear();
        sampleItemControllers.Clear();
        currentHerdsManCodeTable.Clear();
        //����б�����
        while (sampleList.childCount>0)
        {
            DestroyImmediate(sampleList.GetChild(0).gameObject);
        }
        num = 0;
    }
    // Update is called once per frame
    void Update()
    {
       
        if (sampleInfo != null) 
        {
            sampleText.text = "<size=65>" + m_TagTable.Count.ToString() + "</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
            sampleInfo.result.actualSampleSum = m_TagTable.Count;
            int temp= sampleInfo.result.sampleQuantitys - detectSum;
            if (temp < 0)
            {
                lackSampleNumText.text = 0.ToString();
            }
            else 
            {
                lackSampleNumText.text=temp.ToString();
            }
            herdsmanText.text= "<size=65>"+currentHerdsManCodeTable.Count.ToString()+"</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
            //��Ӧ����Ʒ
            if (m_TagTable.Count > 0)
            {
                changeSprite(recongnizeSprite, recongnizeLittleSprite);
                sampleText.color = Style.blue;
                lackSampleNumText.color = Style.blue;
                herdsmanText.color = Style.blue;
                
                lackSampleBtn.enabled = true;
            }
            else 
            {
                sampleText.color = Color.red;
                lackSampleNumText.color = Color.red;
                herdsmanText.color = Color.red;
               
            }
            //����������ȷ
            if (sampleInfo.result.herdsmanQuantity==currentHerdsManCodeTable.Count)
            {
                herdsmanImage.sprite = rightSprite;
                herdsmanLittleBG.sprite = rightLittleSprite;
                herdsmanText.color = Color.green;
            }
            //��Ʒ������ȷ
            if (sampleInfo.result.sampleQuantitys<=m_TagTable.Count)
            {
                changeSprite(rightSprite,rightLittleSprite);
                sampleImage.sprite = rightSprite;
                lackImage.sprite= rightSprite;
                lackSampleNumText.color = Color.green;
                sampleText.color = Color.green;
                lackSampleBtn.enabled = false;
            }
        }
        
    }

    //������Ʒ�ύ��ť
    public void reciveSampleBtn()
    {
        
        
        sampleInfo.result.applyCode= currentApplyCode;
        sampleInfo.result.detectHermanNum = currentHerdsManCodeTable.Count; 
        sampleInfo.result.actualSampleSum= m_TagTable.Count;
        //�������
        // ConfirmPanelController confirmPanelController = confirmpanel.GetComponent<ConfirmPanelController>();
        // confirmPanelController.initPanel(sampleInfo.result);
        MultiWarehouseInPanel multiWarehouseInPanel = warehouseInPanel.GetComponent<MultiWarehouseInPanel>();
        multiWarehouseInPanel.initPanel(sampleInfo.result);
        //�˴�˳������������
        //this.gameObject.SetActive(false);
        doorDevice.StopInventory();
        warehouseInPanel.SetActive(true);
        
        //confirmpanel.gameObject.SetActive(true);
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
            //����ȷ�ĵ���Ʒ����ҵ�ǰ�б��в������������
            foreach (var item in sampleItemControllers)
            {
                //�����Ʒ��ǩ
                if (item.detectOneSample(tagID.Substring(1, 15))) 
                {
                    m_TagTable.Add(tagID, "1");
                    string herdsmanCodeTemp = tagID.Substring(5,7);
                    if (item.data.herdsmanCode== herdsmanCodeTemp)
                    {
                        
                        if (!currentHerdsManCodeTable.Contains(herdsmanCodeTemp) )
                        {
                            currentHerdsManCodeTable.Add(herdsmanCodeTemp, "2");
                        }
                     
                    }
                }
               
            }

        }
    }

    public void setcurrentData(string apply_code,string patchTitle, SampleInfo info,ReciveEnum reciveEnum)
    {
        reciveType = reciveEnum;
        Debug.Log(gameObject.name+reciveType);
        sampleInfo= info;
        sampleInfo.result.patchTitle=patchTitle;
        currentApplyCode = apply_code;
        //��ȡ��������
        
        if (sampleInfo.success)
        {
            
            sampleSourceText.text = "��Ʒ��Դ��" + sampleInfo.result.sampleSource;
            Debug.Log(sampleInfo.result.sampleSource);
            herdsmanText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.herdsmanQuantity + "</size>";
            sampleText.text = "<size=80>0</size>" + "/<size=65>" + sampleInfo.result.sampleQuantitys + "</size>";
            lackSampleNumText.text = sampleInfo.result.sampleQuantitys.ToString();
           
            foreach (var item in sampleInfo.result.herdsmanList)
            {
                ++num;
                GameObject go = Instantiate(sampleItem);
                SampleItemController sampleItemController = go.GetComponent<SampleItemController>();
                sampleItemController.setInfo(num.ToString(), item,reciveType);
                sampleItemController.isReconizing = true;
                go.transform.SetParent(sampleList);
                go.transform.localScale = new Vector3(1, 1, 1);
                sampleItemControllers.Add(sampleItemController);
            }
        }

    }
   

    /// <summary>
    /// �ı�����м�������ͼ
    /// </summary>
    /// <param name="currentSprite"></param>
    public void changeSprite(Sprite currentSprite,Sprite currentLittleSprite)
    {
        herdsmanImage.sprite = currentSprite;
        herdsmanLittleBG.sprite = currentLittleSprite;
        sampleImage.sprite = currentSprite;
        sampleLittleBG.sprite = currentLittleSprite;
        lackImage.sprite = currentSprite;
        lackImageLittleBG.sprite = currentLittleSprite;
    }
    /// <summary>
    /// ȱ����Ʒ������
    /// </summary>
    public void lackBtnOnclick() 
    {
        Debug.Log("����鿴ȱ��������");
        
        ReciveLackSamplePanel reciveLackSamplePanel=lackSamPanel.GetComponent<ReciveLackSamplePanel>();
        reciveLackSamplePanel.initData(sampleInfo.result);
        doorDevice.StopInventory();
        lackSamPanel.SetActive(true);
    } 
}
