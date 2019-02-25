using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SelfRotate : MonoBehaviour {

    public Vector3 localAngularVel;
    
	void Start () {
	}
	
	void Update () {
        transform.Rotate(localAngularVel, localAngularVel.magnitude * Time.deltaTime,Space.Self);
	}
}
