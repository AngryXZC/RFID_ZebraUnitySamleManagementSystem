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
    /// ������Э��
    /// </summary>

    private GangwayDoorDevice doorDevice;
    /// <summary>
    /// �豸������ʾ����
    /// </summary>
    public Text deviceInfo;
    public Text deviceStatus;
    /// <summary>
    /// ��ť�Ƿ�ɽ���
    /// </summary>
    public List<Button> interactButtons;
    /// <summary>
    /// ����˵�ҳ��ر��̵㹦��
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
            deviceInfo.text = "ͨ�����豸";
            deviceStatus.text = "δ����";
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
                deviceInfo.text = "RFID�豸";
                deviceStatus.text = "������";
            }
            catch (Exception e)
            {
                doorDevice.closeInventory();
            }

        }
    }

    //���������豸��Э��
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

    //�ȴ�2s
    IEnumerator waitForTime() 
    {
        yield return new WaitForSeconds(5f);
    }
    
    private void connectDevicCortine()
    {
        Debug.Log("��־��" + doorDevice.IsConnect);
        Debug.Log("Э�̿�����");
        if (doorDevice != null && (!doorDevice.IsConnect))
        {
            while (!doorDevice.IsConnect)
            {
                Debug.Log("�������ӣ�");
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
