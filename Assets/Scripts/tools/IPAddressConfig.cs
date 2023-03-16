using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPAddressConfig
{
    /// <summary>
    /// 刘澳
    /// </summary>
    //private static string taretIPAddress = "http://192.168.1.150:8080/";
    ////段宁
    //private static string taretIPAddress1 = "http://192.168.1.150:8080/";
    ///<smmary>
    ///云服务器地址
    /////</smmary>>
    //private static string taretIPAddress = "http://cloud.nmgwide.com:32385/";
    //private static string taretIPAddress1 = "http://cloud.nmgwide.com:32385/";
    //正式测试地址
    private static string taretIPAddress ;
    private static string taretIPAddress1 ;
    private static string iniPath = Application.streamingAssetsPath + "/config.ini";
    private static INIParser iNIParser;
    ////正式地址
    //private static string taretIPAddress = "http://192.168.1.115:8080/";
    //private static string taretIPAddress1 = "http://192.168.1.115:8080/";


    static IPAddressConfig() {
        /// <summary>
        /// 配置文件路径
        /// </summary>

        iNIParser = new INIParser();
        ////正式地址
        iNIParser.Open(iniPath);
        IPAddressConfig.taretIPAddress = iNIParser.ReadValue("ServerHost", "hostRear", "null");
        IPAddressConfig.taretIPAddress1 = iNIParser.ReadValue("ServerHost", "hostRear", "null");
        patchPanelAddress = taretIPAddress1 + "jeecg-boot/sample/sample/batchList";
        warehouseAndrack = taretIPAddress + "jeecg-boot/sample/sample/roomAndRack";
        submitReciveSampleResAdress = taretIPAddress1 + "jeecg-boot/sample/sample/receive";
        sampleReciveAdderss = taretIPAddress1 + "jeecg-boot/sample/sample/sampleReceiveList";
        ledSampleManIdentify = taretIPAddress1 + "jeecg-boot/sample/sample/sampleReceive";
        warehouseManIdentify = taretIPAddress1 + "jeecg-boot/sample/sample/startCollectingSamples";
        finishCleanManIdentify = taretIPAddress + "jeecg-boot/wash/api/wash";
        submitFinishClean = taretIPAddress + "jeecg-boot/wash/api/accomplishWash";
        getAtagInfo = taretIPAddress + "jeecg-boot/wash/api/checkSample";
        sampleReciveManConfirm = taretIPAddress1 + "jeecg-boot/deliver/api/sampleDeliver";
        sampleDeliverConfirm = taretIPAddress1 + "jeecg-boot/deliver/api/sampleTransfer";
        getManNameByPhone = taretIPAddress1 + "jeecg-boot/deliver/api/queryPersonnel";
        getOutboundPatchPanel = taretIPAddress + "jeecg-boot/wash/api/OutSampleList";
        getOutboundPatchInfo = taretIPAddress + "jeecg-boot/wash/api/OutSample";
        outboundQuery = taretIPAddress + "jeecg-boot/wash/api/OutSample";
        outboundConfirm = taretIPAddress + "jeecg-boot/wash/api/confirmOutSample";
        backSampleConfirm = taretIPAddress + "jeecg-boot/return/api/sampleReturn";
        backSamolePutup = taretIPAddress + "jeecg-boot/return/api/sampleReturnOk";
        Debug.Log(taretIPAddress);
        iNIParser.Close();
       
    }
    //样品接收
    /// <summary>
    /// 批次选择地址
    /// </summary>
    public static string patchPanelAddress;
    /// <summary>
    /// 获取库房货架信息
    /// </summary>
    public static string warehouseAndrack ;
    /// <summary>
    /// 确认提交地址
    /// </summary>
    public static string submitReciveSampleResAdress;
    //接收提交新的地址
    //public static string submitReciveSampleResAdress = taretIPAddress1 + "jeecg-boot/sample/sample/receivetest";
    /// <summary>
    /// 接收样品时返回的批次详细信息
    /// </summary>
    public static string sampleReciveAdderss;
    //样品领取
    /// <summary>
    /// 样品领取人刷卡
    /// </summary>
    public static string ledSampleManIdentify;
    /// <summary>
    /// 领样时库房管理员刷卡
    /// </summary>
    public static string warehouseManIdentify ;
    //清洗样品
    /// <summary>
    /// 清洗样品人员刷卡
    /// </summary>
    public static string finishCleanManIdentify ;
    /// <summary>
    /// 清洗结束提交数据
    /// </summary>
    public static string submitFinishClean ;
    //清点样品
    public static string getAtagInfo;
    //样品交接
    /// <summary>
    /// 接收人刷卡
    /// </summary>
    public static string sampleReciveManConfirm ;
    /// <summary>
    /// 交样人确认
    /// </summary>
    public static string sampleDeliverConfirm ;
    //根据电话号查询人员名称
    public static string getManNameByPhone ;
    //出库
    /// <summary>
    /// 出库批次选择列表
    /// </summary>
    public static string getOutboundPatchPanel;
    /// <summary>
    /// 单批次详细信息
    /// </summary>
    public static string getOutboundPatchInfo;
    /// <summary>
    /// 出库根据样品编号查询
    /// </summary>
    public static string outboundQuery;
    /// <summary>
    /// 库房管理人员刷卡
    /// </summary>
    public static string outboundConfirm ;
    //样品退还
    /// <summary>
    /// 样品退还，退还人刷卡接口
    /// </summary>
    public static string backSampleConfirm ;
    /// <summary>
    /// 样品退还，库房管理员确认刷卡
    /// </summary>
    public static string backSamolePutup;

}
