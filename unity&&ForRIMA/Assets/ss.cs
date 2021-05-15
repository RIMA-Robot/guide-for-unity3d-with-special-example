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
    string ip = "127.0.0.1";
    public bodyController CF;
    public float m_speed = 5f;
    private static Socket clientSocket;
        private static byte[] result = new byte[1024];
    // Use this for initialization
    IPAddress mIp;
    IPEndPoint ip_end_point;
    void Start()
    {
        CF = GameObject.Find("RimaVirtualBody/rima").GetComponent<bodyController>();

        text = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>();
      
   ;
        ////client = new TcpClient("172.20.20.241", 8140);
        // client = new TcpClient("127.0.0.1", 8139);
        //  netstream = client.GetStream();
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //IPAddress mIp = IPAddress.Parse("172.20.20.241");
        //IPEndPoint ip_end_point = new IPEndPoint(mIp, 8140);


    }
    private void ReceiveData()
    {
        while (true)
        {
            int length = clientSocket.Receive(result);
            //string receiveSting = null;
            abc = Encoding.UTF8.GetString(result, 0, length);



            Debug.Log(abc);
        }
    }

    public void startSocket()

    {
        ip = text.text;
        Debug.Log(ip);

        mIp = IPAddress.Parse(ip.ToString());
       
        ip_end_point = new IPEndPoint(mIp, 8140);

        clientSocket.Connect(ip_end_point);

        //breader = new BinaryReader(netstream);
        // bwriter = new BinaryWriter(netstream);


        Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
        threadReceive.IsBackground = false;
        threadReceive.Start();

        //text = GameObject.Find("Canvas/Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        if(abc!=abc0)
        {
        if (abc == "聆听") CF.hear();
         if (abc == "跳舞") CF.dance();
        if (abc == "谈话") CF.talk();
         if(abc=="立正") CF.idle();

            abc0 = abc;
        }
        else
        {
         
        }
    }
}
