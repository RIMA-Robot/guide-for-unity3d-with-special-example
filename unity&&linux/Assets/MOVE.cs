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
    void Start () {
      
  
	}
	
	// Update is called once per frame
	void Update () {
        audioSource = GameObject.Find("Canvas/Audio Source").GetComponent<AudioSource>();
        if (Input.GetKey(KeyCode.W))
        {
            text = GameObject.Find("Canvas/Text").GetComponent<Text>();
            input = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>(); ;
            text.text +="我："+ input.text+"\n";           
            string url = "http://www.tuling123.com/openapi/api";
            string key = "7c664d28fa0b472ab9833c2679c431f5";
            string postDataStr = "key=" + key + "&info=" + input.text;
            string result = HttpGet(url, postDataStr);
            JsonObject newObj = new JsonObject(result);
            string info = newObj["text"].Value.ToString();
            text.text += "rima:" + info + "\n";
            print(info);

        }
          
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
}
