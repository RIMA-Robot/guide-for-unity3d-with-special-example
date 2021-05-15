using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Xfrog.Net;

public class MOVE : MonoBehaviour {

    public Text text;
    public Text input;
    AudioClip audioClip;
    AudioSource audioSource;

    // Use this for initialization
    void Start() {
        audioSource = GameObject.Find("Canvas/Audio Source").GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {


    }
    public static string HttpGet(string Url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();
        return retString;
    }

    public void ok()
    {
        text = GameObject.Find("Canvas/Text").GetComponent<Text>();
        input = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>(); ;
        text.text += "我：" + input.text + "\n";
        /// string url = "http://www.tuling123.com/openapi/api";
        ///string key = "7c664d28fa0b472ab9833c2679c431f5";
        // string postDataStr = "key=" + key + "&info=" + input.text;
        string url = "http://rimaapi20180325.azurewebsites.net/api/Talk";
          string postDataStr = "request="+ input.text;
        string result = HttpGet(url, postDataStr);
        result = result.Replace(" ", "1111");
        JsonObject newObj = new JsonObject(result);
        string info = newObj["info"].Value.ToString();
        info = info.Replace("1111", " ");
        text.text += "rima:" + info + "\n";
        print(info);
     

        string url_speaker = "http://tsn.baidu.com/text2audio";
        string postDataStr_speaker = "tex=" + info + "&lan=zh&cuid=B8-81-98-41-3E-E9&ctp=1&tok=24.d1ba8c1f1efa8a3de68678e5404d55a4.2592000.1523629153.282335-10681472&ctp=1&cuid=10681472";
        string req = url_speaker + "?" + postDataStr_speaker;

        StartCoroutine(DownloadAndPlay(req));
  
        text.text += "over";

    }
    IEnumerator DownloadAndPlay(string url)
    {
        WWW www = new WWW(url);
        yield return www;
    
        audioSource.clip = www.GetAudioClip(false, true, AudioType.MPEG);
        audioSource.Play();
    }
}
