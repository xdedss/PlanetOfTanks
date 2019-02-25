using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurrentMotor : MonoBehaviour {

    public bool controllable = true;
    [Space]
    public Vector3 worldTarget;
    public Vector3 localPointer;
    
    HingeJoint joint;
    Vector3 localTarget;
    
	void Start () {
        joint = GetComponent<HingeJoint>();
        joint.axis = joint.axis.normalized;
	}
	
	void Update () {

	}

    public void Explode()
    {
        GetComponent<Rigidbody>().mass /= 10;
        if (joint)
        {
            joint.breakForce = 0;
            transform.parent = null;
        }
        controllable = false;
    }

    private void FixedUpdate()
    {
        if (joint)
        {
            if (controllable)
            {
                joint.useMotor = true;
                localTarget = transform.worldToLocalMatrix * new Vector4(worldTarget.x, worldTarget.y, worldTarget.z, 1);
                //localTarget -= joint.axis * Vector3.Dot(joint.axis, localPointer);
                //localTarget.Normalize();
                //localPointer -= joint.axis * Vector3.Dot(joint.axis, localPointer);
                //localPointer.Normalize();
                localTarget = Vector3.ProjectOnPlane(localTarget, joint.axis);
                localPointer = Vector3.ProjectOnPlane(localPointer, joint.axis);
                float deltaAngle = Vector3.SignedAngle(localPointer, localTarget, joint.axis);

                var a = joint.motor;
                a.targetVelocity = Mathf.Clamp(deltaAngle * 3, -60, 60);
                joint.motor = a;
            }
            else
            {
                //joint.useMotor = false;
                var a = joint.motor;
                a.targetVelocity = 0;
                joint.motor = a;
            }
        }
        //Debug.Log(deltaAngle);
    }
}
