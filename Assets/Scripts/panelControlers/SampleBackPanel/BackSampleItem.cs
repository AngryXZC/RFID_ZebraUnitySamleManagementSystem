using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackSampleItem : MonoBehaviour
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

    public BackHerdsmanListItem myData;
    /// <summary>
    /// ϸ��չʾ����
    /// </summary>
    public GameObject lackSampleInfoPanel;
    public void initItem(int number, BackHerdsmanListItem result, string operateInfo)
    {

        myData = result;
        num.text = number.ToString();
        herdsmanName.text = myData.herdsmanName;
        numOfReciveSample.text = myData.reciveSampleNum.ToString();
        cleanSampleNum.text = myData.cleanSampleNum.ToString();
        detectSampleNum.text = myData.detectSampleNum.ToString();
        abnormalSampleNum.text = myData.abnormalBackSampleNum.ToString();

        cleanMan.text = result.cleanMan == null ? "����" : result.cleanMan;
        operate.text = operateInfo;

    }

    private void LateUpdate()
    {

        this.detectSampleNum.text = myData.detectSampleNum.ToString();
        

        if (myData.detectSampleNum - myData.cleanSampleNum-myData.abnormalBackSampleNum != 0)
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
            if (button != null)
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
        GameObject go = Instantiate(lackSampleInfoPanel);
        LackSampleController lackSampleController = go.GetComponent<LackSampleController>();
        if (lackSampleController != null)
        {
           
            //��������
            lackSampleController.showBackItem(myData);
        }
    }
    /// <summary>
    /// ʶ��һ����ǩ
    /// </summary>
    public void getATag(string tagCode)
    {
        
        foreach (var item in this.myData.cleanSampleList)
        {
            if (item.sampleCode == tagCode&&(!item.isRecongnized))
            {
                item.isRecongnized = true;
                myData.detectSampleNum++;
            }
        }
    }
}
