using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
/// <summary>
/// 数据校验工具类
/// </summary>
public class DataCheck
{
    /// <summary>
    /// 验证是否为电话号的正则表达式
    /// </summary>
    /// <param name="str_handset"></param>
    /// <returns></returns>
    public static  bool isHandset(string str_handset)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57]|19[89]|166)[0-9]{8}");
    }
    /// <summary>
    /// 判断是否为数字，并且可以 判断从2000年到29999的数据
    /// </summary>
    /// <param name="str_process"></param>
    /// <returns></returns>
    public static bool isSampleNumber(string str_process) 
    {
        // 判断是否为数字，并且可以 判断从2000年到29999的数据
        string regexStr = string.Empty;
        regexStr = "^[0-9]+$";  //匹配字符串的开始和结束是否为0-9的数字[定位字符]
        bool isNumber = Regex.IsMatch(str_process, regexStr);
        return (isNumber && str_process.Substring(0, 2) == "02");
    }
}
