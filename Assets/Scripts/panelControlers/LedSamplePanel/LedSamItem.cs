using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LedSamItem : MonoBehaviour
{
    public Text numText;
    public Text nameText;
    public Text inspenctNumText;
    public Text detectNumText;
    public Text inspenctNameText;
    public  CleanHerdsmanListItem data;
    // Start is called before the first frame update



    // Update is called once per frame
    void Update()
    {
        detectNumText.text = data.detectSampleNum.ToString();
        if (data.detectSampleNum - data.reciveSampleNum != 0)
        {
            detectNumText.color = Color.red;
        }
        else 
        {
            detectNumText.color = Color.green;
        }
    }
    public void  setInfo(string num,CleanHerdsmanListItem item)
    {
        data = item;
        numText.text = num;
        nameText.text = data.herdsmanName;
        inspenctNumText.text = data.reciveSampleNum.ToString();
        detectNumText.text = data.detectSampleNum.ToString();
        inspenctNameText.text = data.cleanMan;
    }
    public bool getATag(string tag)
    {
        
        foreach (var item in data.cleanSampleList)
        {
           
            if (item.sampleCode==tag&&(!item.isRecongnized))
            {
                
                item.isRecongnized = true;
                data.detectSampleNum++;
                return true;

            }
        }
        return false;
    }
}
