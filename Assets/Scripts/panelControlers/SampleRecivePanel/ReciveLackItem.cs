using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReciveLackItem : MonoBehaviour
{
    public Text number;
    public Text herdsmanName;
    public Text sampleCode;
    public void setInfo(string num,string name,string samCode)
    {
        number.text = num;
        herdsmanName.text = name;
        sampleCode.text = samCode;
    }
}
