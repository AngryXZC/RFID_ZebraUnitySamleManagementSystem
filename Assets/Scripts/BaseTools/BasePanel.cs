using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 语音控制
    /// </summary>
    public VoiceController voiceController;
    /// <summary>
    /// 弹窗
    /// </summary>
    public GameObject baseAlertPanel;

    /// <summary>
    /// 父页面
    /// </summary>
    public GameObject parentPanel;
    /// <summary>
    /// 返回按钮
    /// </summary>
    public Button returnBtn;
   
    /// <summary>
    /// 加载界面
    /// </summary>
    public GameObject loadPanel;
    /// <summary>
    /// 返回父页面
    /// </summary>
    public void  onReturnBtnlick()
    {
        Debug.Log("点击Invoke!");
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
    /// 播放加载动画
    /// </summary>
    public void playLoadAni() 
    {
        loadPanel.SetActive(true);
    }
    /// <summary>
    /// 停止加载动画
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
        //初始化时将按钮所有监听事件移除代码中动态指定按钮的回调函数
       
        if (returnBtn!=null)
        {
            returnBtn.onClick.RemoveAllListeners();
        }
    }
}
