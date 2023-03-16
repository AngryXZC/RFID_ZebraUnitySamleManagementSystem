using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanItem : MonoBehaviour
{
    public Text num;
    public Text herdsmanName;
    public Text numOfReciveSample;
    public Text cleanSampleNum;
    public Text detectSampleNum;
    public Text abnormalSampleNum;
    public Text cleanMan;
    public Text operate;
    public string earCode;
    
    public CleanHerdsmanListItem myData;
    /// <summary>
    /// ϸ��չʾ����
    /// </summary>
    public GameObject lackSampleInfoPanel;
    public void initItem(int number, CleanHerdsmanListItem result,string operateInfo) 
    {
      
        myData = result;
        num.text = number.ToString();
        herdsmanName.text = myData.herdsmanName;
        numOfReciveSample.text = myData.reciveSampleNum.ToString();
        cleanSampleNum.text =myData.cleanSampleNum.ToString();
        detectSampleNum.text = myData.detectSampleNum.ToString();
        abnormalSampleNum.text = myData.abnormalBackSampleNum.ToString();
        
        cleanMan.text = result.cleanMan==null?"����" : result.cleanMan;
        operate.text = operateInfo;
        
    }

    private void LateUpdate()
    {
        
        this.detectSampleNum.text = myData.detectSampleNum.ToString();
        int detectSamNum = System.Convert.ToInt32(detectSampleNum.text);
        int cleanSamNum=System.Convert.ToInt32(cleanSampleNum.text);

        if (detectSamNum - cleanSamNum != 0)
        {
           
            operate.text = "<color=#ff0000ff><i>ȱ����Ʒ����</i></color>";
            if (operate.gameObject.GetComponent<Button>() == null) 
            {
                Button cleanItemInfoBtn = operate.gameObject.AddComponent<Button>();
                cleanItemInfoBtn.onClick.AddListener(() => onInfoClick());
            }
            detectSampleNum.color = Color.red;
        }
        else 
        {
            operate.text = "�޺���������";
            Button button = operate.gameObject.GetComponent<Button>();
            if (button!= null) 
            {
                Destroy(button);
            }
          detectSampleNum.color = Color.green;
        }
    }
   /// <summary>
   /// �������
   /// </summary>
    public void onInfoClick()
    {
        GameObject go= Instantiate(lackSampleInfoPanel);
        LackSampleController lackSampleController= go.GetComponent<LackSampleController>();
        if (lackSampleController != null) 
        {
            //��������
            lackSampleController.showItem(myData);
        }
    }
    /// <summary>
    /// ʶ��һ����ǩ
    /// </summary>
    public bool getATag(string tagCode) 
    {
        foreach (var item in this.myData.cleanSampleList) 
        {
            if (item.sampleCode==tagCode)
            {
                item.isRecongnized = true;
                myData.detectSampleNum++;
                return true;
            }
        }
        return false;
    }
}
