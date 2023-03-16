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
    /// �첽ͨ�ŷ�ʽ
    /// </summary>
    public static async void get()
    {
        var responseString = await client.GetStringAsync("http://127.0.0.1:23/api");
        Debug.Log(responseString);
    }
    public static async void getTagsAboutByPost(string applyCode)
    {
        // ����һ���ֵ䣬�������
        Dictionary<string, string> values = new Dictionary<string, string>();
        values.Add("apply_code", applyCode);

        // ����ת��Ϊ key=val ��ʽ
        var content = new FormUrlEncodedContent(values);

        // ��������
        var response = await client.PostAsync(url, content);
        // ��ȡ����
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
    }
    /// <summary>
    /// ���������ͬ������
    /// </summary>
    /// <param name="Url">��ַ</param>
    /// <param name="postDataStr">����</param>
    /// <returns></returns>
    public static string HttpPost(string Url, string postDataStr)
    {
        //��������
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

        Dictionary<string, string> parameters = new Dictionary<string, string>(); //�����б�
        parameters.Add("params", postDataStr);
        string url = Url;
        string retString = string.Empty;
        HttpWebRequest request = null;
        HttpWebResponse response = null;
        Stream reqStream = null;
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "post"; //����Ϊpost����
        request.ReadWriteTimeout = 5000;
        request.KeepAlive = false;
        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
        byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8")); //ʹ��utf-8��ʽ��װpost����
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
    //��װ�������
    public static string BuildQuery(IDictionary<string, string> parameters, string encode)
    {
        StringBuilder postData = new StringBuilder();
        bool hasParam = false;
        IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
        while (dem.MoveNext())
        {
            string name = dem.Current.Key;
            string value = dem.Current.Value;
            // ���Բ����������ֵΪ�յĲ���
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

    //ʹ��Unity�����෢��
    IEnumerator UntiyGet(string url, string getDataStr, string result)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url + (getDataStr == "" ? "" : "?") + getDataStr);

        yield return webRequest.SendWebRequest();
        //�쳣�����ܶ಩������error!=null���Ǵ���ģ��뿴�����������Բ���
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
