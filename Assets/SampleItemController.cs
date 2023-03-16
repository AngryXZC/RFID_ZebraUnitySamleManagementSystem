using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SampleItemController : MonoBehaviour
{

    private ReciveEnum reciveType;

    public Text bagCode;
    public Text herdsmanName;
    public Text sampleNum;
    public Text detectNum;
    public Text lackNum;
    public Text earCodeLackNum;
    public Text actualNum;



    /// <summary>
    /// ��ʶ�Ƿ�����ʶ�������
    /// </summary>
    public bool isReconizing;
    /// <summary>
    /// �������ݰ�
    /// </summary>
    public HerdsmanListItem data;
    /// <summary>
    /// ��������
    /// </summary>
    private VoiceController voiceController;
    /// <summary>
    /// �޶��ŵĹ�ϣ��
    /// </summary>
    private Hashtable hasNoEarCodeTable;
    /// <summary>
    /// ������Ʒ���
    /// </summary>
    private GameObject recivePanel;

    Button noearBtn;

    Button lackBtn;
    /// <summary>
    /// ��ϸ��Ϣ�Ľ���
    /// </summary>
    private GameObject infoPanel;
    ReciveLackSamplePanel infoController;
    private void OnEnable()
    {
        Debug.Log("��ʾ");
        infoPanel = GameObject.Find("LackAnchor").transform.Find("ReciveLackSamplePanel").gameObject;
        infoController=infoPanel.GetComponent<ReciveLackSamplePanel>();
        recivePanel = GameObject.FindGameObjectWithTag("samplePanel");
        hasNoEarCodeTable = new Hashtable();
        voiceController = GameObject.FindGameObjectWithTag("Voice"). GetComponent<VoiceController>();
        noearBtn=earCodeLackNum.GetComponent<Button>();
        lackBtn=lackNum.GetComponent<Button>();
        noearBtn.onClick.AddListener(()=>watchNoearCodeSam());
        lackBtn.onClick.AddListener(()=>watchlackCodeSam());
    }
    private void OnDisable()
    {
        hasNoEarCodeTable.Clear();
        noearBtn.onClick.RemoveAllListeners();
        lackBtn.onClick.RemoveAllListeners();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void setInfo(string bag, HerdsmanListItem param,ReciveEnum reciveEnum)
    {
        
        this.reciveType = reciveEnum;
        RectTransform herdsmanNameWraper = herdsmanName.transform.parent.gameObject.GetComponent<RectTransform>();
        if (reciveType == ReciveEnum.SingeleRcive)
        {
            bagCode.transform.parent.gameObject.SetActive(true);
            herdsmanNameWraper.anchoredPosition = Vector2.right * -486.75f;
            herdsmanNameWraper.sizeDelta = new Vector2(407.66f, 100f);
            herdsmanNameWraper.localPosition = new Vector2(-486.75f, 0);
        }
        else 
        {
            bagCode.transform.parent.gameObject.SetActive(false);
            herdsmanNameWraper.anchoredPosition = Vector2.right * -592.6001f;
            herdsmanNameWraper.sizeDelta = new Vector2(619.36f,100f);
            herdsmanNameWraper.localPosition = new Vector2(-592.6f,0);
        }
        this.data = param;
        this.data.noEarCodeList=new List<string>();
        this.data.bagCode = bag;
        this.bagCode.text = data.bagCode;
        this.herdsmanName.text = data.herdsmanName;
        this.sampleNum.text = data.sampleQuantity.ToString();
        this.detectNum.text = data.detectSampleNum.ToString();
        this.lackNum.text = data.sampleQuantity.ToString();
        this.earCodeLackNum.text = data.earCodeLackSampleNum.ToString();

    }
    /// <summary>
    /// ��⵽һ����Ʒ
    /// </summary>
    public bool detectOneSample(string tag)
    {
        //�ж��Ƿ������ڸ�������ǩ
        if (tag.Substring(4, 7) == data.herdsmanCode) 
        {
            
            //�ж��Ƿ�Ϊ�˶��ı�ǩ
            foreach (var item in data.tagCodeList)
            {
                if (item.tagCode == tag)
                {
                    item.isRecongnized = true;
                    data.detectSampleNum++;
                    data.lackSampleNum = data.sampleQuantity - data.detectSampleNum;
                    SampleReciveController.detectSum++;
                    return true;
                }
                else
                {
                    continue;
                }
            }
            //����ʶ�𵽵ı�ǩ���ڸ��������ǲ�����������ĺ˶���Χ
            if (!hasNoEarCodeTable.Contains(tag)) 
            {
                hasNoEarCodeTable.Add(tag,"2");

                data.earCodeLackSampleNum = hasNoEarCodeTable.Count;
                data.noEarCodeList.Add(tag);
            }
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (data != null)
        {
            


            if (isReconizing&&recivePanel.gameObject.name== "SingleSampleRecivePanel") 
            {
                recivePanel.GetComponent<SingleSampleReciveController>().setcurrentHerdsmanText(data.detectSampleNum.ToString(),data.sampleQuantity.ToString());
            }
            if (data.detectSampleNum != 0)
            {
                if (data.detectSampleNum - data.sampleQuantity != 0)
                {
                    this.detectNum.color = Style.blue;

                }
                else
                {
                    
                    this.detectNum.color = Color.green;
                    herdsmanName.color = Color.green;
                    transform.SetAsFirstSibling();
                    isReconizing = false;
                    if (voiceController != null) 
                    {
                        voiceController.voiceQueue.Enqueue(this.data.herdsmanName + "ʶ����ϣ�");
                    }
                  //���֮������ֻ�Ქ��һ��
                    voiceController = null;
                }
            }
            else
            {
                this.detectNum.color = Color.red;
                transform.SetAsLastSibling();
            }

            this.sampleNum.text = data.sampleQuantity.ToString();
            this.detectNum.text = data.detectSampleNum.ToString();
            
            this.lackNum.text = (data.sampleQuantity - data.detectSampleNum).ToString();
            this.data.lackSampleNum = data.sampleQuantity - data.detectSampleNum;
            this.earCodeLackNum.text=data.earCodeLackSampleNum.ToString();
            if (this.actualNum.text != "")
            {
                data.actualSampleNum = System.Convert.ToInt32(this.actualNum.text);
            }
            else {
                data.actualSampleNum = 0;
            }
            //�����쳣��Ʒ�Ĳ鿴
            if (data.lackSampleNum == 0)
            {
                lackBtn.interactable = false;
                lackNum.color = Color.white;
            }
            else
            {
                lackNum.color = Color.red;
                lackBtn.interactable = true;
            }
            if (data.earCodeLackSampleNum == 0)
            {
                noearBtn.interactable = false;
                earCodeLackNum.color = Color.white;
            }
            else
            {
                earCodeLackNum.color = Color.yellow;
                noearBtn.interactable = true;
            }

        }
    }
    /// <summary>
    /// �жϸ������Ƿ�ʶ�����
    /// </summary>
    /// <returns></returns>
    public bool hasFinished() 
    {
        if (data.detectSampleNum - data.sampleQuantity == 0)
            return true;
        return false;
    }
    /// <summary>
    /// �鿴ȱ�ٶ��ŵ�����
    /// </summary>
    public void watchNoearCodeSam() 
    {
        infoController.initHerdmanData(data,WatchEnum.NoearCodeSample);
        infoPanel.SetActive(true);
        Debug.Log("�鿴�޶�������!"+data.noEarCodeList.Count);
        GangwayDoorDevice device = GangwayDoorDevice.getInstance();
        if (device.IsInventory)
        {
            device.StopInventory();
        }
    }
  /// <summary>
  /// �鿴ȱ����Ʒ����
  /// </summary>
    public void watchlackCodeSam()
    {
        infoController.initHerdmanData(data, WatchEnum.LackSample);
        infoPanel.SetActive(true);
        Debug.Log("�鿴ȱ����Ʒ���飡");
        GangwayDoorDevice device = GangwayDoorDevice.getInstance();
        if (device.IsInventory)
        {
            device.StopInventory();
        }
    }  
}



