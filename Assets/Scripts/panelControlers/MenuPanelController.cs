using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Symbol.RFID3;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelController : MonoBehaviour
{
#pragma warning disable 0168
    /// <summary>
    /// 连接子协程
    /// </summary>

    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// 设备连接提示文字
    /// </summary>
    public Text deviceInfo;
    public Text deviceStatus;
    /// <summary>
    /// 按钮是否可交互
    /// </summary>
    public List<Button> interactButtons;
    /// <summary>
    /// 进入菜单页则关闭盘点功能
    /// </summary>
    private void OnDisable()
    {

        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        StopAllCoroutines();
    }
    private void OnEnable()
    {
        doorDevice = GangwayDoorDevice.getInstance();
      
        if (doorDevice.IsConnect && doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        StartCoroutine(waitForTime());
        StartCoroutine(startConnectCortine());
    }

    private void Update()
    {
       
        if (!doorDevice.IsConnect)
        {
            deviceInfo.text = "通道门设备";
            deviceStatus.text = "未连接";
            deviceStatus.color = Color.red;
            //foreach (Button button in interactButtons)
            //{
            //    button.interactable = false;
            //}
        }
        else
        {
            //foreach (Button button in interactButtons)
            //{
            //    button.interactable = true;
            //}
            try
            {
                // deviceInfo.text = doorDevice.ReaderAPI.ReaderCapabilities.ModelName;
                deviceInfo.text = "RFID设备";
                deviceStatus.text = "已连接";
            }
            catch (Exception e)
            {
                doorDevice.closeInventory();
            }

        }
    }

    //开启连接设备子协程
    IEnumerator startConnectCortine()
    {
        
        while (true)
        {
            if (!doorDevice.IsConnect)
            {
                connectDevicCortine();
            }
            Debug.Log(doorDevice.IsConnect);
            yield return new WaitForSeconds(5f);
        }
    }

    //等待2s
    IEnumerator waitForTime() 
    {
        yield return new WaitForSeconds(5f);
    }
    
    private void connectDevicCortine()
    {
        Debug.Log("标志：" + doorDevice.IsConnect);
        Debug.Log("协程开启！");
        if (doorDevice != null && (!doorDevice.IsConnect))
        {
            while (!doorDevice.IsConnect)
            {
                Debug.Log("尝试连接！");
                doorDevice.connectDevice();
                      
            }
        }

    }
    public void exitApplication()
    {
#if UNITY_EDITOR


        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        if (doorDevice.IsConnect)
        {
            doorDevice.closeInventory();
        }
        StopAllCoroutines();
        UnityEditor.EditorApplication.isPlaying = false;
#else

           Debug.Log(doorDevice.IsConnect);

        if (doorDevice.IsInventory)
        {
            doorDevice.StopInventory();
        }
        if (doorDevice.IsConnect)
        {
            doorDevice.closeInventory();
        }
        StopAllCoroutines();
    
Application.Quit();
#endif
    }


}
