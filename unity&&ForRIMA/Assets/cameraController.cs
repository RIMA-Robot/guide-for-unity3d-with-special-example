using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    public Transform posIdle, posHear,posMainCamera;
	// Use this for initialization
	void Start () {
        posIdle = GameObject.Find("Target1").GetComponent<Transform>();
        posHear = GameObject.Find("Target2").GetComponent<Transform>();
        posMainCamera = GetComponent<Transform>();
        this.GetComponent<Transform>().position = posIdle.position;
        this.GetComponent<Transform>().rotation = posIdle.rotation;

    }

    // Update is called once per frame
    void Update () {
        
    }

    public void Hear()
    {
        Debug.Log("Hear");
        posMainCamera = posHear;
    }

}
