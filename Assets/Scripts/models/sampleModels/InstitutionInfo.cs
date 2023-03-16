using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstitutionInfo
{
    /// <summary>
    /// �ⷿ����Ա�ֻ���
    /// </summary>
    public string creator_phone { get; set; }

    /// <summary>
    /// ���α��
    /// </summary>
    public string applyCode { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    public string patchTitle { get; set; }
    /// <summary>
    /// �õ�λ�µ���������
    /// </summary>
    public int herdsmanQuantity { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    public string sampleSource { get; set; }
    /// <summary>
    /// ʵ����Ʒ����
    /// </summary>
    public int actualSampleSum { get; set; }
    /// <summary>
    /// �����������
    /// </summary>
    public int detectHermanNum { get; set; }
    /// <summary>
    /// �û����µ������б�
    /// </summary>
    public List<HerdsmanListItem> herdsmanList { get; set; }
    /// <summary>
    /// �û����µ���Ʒ����
    /// </summary>
    public int sampleQuantitys { get; set; }
}
