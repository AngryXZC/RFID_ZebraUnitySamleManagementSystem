using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassSampleItemController : MonoBehaviour
{
    public Text num;
    public Text herdsmanName;
    public Text reciveNum;
    public Text cleanNum;
    public Text detectNum;
    public Text abnormalBackNum;
    public Text inspectName;
    public PassSampleListItem data;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (data.abnormalBackNum + System.Convert.ToInt32(detectNum.text) != data.cleanSampleCount)
        {
            detectNum.color = Color.red;
        }
        else
        {
            detectNum.color = Color.green;
        }
        detectNum.text =data.detectSampleNum.ToString();
    }
    public void init(string number,PassSampleListItem res,string inspectMan)
    {
        data = res;
        num .text = number;
        herdsmanName .text = data.herdsmanName;
        reciveNum.text = data.reciveSampleCount.ToString();
        cleanNum.text=data.cleanSampleCount.ToString();
        detectNum.text=data.detectSampleNum.ToString();
        abnormalBackNum.text=data.abnormalBackNum.ToString();
        inspectName.text = inspectMan;
    }

    public void getAtag(string tag)
    {
        foreach (PassSampleItem item in data.cleanSampleList)
        {
            if (tag==item.sampleCode)
            {
                item.isRecongnized = true;
                data.detectSampleNum++;
            }
        }
    }
}
