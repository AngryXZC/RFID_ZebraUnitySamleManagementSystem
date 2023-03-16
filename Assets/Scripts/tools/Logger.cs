using System.IO;
using log4net;
using UnityEngine;

public static class Logger
{
    private static ILog log = LogManager.GetLogger("FileLogger"); //FileLogger

    public static void Init()
    {
        Application.logMessageReceived += onLogMessageReceived; //���unity��־����

        //ApplicationLogPath��LogFileName��log4net.config��ʹ��
        FileInfo file = new System.IO.FileInfo(Application.streamingAssetsPath + "/log4net.config"); //��ȡlog4net�����ļ�
        GlobalContext.Properties["ApplicationLogPath"] = Path.Combine(Directory.GetCurrentDirectory(), "Log"); //��־���ɵ�·��
        GlobalContext.Properties["LogFileName"] = "log"; //������־���ļ���
        log4net.Config.XmlConfigurator.ConfigureAndWatch(file); //����log4net�����ļ�

        Debug.Log("��־��ʼ��");
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
