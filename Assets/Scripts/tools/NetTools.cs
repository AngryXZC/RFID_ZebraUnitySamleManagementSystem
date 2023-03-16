using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;
using System.Web;
using UnityEngine.Networking;
using LitJson;

public class NetTools : MonoBehaviour
{
    private static readonly HttpClient client = new HttpClient();
    public static readonly string url= "http://10.255.255.244:8080/jeecg-boot/sample/sample/sampleReceiveList";
    /// <summary>
    /// 异步通信方式
    /// </summary>
    public static async void get()
    {
        var responseString = await client.GetStringAsync("http://127.0.0.1:23/api");
        Debug.Log(responseString);
    }
    public static async void getTagsAboutByPost(string applyCode)
    {
        // 创建一个字典，添加数据
        Dictionary<string, string> values = new Dictionary<string, string>();
        values.Add("apply_code", applyCode);

        // 数据转化为 key=val 格式
        var content = new FormUrlEncodedContent(values);

        // 发送请求
        var response = await client.PostAsync(url, content);
        // 获取数据
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
    }
    /// <summary>
    /// 发送请求的同步方法
    /// </summary>
    /// <param name="Url">地址</param>
    /// <param name="postDataStr">数据</param>
    /// <returns></returns>
    public static string HttpPost(string Url, string postDataStr)
    {
        //返回数据
        string retString = null;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        try
        {
            
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.ReadWriteTimeout = 500000000;
            using (Stream myRequestStream = request.GetRequestStream())
            {
                myRequestStream.Flush();
                using (StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312")))
                {
                    myStreamWriter.Write(postDataStr);
                    myStreamWriter.Flush();
                    myStreamWriter.Close();
                    myRequestStream.Flush();
                    myRequestStream.Close();

                }
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream myResponseStream = response.GetResponseStream())
                {
                    myResponseStream.Flush();
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myStreamReader.Dispose();
                        myResponseStream.Flush();
                        myResponseStream.Close();

                    }
                }
            }
        }
        catch (System.Exception)
        {
            throw;
        }
        finally 
        {
           request.Abort();
        }
        return retString;
    }

    public static string HttpGet(string Url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        request.Method = "GET";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ReadWriteTimeout = 500000000;
        string retString=string.Empty;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) 
        {
            using (Stream myResponseStream = response.GetResponseStream())
            {
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                myResponseStream.Flush();
                myResponseStream.Close();
                myStreamReader.Close();
            }
            
        }

        return retString;
    }

    public static string HttpPostFin(string Url, string postDataStr)
    {

        Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
        parameters.Add("params", postDataStr);
        string url = Url;
        string retString = string.Empty;
        HttpWebRequest request = null;
        HttpWebResponse response = null;
        Stream reqStream = null;
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "post"; //设置为post请求
        request.ReadWriteTimeout = 5000;
        request.KeepAlive = false;
        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
        byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8")); //使用utf-8格式组装post参数
        reqStream = request.GetRequestStream();
        reqStream.Write(postData, 0, postData.Length);
        response = (HttpWebResponse)request.GetResponse();
        using (Stream myResponseStream = response.GetResponseStream())
        {
            
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            retString = myStreamReader.ReadToEnd();
        }
        return retString;
    }
    //组装请求参数
    public static string BuildQuery(IDictionary<string, string> parameters, string encode)
    {
        StringBuilder postData = new StringBuilder();
        bool hasParam = false;
        IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
        while (dem.MoveNext())
        {
            string name = dem.Current.Key;
            string value = dem.Current.Value;
            // 忽略参数名或参数值为空的参数
            if (!string.IsNullOrEmpty(name))
            {
                if (hasParam)
                {
                    postData.Append("&");
                }
                postData.Append(name);
                postData.Append("=");
                if (encode == "gb2312")
                {
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                }
                else if (encode == "utf8")
                {
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                }
                else
                {
                    postData.Append(value);
                }
                hasParam = true;
            }
        }
        return postData.ToString();
    }

    //使用Unity发送类发送
    IEnumerator UntiyGet(string url, string getDataStr, string result)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.result==UnityWebRequest.Result.ProtocolError || webRequest.result==UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            result = webRequest.downloadHandler.text;
            Debug.Log(webRequest.downloadHandler.text);
        }
      
    }


    public IEnumerator UntiyPost(string url, byte[] databyte)
    {
        
        //byte[] databyte = Encoding.UTF8.GetBytes(jsondata);
        UnityWebRequest _request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        _request.uploadHandler = new UploadHandlerRaw(databyte);
        _request.downloadHandler = new DownloadHandlerBuffer();
        _request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
        yield return _request.SendWebRequest();
        Debug.Log(_request.responseCode);

        if (_request.result == UnityWebRequest.Result.ProtocolError || _request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(_request.error);
        }
        else
        {
            
            Debug.Log(_request.downloadHandler.text);
        }
    }
}
