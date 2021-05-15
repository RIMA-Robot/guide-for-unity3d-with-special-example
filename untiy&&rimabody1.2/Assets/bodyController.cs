using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bodyController : MonoBehaviour {
    public Animator anim;
    public TargetChange tc;
    public Camera mCamera;
    public bool lookHead = false;
    public Vector3 headPos;
    public Texture[] mousetexture;
    public GameObject Head;
    public Text text;
    public TransformFunctions CF;
    // Use this for initialization
    void Start () {
        CF = GameObject.Find("RimaVirtualBody/rima").GetComponent<TransformFunctions>();
        text = GameObject.Find("Canvas/Model/chatapi").GetComponent<Text>();
        anim = this.GetComponent<Animator>();
        tc = this.GetComponentInChildren<TargetChange>();
        mCamera = Camera.main;
        headPos = GameObject.Find("headPos").GetComponent<Transform>().position;
        Head = GameObject.Find("C_man_Head");
        idle();
	}
	
	// Update is called once per frame
	void Update () {
        if(lookHead)
        {
            headPos = GameObject.Find("headPos").GetComponent<Transform>().position;

            mCamera.transform.LookAt(headPos);
        }
		
	}

    public void talk()
    {
        changeTexture(1);
        //idle();
        anim.SetBool("talk", true);
    }

    public void dance()
    {
        tc.ChangePos(3);
        anim.SetBool("dance", true);
        lookHead = true;
    }

    public void idle()
    {
        changeTexture(0);
        tc.ChangePos(1);
        anim.SetBool("dance", false);
        anim.SetBool("talk", false);
        lookHead = false;
    }

    public void hear()
    {
        lookHead = false;
        anim.SetBool("dance", false);
        
        tc.ChangePos(2);
        tc.ChangePos(2);

        lookHead = false;
    }

    public void changeTexture(int i)
    {
        //Head.GetComponent<SkinnedMeshRenderer>().material.mainTexture = mousetexture[i];
        Material mouseTexs= Head.GetComponent<Renderer>().materials[1];
        mouseTexs.mainTexture = mousetexture[i];

    }
    public void changeModel()
    {if (text.text == "RIMA混合模式")
            text.text = "图灵模式";
        else text.text = "RIMA混合模式";
    }
}
