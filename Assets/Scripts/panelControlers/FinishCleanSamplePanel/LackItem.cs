using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LackItem : MonoBehaviour
{
    public Text num;
    public Text herdsmanName;
    public Text code;
    public Text earcode;
    public void setInfo(string nmber,string name,string cod,string earcod)
    {
        num.text = nmber;
        herdsmanName.text = name;
        code.text = cod;
        earcode.text = earcod;
    }
}
