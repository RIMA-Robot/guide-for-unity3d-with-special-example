/// 脚本位置：UGUI按钮组件身上  
/// 脚本功能：实现按钮长按状态的判断  
/// 创建时间：2015年11月17日  
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Net;
using Xfrog.Net;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;
// 继承：按下，抬起和离开的三个接口  
public class AsrResponse
{
    public int err_no;
    public string err_msg;
    public string sn;
    public string[] result;

    public static AsrResponse CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<AsrResponse>(jsonString);
    }
}

public class Rimatalker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public int model = 2;//RIMA模式或者图灵模式

    bool isSpeaking = false;

    // 延迟时间  
    private float delay = 0.2f;

    // 按钮是否是按下状态  
    private bool isDown = false;

    // 按钮最后一次是被按住状态时候的时间  
    private float lastIsDownTime;

    public Text text;

    AudioClip audioClip;
    AudioSource audioSource;
    public int recordTime = 10;
    public bodyController CF;
    //// Use this for initialization
    void Start()
    {
        CF = GameObject.Find("RimaVirtualBody/rima").GetComponent<bodyController>();
        text = GameObject.Find("Canvas/Model/chatapi").GetComponent<Text>();
        audioSource = GameObject.Find("Canvas/Audio Source").GetComponent<AudioSource>();
        string[] md = Microphone.devices;
        int mdl = md.Length;
        if (mdl == 0)
        {
            Debug.Log("no microphone found");
        }
    }






    void Update()
    {
        if (isSpeaking == true)
        {
            if (audioSource.isPlaying==false)
            { isSpeaking = false; print(isSpeaking+"5644");
                CF.idle();
            }
        }

    }

    // 当按钮被按下后系统自动调用此方法  
    public void OnPointerDown(PointerEventData eventData)
    {
        CF.hear();
        isDown = true;
        lastIsDownTime = Time.time;
        Debug.Log("按下");
        audioClip = Microphone.Start(null, false, recordTime, 16000);
        Debug.Log("开始录音.....");
        // if(Microphone.GetPosition())
        if (!Microphone.IsRecording(null))
        {
            Debug.Log("没有声音.....");
            return;
        }
        Microphone.GetPosition(null);
    }

    // 当按钮抬起的时候自动调用此方法  
    public void OnPointerUp(PointerEventData eventData)
    {
        CF.talk();
        isDown = false;

        StartCoroutine(PostAudio());

    }
    IEnumerator PostAudio()
    {
        Debug.Log("抬起");
        //*************使用语音识别api
        byte[] buffer = ConvertAudioClipToPCM16(audioClip);
        //byte[] buffer = GetClipData(); 
        yield return 2;
        HttpWebRequest request = null;
        //request = (HttpWebRequest)HttpWebRequest.Create("https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=ZH-CN&format=detailed");
        request = (HttpWebRequest)HttpWebRequest.Create("http://vop.baidu.com/server_api?lan=zh&cuid=B8-81-98-41-3E-E9&token=24.91d00cdafeef1490ec706f7e2f2659e1.2592000.1524029061.282335-10681472");
        request.SendChunked = true;
        request.Accept = @"application/json;text/xml";
        request.Method = "POST";
        request.ProtocolVersion = HttpVersion.Version11;
        // request.ContentType = @"audio/wav; codec=audio/pcm; samplerate=16000";
        request.ContentType = @"audio/pcm; rate = 16000";
        request.Headers["Ocp-Apim-Subscription-Key"] = "e8cd273d62c347cb9f64d6b94b94435d";
        request.ContentLength = buffer.Length;
        // Send an audio file by 1024 byte chunks
        /*
        * Open a request stream and write 1024 byte chunks in the stream one at a time.
        */
        Debug.Log("ResponseS");
        yield return 6;
        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(buffer, 0, buffer.Length);
        }
        Debug.Log("Response:");
        yield return 6;
        string responseString;
        WebResponse response = request.GetResponse();
        yield return 2;
        StreamReader sr = new StreamReader(response.GetResponseStream());
        responseString = sr.ReadToEnd();
        yield return 2;
        string info = null;

        if (AsrResponse.CreateFromJSON(responseString).err_msg == "success.")
        {

            responseString = AsrResponse.CreateFromJSON(responseString).result[0];

            Debug.Log(responseString);

            ////************图灵api得到回答
            //string url = "http://www.tuling123.com/openapi/api";
            //string key = "7c664d28fa0b472ab9833c2679c431f5";
            //string postDataStr = "key=" + key + "&info=" + responseString;
            //string result = HttpGet(url, postDataStr);
            //JsonObject newObj = new JsonObject(result);
            //info = newObj["text"].Value.ToString();
            //Debug.Log(info);

            //************图灵api得到回答
            //string url = "http://www.tuling123.com/openapi/api";
            //string key = "7c664d28fa0b472ab9833c2679c431f5";

            string url;
            if (text.text == "RIMA混合模式")
            {
                url = "http://rimaapi20180325.azurewebsites.net/api/Talk?request=" + responseString;
                string result = HttpGet(url);
                JsonObject newObj = new JsonObject(result);
                info = newObj["info"].Value.ToString();
            }
            else
            {
                url = "http://www.tuling123.com/openapi/api?key=7c664d28fa0b472ab9833c2679c431f5&info=" + responseString;
                string result = HttpGet(url);
                JsonObject newObj = new JsonObject(result);
                info = newObj["text"].Value.ToString();
            }
        }
        else
        {
            info = "我听不清";

        }
        yield return 2;
        string url_speaker = "http://tsn.baidu.com/text2audio";
        string postDataStr_speaker = "tex=" + info + "&lan=zh&per=1&cuid=B8-81-98-41-3E-E9&ctp=1&tok=24.d1ba8c1f1efa8a3de68678e5404d55a4.2592000.1523629153.282335-10681472&ctp=1&cuid=10681472";
        string req = url_speaker + "?" + postDataStr_speaker;

        StartCoroutine(DownloadAndPlay(req));

    }



    IEnumerator DownloadAndPlay(string url)
    {
        WWW www = new WWW(url);
        yield return www;
       

        audioSource.clip = www.GetAudioClip(false, true, AudioType.MPEG);
        isSpeaking = true;
      
        audioSource.Play();
  print(isSpeaking);
    }



    // 当鼠标从按钮上离开的时候自动调用此方法  
    public void OnPointerExit(PointerEventData eventData)
    {
        isDown = false;
    }
    public static byte[] ConvertAudioClipToPCM16(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        var samples_int16 = new short[samples.Length];

        for (var index = 0; index < samples.Length; index++)
        {
            var f = samples[index];
            samples_int16[index] = (short)(f * short.MaxValue);
        }

        var byteArray = new byte[samples_int16.Length * 2];
        Buffer.BlockCopy(samples_int16, 0, byteArray, 0, byteArray.Length);

        return byteArray;
    }
        public static string HttpGet(string Url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Method = "GET";
        Debug.Log(request);
        request.ContentType = "text/html;charset=UTF-8";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();
        return retString;


    }


}