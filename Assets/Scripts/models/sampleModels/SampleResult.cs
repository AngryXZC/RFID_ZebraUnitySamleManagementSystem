using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleResult 
{
    /// <summary>
    /// �ⷿ������Ա�ֻ���
    /// </summary>
    private string creator_phone;
    /// <summary>
    /// �ͼ쵥λ����
    /// </summary>
    private string applyCode;
    /// <summary>
    /// �ͼ쵥λ����
    /// </summary>
    private string inspectDepartmentName;
    /// <summary>
    /// ʵ����Ʒ����
    /// </summary>
    private int actualSampleSum;

    /// <summary>
    /// �����б�
    /// </summary>
    private List<HerdsmanListItem> herdsmanLists;
    /// <summary>
    /// ��⵽����������
    /// </summary>
    private int detectHermanNum;

    public List<HerdsmanListItem> HerdsmanLists { get => herdsmanLists; set => herdsmanLists = value; }
    public int DetectHermanNum { get => detectHermanNum; set => detectHermanNum = value; }
    public string ApplyCode { get => applyCode; set => applyCode = value; }
    public string InspectDepartmentName { get => inspectDepartmentName; set => inspectDepartmentName = value; }
    public int ActualSampleNum { get => actualSampleSum; set => actualSampleSum = value; }
    public string Creator_phone { get => creator_phone; set => creator_phone = value; }
}
