using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChange : MonoBehaviour {
    public TransformFunctions TF;
	// Use this for initialization
	void Start () {
        TF = this.GetComponent<TransformFunctions>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangePos(int id)
    {
        TF.TargetObject = GameObject.Find("Target" + id.ToString());
        TF.startHitChange();
    }
}
