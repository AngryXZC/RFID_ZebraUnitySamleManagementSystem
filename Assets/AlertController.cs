using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertController : MonoBehaviour
{
    public Text alerInfo;
    public Button returnBtn;
    public Button continueBtn;
    public void  setAlertInfo(string mes)
    {
        alerInfo.text = mes;
    }
    public void  setButonActivate(bool on_off) 
    {
        if (returnBtn!=null)
        {
            returnBtn.gameObject.SetActive(on_off);
        }
    }
    /// <summary>
    /// 隐藏警告弹窗
    /// </summary>
    public void hideSelf() 
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 隐藏recive，重新加载批次界面
    /// </summary>
    public void patchAlert(GameObject recivePanel,GameObject patchPanel) 
    {
        gameObject.SetActive(false);
        recivePanel.SetActive(false);
        patchPanel.SetActive(true);
    }
    /// <summary>
    /// 隐藏旧界面显示新的界面
    /// </summary>
    /// <param name="newPanel"></param>
    /// <param name="oldPanel"></param>
    public void hideSelfAndReturnTonew(GameObject newPanel,GameObject oldPanel)
    {
        gameObject.SetActive(false);
        oldPanel.SetActive(false);
        newPanel.SetActive(true);
    }
    private void OnDisable()
    {
        if (returnBtn.onClick!=null)
        {
            Debug.Log("移除所有监听事件！");
            returnBtn.onClick.RemoveAllListeners();
        }
    }

    

}
