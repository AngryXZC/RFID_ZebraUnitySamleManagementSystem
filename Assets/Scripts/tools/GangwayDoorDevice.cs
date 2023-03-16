using System;
using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;

public class GangwayDoorDevice 
{
    
    /// <summary>
    /// �������ģʽ
    /// </summary>
    private static GangwayDoorDevice instance=new GangwayDoorDevice();
    /// <summary>
    /// �豸���Ӷ˿ں�
    /// </summary>
    // �����豸������ʹ��IP��;
    //private string hostname = "10.1.1.104";
    //����ʹ���豸IP
    private string hostname ;
    //���ⷿ��ʽ�豸��
    // private  string hostname = "10.1.1.100";
    //�ⷿ�豸IP(��ʽ�豸)
    //private string hostname = "10.1.1.105";
    /// <summary>
    /// ����Ƿ�����
    /// </summary>
    private bool m_IsConnect = false;
    /// <summary>
    /// �����豸����
    /// </summary>
    private  RFIDReader m_ReaderAPI;
    /// <summary>
    /// ����Ƿ�ʼִ�д��̲���
    /// </summary>
    private bool isInventory = false;
    /// <summary>
    /// �����ļ�·��
    /// </summary>
    private string iniPath = Application.streamingAssetsPath + "/config.ini";
    private GangwayDoorDevice() {
        INIParser iNIParser = new INIParser();
        iNIParser.Open(iniPath);
        this.hostname = iNIParser.ReadValue("DeviceHost", "host", "null");
        iNIParser.Close();
        this.ReaderAPI = new RFIDReader(hostname, 5084, 0);
    }
    /// <summary>
    /// �����豸
    /// </summary>
    public bool connectDevice() 
    {
        try
        {
            this.ReaderAPI.Connect();
            this.IsConnect = true;
        }
        catch (System.Exception)
        {
           this.IsConnect=false;
        }
        return this.IsConnect;
    }

    

    public string Hostname { get => hostname; set => hostname = value; }
    public RFIDReader ReaderAPI { get => m_ReaderAPI; set => m_ReaderAPI = value; }
    public bool IsConnect { get => m_IsConnect; set => m_IsConnect = value; }
    public bool IsInventory { get => isInventory; set => isInventory = value; }
    /// <summary>
    /// ��������
    /// </summary>
    public void StartInventory() 
    {
        if (this.IsConnect) 
        {
            Debug.Log("�����̵㣡");
            this.ReaderAPI.Actions.Inventory.Perform();
            this.isInventory = true;
        }
    }
    /// <summary>
    /// �رմ���
    /// </summary>
    public void StopInventory()
    {
        if (this.IsConnect)
        {
            Debug.Log("�ر��̵㣡");
            this.ReaderAPI.Actions.Inventory.Stop();
            this.isInventory = false;
        }
    }
    /// <summary>
    /// �ر��豸
    /// </summary>
    /// <returns></returns>
    public bool closeInventory() 
    {
        try
        {
            this.ReaderAPI.Disconnect();
            this.IsConnect = false;
        }
        catch (System.Exception)
        {
            this.IsConnect = this.IsConnect;
        }
        return this.IsConnect;
    }
    
    public static GangwayDoorDevice getInstance() 
    {
        return instance;
    }

  
}
