//using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Xfrog.Net;
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
public class record : MonoBehaviour
{
    public TcpClient client;
    public BinaryReader breader;
    public BinaryWriter bwriter;
    public NetworkStream netstream;


    Text text;
    AudioClip audioClip;
    AudioSource audioSource;

    public int recordTime=5;
    //// Use this for initialization
    void Start()
    {
        client = new TcpClient("127.0.0.1", 8001);
        netstream = client.GetStream();
        breader = new BinaryReader(netstream);
        bwriter = new BinaryWriter(netstream);



        text = GameObject.Find("Canvas/Text").GetComponent<Text>();
        audioSource = GameObject.Find("Canvas/Audio Source").GetComponent<AudioSource>();
        string[] md = Microphone.devices;
        int mdl = md.Length;
        if (mdl == 0)
        {
            Debug.Log("no microphone found");
        }
    }

    //// Update is called once per frame
    //void Update () {

    //}
    public void StartRecordAudio()
    {
   
        Microphone.End(null);
        audioClip = Microphone.Start(null, false, recordTime, 16000);
        Debug.Log("开始录音.....");
        text.text += "开始录音.....";
        // if(Microphone.GetPosition())
        if (!Microphone.IsRecording(null))
        {
            Debug.Log("没有声音.....");
            return;
        }
        Microphone.GetPosition(null);

    }

    public void StopRecordAudio()
    {
        /***文件读取为字节流***
        FileInfo fi = new FileInfo("d:\\1.wav");
        FileStream fs = new FileStream("d:\\1.wav", FileMode.Open);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, buffer.Length);
        fs.Close();
        ***/
        Microphone.End(null);



        //*************使用语音识别api
        byte[] buffer = ConvertAudioClipToPCM16(audioClip);
       //byte[] buffer = GetClipData();    
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
        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(buffer, 0, buffer.Length);
        }
        Debug.Log("Response:");
        string responseString;
        WebResponse response = request.GetResponse();
        Debug.Log(((HttpWebResponse)response).StatusCode);
        StreamReader sr = new StreamReader(response.GetResponseStream());
        responseString = sr.ReadToEnd();
        responseString = AsrResponse.CreateFromJSON(responseString).result[0];
        Debug.Log(responseString);
        text.text += "我：" + responseString + "\n";


        //************图灵api得到回答
        string url = "http://www.tuling123.com/openapi/api";
        string key = "7c664d28fa0b472ab9833c2679c431f5";
        string postDataStr = "key=" + key + "&info=" + responseString;
        string result = HttpGet(url, postDataStr);
        JsonObject newObj = new JsonObject(result);
        string info = newObj["text"].Value.ToString();
        Debug.Log(info);
        text.text += "我：" + info + "\n";

        bwriter.Write(info);
        bwriter.Flush();


        ////**************合成语音不支持linux和window只支持移动端，据说mp3版权问题，无法直接在window上播放，也就是无法从网上或者文件里读取播放。场景里的应该被转编码了。
        //string url_speaker = "http://tsn.baidu.com/text2audio";
        //string postDataStr_speaker = "tex=" + info + "&lan=zh&cuid=B8-81-98-41-3E-E9&ctp=1&tok=24.d1ba8c1f1efa8a3de68678e5404d55a4.2592000.1523629153.282335-10681472&ctp=1&cuid=10681472";
        //string req = url_speaker + "?" + postDataStr_speaker;
        //WWW www = new WWW(req);  // start a download of the given URL
        //audioSource.clip = www.GetAudioClip(true, false, AudioType.MPEG); // 2D, streaming
        //audioSource.Play();
        //text.text += "over";


        ////**************合成语音
        //string url2 = "http://tsn.baidu.com/text2audio";
        //byte[] buffer2 = null;
        //string postDataStr2 = "tex=" + info + "&lan=zh&cuid=B8-81-98-41-3E-E9&ctp=1&tok=24.d1ba8c1f1efa8a3de68678e5404d55a4.2592000.1523629153.282335-10681472";

        //HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url2 + "?" + postDataStr2);
        //request2.Method = "GET";
        //request2.ContentType = "text/html;charset=UTF-8";
        //HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();

