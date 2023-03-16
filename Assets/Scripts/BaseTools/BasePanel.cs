using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// ��������
    /// </summary>
    public VoiceController voiceController;
    /// <summary>
    /// ����
    /// </summary>
    public GameObject baseAlertPanel;

    /// <summary>
    /// ��ҳ��
    /// </summary>
    public GameObject parentPanel;
    /// <summary>
    /// ���ذ�ť
    /// </summary>
    public Button returnBtn;
   
    /// <summary>
    /// ���ؽ���
    /// </summary>
    public GameObject loadPanel;
    /// <summary>
    /// ���ظ�ҳ��
    /// </summary>
    public void  onReturnBtnlick()
    {
        Debug.Log("���Invoke!");
        //if(loadPanel.activeInHierarchy==true)
        //stopLoadAni();
        if (voiceController!=null)
        {
            voiceController.stopVoiceCoroutine();
        }
        this.gameObject.SetActive(false);
        parentPanel.SetActive(true);
    }
    /// <summary>
    /// ���ż��ض���
    /// </summary>
    public void playLoadAni() 
    {
        loadPanel.SetActive(true);
    }
    /// <summary>
    /// ֹͣ���ض���
    /// </summary>
    public void stopLoadAni()
    {
        loadPanel.SetActive(false);
    }

    public void initLoadPanel()
    {
        voiceController = GameObject.FindGameObjectWithTag("Voice").GetComponent<VoiceController>();
        loadPanel = GameObject.FindGameObjectWithTag("AnimationPanel").transform.Find("LoadPanel").gameObject;
        baseAlertPanel = GameObject.FindGameObjectWithTag("AlertPanelAnchor").transform.Find("AlertPanel").gameObject;
        //��ʼ��ʱ����ť���м����¼��Ƴ������ж�ָ̬����ť�Ļص�����
       
        if (returnBtn!=null)
        {
            returnBtn.onClick.RemoveAllListeners();
        }
    }
}
