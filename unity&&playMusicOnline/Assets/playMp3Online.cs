using UnityEngine;
using System.Collections;

public class playMp3Online : MonoBehaviour
{
    public string url= "http://tsn.baidu.com/text2audio?tex=你好&lan=zh&cuid=B8-81-98-41-3E-E9&ctp=1&tok=24.d1ba8c1f1efa8a3de68678e5404d55a4.2592000.1523629153.282335-10681472&ctp=1&cuid=10681472";
    public AudioSource source;

    IEnumerator Start()
    {
        source = GameObject.Find("Canvas/Audio Source").GetComponent<AudioSource>();
        using (var www = new WWW(url))
        {
            yield return www;
            source.clip = www.GetAudioClip(true, false, AudioType.MPEG); ;
        }
    }

    void Update()
    {
        if (!source.isPlaying )
            source.Play();
    }
}