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
    /// ���ؾ��浯��
    /// </summary>
    public void hideSelf() 
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// ����recive�����¼������ν���
    /// </summary>
    public void patchAlert(GameObject recivePanel,GameObject patchPanel) 
    {
        gameObject.SetActive(false);
        recivePanel.SetActive(false);
        patchPanel.SetActive(true);
    }
    /// <summary>
    /// ���ؾɽ�����ʾ�µĽ���
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
            Debug.Log("�Ƴ����м����¼���");
            returnBtn.onClick.RemoveAllListeners();
        }
    }

    

}
