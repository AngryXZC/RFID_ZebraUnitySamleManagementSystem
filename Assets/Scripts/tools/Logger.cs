using System.IO;
using log4net;
using UnityEngine;

public static class Logger
{
    private static ILog log = LogManager.GetLogger("FileLogger"); //FileLogger

    public static void Init()
    {
        Application.logMessageReceived += onLogMessageReceived; //添加unity日志监听

        //ApplicationLogPath和LogFileName在log4net.config中使用
        FileInfo file = new System.IO.FileInfo(Application.streamingAssetsPath + "/log4net.config"); //获取log4net配置文件
        GlobalContext.Properties["ApplicationLogPath"] = Path.Combine(Directory.GetCurrentDirectory(), "Log"); //日志生成的路径
        GlobalContext.Properties["LogFileName"] = "log"; //生成日志的文件名
        log4net.Config.XmlConfigurator.ConfigureAndWatch(file); //加载log4net配置文件

        Debug.Log("日志初始化");
    }

    private static void onLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                log.ErrorFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Assert:
                log.DebugFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Exception:
                log.FatalFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            case LogType.Warning:
                log.WarnFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                break;
            default:
                log.Info(condition);
                break;
        }
    }
}