        //using (Stream stream = response2.GetResponseStream())
        //{
        //    //buffer2 = new byte[stream.Length];  ////////////////报错*******无法取得响应流，应该用什么先盛放一下

        //    // long length = request2.ContentLength;
        //    //buffer2 = new byte[length];///数字溢出
        //    // stream.Read(buffer2, 0, (int)length);
        //    //BinaryReader br = new BinaryReader(stream);
        //    //    buffer2 = br.ReadBytes((int)stream.Length);

        //        byte[] buffer3 = new byte[16*1096];
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            int count = 0;
        //            do
        //            {
        //                count = stream.Read(buffer3, 0, buffer3.Length);
        //                memoryStream.Write(buffer3, 0, count);

        //            } while (count != 0);

        //            buffer2 = memoryStream.ToArray();

        //        }



        //}
        //audioSource.clip = FromMp3Data(buffer2);
        //audioSource.Play();
    }
    /// <summary>
    /// 将mp3格式的字节数组转换为audioclip
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>

    //public static AudioClip FromMp3Data(byte[] data)
    //{
    //    // Load the data into a stream  
    //    MemoryStream mp3stream = new MemoryStream(data);
    //    // Convert the data in the stream to WAV format  
    //    Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);

    //    WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
    //    // Convert to WAV data  
    //    Wav wav = new Wav(AudioMemStream(waveStream).ToArray());

    //    AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false);
    //    audioClip.SetData(wav.LeftChannel, 0);
    //    // Return the clip  
    //    return audioClip;
    //}

    //private static MemoryStream AudioMemStream(WaveStream waveStream)
    //{
    //    MemoryStream outputStream = new MemoryStream();
    //    using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat))
    //    {
    //        byte[] bytes = new byte[waveStream.Length];
    //        waveStream.Position = 0;
    //        waveStream.Read(bytes, 0, Convert.ToInt32(waveStream.Length));
    //        waveFileWriter.Write(bytes, 0, bytes.Length);
    //        waveFileWriter.Flush();
    //    }
    //    return outputStream;
    //}
    //**********audioClip格式转成字节流
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
    ////************把mp3转换成audioclip
    //public static AudioClip FromMp3Data(byte[] data)
    //{
    //    // Load the data into a stream  
    //    MemoryStream mp3stream = new MemoryStream(data);
    //    // Convert the data in the stream to WAV format  
    //    Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);

    //    WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
    //    // Convert to WAV data  
    //    Wav wav = new Wav(AudioMemStream(waveStream).ToArray());

    //    AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false);
    //    audioClip.SetData(wav.LeftChannel, 0);
    //    // Return the clip  
    //    return audioClip;
    //}

    //用于http get请求
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
    public void PlayRecordAudio()
    {
        Microphone.End(null);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    public void EndPlayRecordAudio()
    {
        Microphone.End(null);
        audioSource.Stop();
    }




    /// <summary>
    /// 把录音转换为Byte[]
    /// </summary>
    /// <returns></returns>
    //public byte[] GetClipData()
    //{
    //    if (audioClip == null)
    //    {
    //        //Debug.LogError("录音数据为空");
    //        Debug.Log("录音数据为空");
    //        return null;
    //    }

    //    float[] samples = new float[audioClip.samples];

    //    audioClip.GetData(samples, 0);



    //    byte[] outData = new byte[samples.Length * 2];

    //    int rescaleFactor = 32767; //to convert float to Int16   

    //    for (int i = 0; i < samples.Length; i++)
    //    {
    //        short temshort = (short)(samples[i] * rescaleFactor);

    //        byte[] temdata = System.BitConverter.GetBytes(temshort);

    //        outData[i * 2] = temdata[0];
    //        outData[i * 2 + 1] = temdata[1];
    //    }
    //    if (outData == null || outData.Length <= 0)
    //    {
    //        //Debug.LogError("录音数据为空");
    //        Debug.Log("录音数据为空");
    //        return null;
    //    }

    //    //return SubByte(outData, 0, audioLength * 8000 * 2);
    //    return outData;
    //}
}
