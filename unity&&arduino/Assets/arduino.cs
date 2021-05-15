
using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class arduino : MonoBehaviour {
    SerialPort stream = new SerialPort("COM3", 9600); //Set the port (com4) and the baud rate (9600, is standard on most devices)
                                                      // Use this for initialization
    void Start () {
        stream.Open(); //Open the Serial Stream.
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
        {
            stream.Write("1");

        }
        string value = stream.ReadLine(); //Read the information
        print(value);
        stream.BaseStream.Flush(); //Clear the serial information so we assure we get new information.
    
           
    }
}
