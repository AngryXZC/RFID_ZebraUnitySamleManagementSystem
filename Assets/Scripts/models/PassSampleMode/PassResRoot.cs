using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassResRoot
{
#pragma warning disable 8632
    private PassResResult result1;

    /// <summary>
    /// 
    /// </summary>
    public bool success { get; set; }
    /// <summary>
    /// �����ɹ���
    /// </summary>
    public string message { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public PassResResult? result { get => result1; set => result1 = value; }
    /// <summary>
    /// 
    /// </summary>
    public long timestamp { get; set; }
}
