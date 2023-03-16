using System;
using System.Collections;
using System.Collections.Generic;
using Symbol.RFID3;
using UnityEngine;

public class GangwayDoorDevice 
{
    
    /// <summary>
    /// 单例设计模式
    /// </summary>
    private static GangwayDoorDevice instance=new GangwayDoorDevice();
    /// <summary>
    /// 设备连接端口号
    /// </summary>
    // 测试设备（测试使用IP）;
    //private string hostname = "10.1.1.104";
    //开发使用设备IP
    private string hostname ;
    //（库房箱式设备）
    // private  string hostname = "10.1.1.100";
    //库房设备IP(门式设备)
    //private string hostname = "10.1.1.105";
    /// <summary>
    /// 标记是否连接
    /// </summary>
    private bool m_IsConnect = false;
    /// <summary>
    /// 操作设备的类
    /// </summary>
    private  RFIDReader m_ReaderAPI;
    /// <summary>
    /// 标记是否开始执行存盘操作
    /// </summary>
    private bool isInventory = false;
    /// <summary>
    /// 配置文件路径
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
    /// 连接设备
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
    /// 开启存盘
    /// </summary>
    public void StartInventory() 
    {
        if (this.IsConnect) 
        {
            Debug.Log("开启盘点！");
            this.ReaderAPI.Actions.Inventory.Perform();
            this.isInventory = true;
        }
    }
    /// <summary>
    /// 关闭存盘
    /// </summary>
    public void StopInventory()
    {
        if (this.IsConnect)
        {
            Debug.Log("关闭盘点！");
            this.ReaderAPI.Actions.Inventory.Stop();
            this.isInventory = false;
        }
    }
    /// <summary>
    /// 关闭设备
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
