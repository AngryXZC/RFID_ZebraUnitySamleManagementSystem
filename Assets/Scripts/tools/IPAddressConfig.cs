using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPAddressConfig
{
    /// <summary>
    /// ����
    /// </summary>
    //private static string taretIPAddress = "http://192.168.1.150:8080/";
    ////����
    //private static string taretIPAddress1 = "http://192.168.1.150:8080/";
    ///<smmary>
    ///�Ʒ�������ַ
    /////</smmary>>
    //private static string taretIPAddress = "http://cloud.nmgwide.com:32385/";
    //private static string taretIPAddress1 = "http://cloud.nmgwide.com:32385/";
    //��ʽ���Ե�ַ
    private static string taretIPAddress ;
    private static string taretIPAddress1 ;
    private static string iniPath = Application.streamingAssetsPath + "/config.ini";
    private static INIParser iNIParser;
    ////��ʽ��ַ
    //private static string taretIPAddress = "http://192.168.1.115:8080/";
    //private static string taretIPAddress1 = "http://192.168.1.115:8080/";


    static IPAddressConfig() {
        /// <summary>
        /// �����ļ�·��
        /// </summary>

        iNIParser = new INIParser();
        ////��ʽ��ַ
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
    //��Ʒ����
    /// <summary>
    /// ����ѡ���ַ
    /// </summary>
    public static string patchPanelAddress;
    /// <summary>
    /// ��ȡ�ⷿ������Ϣ
    /// </summary>
    public static string warehouseAndrack ;
    /// <summary>
    /// ȷ���ύ��ַ
    /// </summary>
    public static string submitReciveSampleResAdress;
    //�����ύ�µĵ�ַ
    //public static string submitReciveSampleResAdress = taretIPAddress1 + "jeecg-boot/sample/sample/receivetest";
    /// <summary>
    /// ������Ʒʱ���ص�������ϸ��Ϣ
    /// </summary>
    public static string sampleReciveAdderss;
    //��Ʒ��ȡ
    /// <summary>
    /// ��Ʒ��ȡ��ˢ��
    /// </summary>
    public static string ledSampleManIdentify;
    /// <summary>
    /// ����ʱ�ⷿ����Աˢ��
    /// </summary>
    public static string warehouseManIdentify ;
    //��ϴ��Ʒ
    /// <summary>
    /// ��ϴ��Ʒ��Աˢ��
    /// </summary>
    public static string finishCleanManIdentify ;
    /// <summary>
    /// ��ϴ�����ύ����
    /// </summary>
    public static string submitFinishClean ;
    //�����Ʒ
    public static string getAtagInfo;
    //��Ʒ����
    /// <summary>
    /// ������ˢ��
    /// </summary>
    public static string sampleReciveManConfirm ;
    /// <summary>
    /// ������ȷ��
    /// </summary>
    public static string sampleDeliverConfirm ;
    //���ݵ绰�Ų�ѯ��Ա����
    public static string getManNameByPhone ;
    //����
    /// <summary>
    /// ��������ѡ���б�
    /// </summary>
    public static string getOutboundPatchPanel;
    /// <summary>
    /// ��������ϸ��Ϣ
    /// </summary>
    public static string getOutboundPatchInfo;
    /// <summary>
    /// ���������Ʒ��Ų�ѯ
    /// </summary>
    public static string outboundQuery;
    /// <summary>
    /// �ⷿ������Աˢ��
    /// </summary>
    public static string outboundConfirm ;
    //��Ʒ�˻�
    /// <summary>
    /// ��Ʒ�˻����˻���ˢ���ӿ�
    /// </summary>
    public static string backSampleConfirm ;
    /// <summary>
    /// ��Ʒ�˻����ⷿ����Աȷ��ˢ��
    /// </summary>
    public static string backSamolePutup;

}
