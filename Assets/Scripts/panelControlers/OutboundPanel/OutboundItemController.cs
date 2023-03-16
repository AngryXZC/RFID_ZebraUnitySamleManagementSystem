using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutboundItemController : MonoBehaviour
{
    public Text num;
    public Text herdsmanName;
    public Text numOfReciveSample;
    public Text numOfInspectedSample;
    public Text numOfAbnormalSample;
    public Text numOfDetectSample;
    public OutboundListRes data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (data != null)
        {
            numOfDetectSample.text = data.detectSampleNum.ToString();
            if (data.detectSampleNum - data.reciveSampleNum != 0)
            {
                numOfDetectSample.color = Color.red;
            }
            else
            {
                numOfDetectSample.color = Color.black;
            }
        }
    }

    public void init(string number, OutboundListRes result)
    {
        this.data = result;
        num.text = number;
        herdsmanName.text = data.herdsmanName;
        numOfReciveSample.text = data.reciveSampleNum.ToString();
        numOfInspectedSample.text= data.hasInspectedNum.ToString();
        numOfAbnormalSample.text= data.abnormalSampleNum.ToString();
        numOfDetectSample.text= data.detectSampleNum.ToString();
    }

    public bool getAtag(string code)
    {
        foreach (var item in data.reciveSampleList)
        {
            if (item.sampleCode == code)
            {
                if (!item.isRecongenized)
                {
                    item.isRecongenized = true;
                    data.detectSampleNum++;
                    Debug.Log(data.detectSampleNum);
                    return true;
                }
            }
        }
        return false;
    }
}
