using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFunctions : MonoBehaviour {

    public GameObject TargetObject;
    public bool startHit=false;
	// Use this for initialization
	void Start () {
        //TargetObject = GameObject.FindGameObjectWithTag("originPos");
        TargetObject = this.gameObject;
    }

    // Update is called once per frame
    public float rotSpeed = 5f,MoveSpeed=5f;
	void Update () {

        MoveToTargetLerp(this.transform, TargetObject.transform,startHit);
        RotateToTarget(TargetObject.transform,rotSpeed);
    }

    public static void MoveToTarget(Transform ta, Transform tb, bool start)
    {
        if (start)
        {
            if (Vector3.Distance(ta.position, tb.position) > 1)
            {
                Vector3 direction = tb.position - ta.position;
                direction = direction.normalized;
                ta.Translate(direction * Time.deltaTime);
            }
        }
    }


    public float currentLerpTime = 0;
    public float Lerptime = 5;

    public void MoveToTargetLerp( Transform ta, Transform tb, bool start)
    {
        
        float ltime = 0;

        if(start)
        {
            ltime += Time.deltaTime*MoveSpeed;
            ta.transform.position = Vector3.Lerp(ta.position, tb.position,ltime);
            //currentLerpTime = 0;
            ltime = 0;
        }
        if(Vector3.Distance(ta.position,tb.position)<=0.01)
        {
            startHit = false;
        }
    }

    public void startHitChange()
    {
        startHit = !startHit;
    }

    public void ChangePos(int id)
    {
        Debug.Log("change Pos to target" + id.ToString());
        if (id > 0)
        {
            TargetObject = GameObject.FindGameObjectWithTag("Target" + id.ToString());
        }
        else
            TargetObject = GameObject.FindGameObjectWithTag("originPos");


        startHitChange();
    }

    public void RotateToTarget(Transform target,float speed)
    {
        // Vector3 deriction = target.position-transform.position;
        Vector3 deriction = target.rotation.eulerAngles;
        //Quaternion rotation = Quaternion.LookRotation(target.position);
        Quaternion rotation = target.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
   

   

}
