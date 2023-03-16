using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MutiWarehouseInItemController : MonoBehaviour
{
    public Toggle selectRackAndSack;
    public Text sackCode;
    public Text herdsmanName;
    public Text sendInspectedSum;
    public Text detectSamSum;
    public Text lackSamNum;
    public Text noEarCodeNum;
    public Text noTagNum;
    public Text rackCode;
    public HerdsmanListItem data;

    public void initData(HerdsmanListItem arg) {
        this.data = arg;
        selectRackAndSack.isOn = false;
        sackCode.text = "«Î—°‘Ò";
        herdsmanName.text = data.herdsmanName;
        sendInspectedSum.text = data.sampleQuantity.ToString();
        detectSamSum.text=data.detectSampleNum.ToString();
        lackSamNum.text=data.lackSampleNum.ToString();
        noEarCodeNum.text=data.earCodeLackSampleNum.ToString();
        noTagNum.text=data.actualSampleNum.ToString();
        rackCode.text = "«Î—°‘Ò";
    }

}
