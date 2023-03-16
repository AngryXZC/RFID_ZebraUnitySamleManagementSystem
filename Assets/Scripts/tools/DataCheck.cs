using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
/// <summary>
/// ����У�鹤����
/// </summary>
public class DataCheck
{
    /// <summary>
    /// ��֤�Ƿ�Ϊ�绰�ŵ�������ʽ
    /// </summary>
    /// <param name="str_handset"></param>
    /// <returns></returns>
    public static  bool isHandset(string str_handset)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57]|19[89]|166)[0-9]{8}");
    }
    /// <summary>
    /// �ж��Ƿ�Ϊ���֣����ҿ��� �жϴ�2000�굽29999������
    /// </summary>
    /// <param name="str_process"></param>
    /// <returns></returns>
    public static bool isSampleNumber(string str_process) 
    {
        // �ж��Ƿ�Ϊ���֣����ҿ��� �жϴ�2000�굽29999������
        string regexStr = string.Empty;
        regexStr = "^[0-9]+$";  //ƥ���ַ����Ŀ�ʼ�ͽ����Ƿ�Ϊ0-9������[��λ�ַ�]
        bool isNumber = Regex.IsMatch(str_process, regexStr);
        return (isNumber && str_process.Substring(0, 2) == "02");
    }
}
