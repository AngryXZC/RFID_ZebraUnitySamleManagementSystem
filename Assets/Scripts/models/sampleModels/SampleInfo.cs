using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleInfo
{
    /// <summary>
    /// �Ƿ�����ɹ�
    /// </summary>
    public bool success { get; set; }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public string message { get; set; }
    /// <summary>
    /// ������
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public InstitutionInfo result { get; set; }
    /// <summary>
    /// ʱ���
    /// </summary>
    public long timestamp { get; set; }
}
