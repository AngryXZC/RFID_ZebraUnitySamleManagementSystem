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
    /// 标识是否正在识别的牧户
    /// </summary>
    public bool isReconizing;
    /// <summary>
    /// 牧民数据绑定
    /// </summary>
    public HerdsmanListItem data;
    /// <summary>
    /// 语音控制
    /// </summary>
    private VoiceController voiceController;
    /// <summary>
    /// 无耳号的哈希表
    /// </summary>
    private Hashtable hasNoEarCodeTable;
    /// <summary>
    /// 接收样品面板
    /// </summary>
    private GameObject recivePanel;

    Button noearBtn;

    Button lackBtn;
    /// <summary>
    /// 详细信息的界面
    /// </summary>
    private GameObject infoPanel;
    ReciveLackSamplePanel infoController;
    private void OnEnable()
    {
        Debug.Log("显示");
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
    /// 初始化
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
    /// 检测到一个样品
    /// </summary>
    public bool detectOneSample(string tag)
    {
        //判断是否是属于该牧户的签
        if (tag.Substring(4, 7) == data.herdsmanCode) 
        {
            
            //判断是否为核定的标签
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
            //本次识别到的标签属于该牧户但是不是这个牧户的核定范围
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
                        voiceController.voiceQueue.Enqueue(this.data.herdsmanName + "识别完毕！");
                    }
                  //完毕之后语音只会播放一次
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
            //控制异常样品的查看
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
    /// 判断该牧民是否识别完毕
    /// </summary>
    /// <returns></returns>
    public bool hasFinished() 
    {
        if (data.detectSampleNum - data.sampleQuantity == 0)
            return true;
        return false;
    }
    /// <summary>
    /// 查看缺少耳号的详情
    /// </summary>
    public void watchNoearCodeSam() 
    {
        infoController.initHerdmanData(data,WatchEnum.NoearCodeSample);
        infoPanel.SetActive(true);
        Debug.Log("查看无耳号详情!"+data.noEarCodeList.Count);
        GangwayDoorDevice device = GangwayDoorDevice.getInstance();
        if (device.IsInventory)
        {
            device.StopInventory();
        }
    }
  /// <summary>
  /// 查看缺少样品详情
  /// </summary>
    public void watchlackCodeSam()
    {
        infoController.initHerdmanData(data, WatchEnum.LackSample);
        infoPanel.SetActive(true);
        Debug.Log("查看缺少样品详情！");
        GangwayDoorDevice device = GangwayDoorDevice.getInstance();
        if (device.IsInventory)
        {
            device.StopInventory();
        }
    }  
}



