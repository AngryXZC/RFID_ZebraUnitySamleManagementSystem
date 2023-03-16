using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PatchItem : BasePanel
{

    //public GameObject recivePanel;
    public GameObject sigleOrMutiPanel;
    public GameObject patchPanel;
    public Button reciveBtn;
    
    /// <summary>
    /// ���ݰ�ʵ��
    /// </summary>
    public PatchResultItem patchResultItem;
  /// <summary>
  /// ��������
  /// </summary>
   public ReciveEnum reciveEnum;
    public void reciveBtnClick()
    {
       // SampleReciveController controller = recivePanel.GetComponent<SampleReciveController>();
        SingleOrMutilController controller=sigleOrMutiPanel.GetComponent<SingleOrMutilController>();  
        controller.reciveType = reciveEnum;
        controller.setcurrentApplyCode(patchResultItem.applyCode,patchResultItem.title);
        //��ת
       // Debug.Log(gameObject.name+patchResultItem.applyCode);
        patchPanel.SetActive(false);
        sigleOrMutiPanel.SetActive(true);
    }
    private void OnEnable()
    {
        initLoadPanel();
        //recivePanel = GameObject.Find("ReciveAnchor").transform.Find("SampleRecivePanel").gameObject;
        sigleOrMutiPanel= GameObject.Find("ReciveAnchor").transform.Find("SingleReciveOrMul").gameObject;
        patchPanel = GameObject.FindWithTag("patchPanel");
       
        reciveBtn.onClick.AddListener(reciveBtnClick);

    }
    private void OnDisable()
    {
        reciveBtn.onClick.RemoveListener(reciveBtnClick);
    }

}
