using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagInfoItemController : MonoBehaviour
{
    public HerdsmanListItem data;
    public Text number;
    public Text bagcode;
    public Text herdsmanName;
    public Text sampleSum;
    public Button changeBagBtn;

    private SackData currentSack;

    public void setInfo(string num, HerdsmanListItem item,SackData currentBag)
    {
        data = item;
        currentSack = currentBag;
        number.text = num;
        bagcode.text = data.bagCode;
        herdsmanName.text = data.herdsmanName;
        sampleSum.text = (data.actualSampleNum + data.detectSampleNum + data.earCodeLackSampleNum).ToString();
        changeBagBtn.onClick.AddListener(() => onChangeBagCodeClick());
    }
    private void OnDestroy()
    {
        changeBagBtn.onClick.RemoveAllListeners();
    }
    private void onChangeBagCodeClick() 
    {
        GameObject selectSackCodePanel = GameObject.Find("BagCodeAnchor").transform.Find("BagcodeSelectPanel").gameObject;
        SelectSackController selectSackController= selectSackCodePanel.GetComponent<SelectSackController>();
        selectSackCodePanel.SetActive(true);
        selectSackController.returnBtn.onClick.AddListener(()=>selectSackController.bindReturnButton());
        selectSackController.confirmBtn.onClick.AddListener(() => selectSackController.bindConfirmBtntton(this, currentSack));
     
        Debug.Log("¸ü»»´ü×Óµã»÷£¡");
    }
}
