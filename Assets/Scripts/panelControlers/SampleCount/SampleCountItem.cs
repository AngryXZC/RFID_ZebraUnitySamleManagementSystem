using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleCountItem : MonoBehaviour
{
    public Text num;
    public Text herdsmanName;
    public Text recNum;
    public Text decNum;
    public Text withoutEarSamNum;
    public Text tobeCleanNum;
    public Text tobeInspectNum;
    public Text tobeBackNum;
    public Text abnormalBackNum;
    public CountSampleResult data;

    // Update is called once per frame
    void LateUpdate()
    {
      
        if (data != null) 
        {
            decNum.text = data.detectSampleNum.ToString();
            if (data.detectSampleNum - data.reciveSampleNum != 0)
            {
                decNum.color = Color.red;
            }
            else
            {
                decNum.color = Color.green;
            }
        }
        
    }
    public void init(string number, CountSampleResult res)
    {
        this.data = res;
        num.text = number;
        herdsmanName.text = data.herdsmanName;
        recNum.text = data.reciveSampleNum.ToString();
        decNum.text = data.detectSampleNum.ToString();
        withoutEarSamNum.text = data.lackEarSampleNum.ToString();
        tobeCleanNum.text = data.tobeCleanSampleNum.ToString();
        tobeInspectNum.text = data.tobeInspectSampleNum.ToString();
        tobeBackNum.text = data.tobeBackSampleNum.ToString();
        abnormalBackNum.text= data.abnormalSampleNum.ToString();
    }

    public void getAtag(string code) 
    {
        foreach (var item in data.reciveSampleList)
        {
            if (item.sampleCode==code)
            {
                if (!item.isRecongenized)
                {
                    item.isRecongenized = true;
                    data.detectSampleNum++;
                }
            }
        }
    }
}
