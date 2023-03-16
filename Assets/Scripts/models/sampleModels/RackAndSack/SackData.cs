using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SackData 
{
    /// <summary>
    /// ����
    /// </summary>
    public string sackCode { get; set; }
    /// <summary>
    /// ��Ʒ����
    /// </summary>
    public int sampleSum { get; set; }
    /// <summary>
    /// �ͼ�����
    /// </summary>
    public int inspectSamSum { get; set; }

    /// <summary>
    /// ��Ӧ����
    /// </summary>
    public int detectSamSum { get; set; }
    /// <summary>
    /// ȱ����Ʒ����
    /// </summary>
    public int lackSamNum { get; set; }
    /// <summary>
    /// �޶�������
    /// </summary>
    public int noEarcodeSum { get; set; }

    /// <summary>
    /// �ޱ�ǩ����
    /// </summary>
    public int noTagSamSum { get; set; }
    /// <summary>
    /// ���ܱ��
    /// </summary>
    public string rackCode { get; set; }
    /// <summary>
    /// �ô��а���������
    /// </summary>
    public List<HerdsmanListItem> herdsmanItems { get; set; }

}
