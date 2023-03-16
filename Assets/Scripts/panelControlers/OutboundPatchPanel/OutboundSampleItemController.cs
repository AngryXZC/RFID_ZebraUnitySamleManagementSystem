using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutboundSampleItemController : MonoBehaviour
{
    public Text num;
    public Text hersmanName;
    public Text sampleNum;
    public Text inspectNum;
    public Text abnormalNum;
    public Text detectNum;
    public OutboundHerdsmanListItem data;
    public void init(int number, OutboundHerdsmanListItem outboundHerdsmanListItem)
    {
        data = outboundHerdsmanListItem;
        num.text = number.ToString();
        hersmanName.text = data.herdsmanName.ToString();
        sampleNum.text = data.reciveSampleNum.ToString();
        inspectNum.text = data.hasInspectedNum.ToString();
        abnormalNum.text = data.abnormalSampleNum.ToString();
        detectNum.text = data.detectSampleNum.ToString();
    }
    private void Update()
    {
        if (data != null)
        {
            detectNum.text = data.detectSampleNum.ToString();
            if (data.detectSampleNum - data.abnormalSampleNum - data.hasInspectedNum != 0)
            {
                detectNum.color = Color.red;
            }
            else
            {
                detectNum.color = Color.green;
            }
        }
    }

    public bool getAtag(string tag)
    {
        if (data != null) { 
            foreach (var item in data.sampleList)
            {
                if ((!item.isRecongnized) && item.sampleCode == tag)
                {
                    item.isRecongnized = true;
                    data.detectSampleNum++;
                    return true;
                }
            }
        }
        return false;   
    }
}
