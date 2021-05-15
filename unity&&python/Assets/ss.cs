using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ss : MonoBehaviour {
    public TcpClient client;
    public BinaryReader breader;
    public BinaryWriter bwriter;
    public NetworkStream netstream;
    string abc = "";
    string abc0 = "";
    Text text;

    public float m_speed = 5f;
    private static Socket clientSocket;
        private static byte[] result = new byte[1024];
    // Use this for initialization
    void Start () {

        ////client = new TcpClient("172.20.20.241", 8140);
        //client = new TcpClient("127.0.0.1", 8139);
        //netstream = client.GetStream();



        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //IPAddress mIp = IPAddress.Parse("172.20.20.241");
        //IPEndPoint ip_end_point = new IPEndPoint(mIp, 8140);
        IPAddress mIp = IPAddress.Parse("127.0.0.1");
        IPEndPoint ip_end_point = new IPEndPoint(mIp, 8139);

       

        //breader = new BinaryReader(netstream);
        //bwriter = new BinaryWriter(netstream);


        Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
        threadReceive.IsBackground = false;
        threadReceive.Start();

        text = GameObject.Find("Canvas/Text").GetComponent<Text>();
    }
    private void ReceiveData()
    {
        int length = clientSocket.Receive(result);

        abc = Encoding.UTF8.GetString(result, 0, length);

        //string receiveSting = null;
        //abc= breader.ReadString()+"//";
        Debug.Log(abc);

    }



    // Update is called once per frame
    void Update () {
        

        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow)) //前
        {
            this.transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
            //将输入的内容字符串转换为机器可以识别的字节数组     
         byte[] arrClientSendMsg = Encoding.UTF8.GetBytes("11111");
            //调用客户端套接字发送字节数组     
            clientSocket.Send(arrClientSendMsg);

        }
        if (abc!= abc0)
        {
            text.text += abc;
            abc0 = abc;
        }
    }
}
