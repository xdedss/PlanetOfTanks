using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelForceControl : MonoBehaviour {

    public float power;
    public float maxTorque;
    public float breakTorque;
    public WheelCollider[] wheels;

    Vector3 lastPos;

    private void Start()
    {

    }

    private void Update()
    {
        //Debug.Log(wheels[1].rpm * wheels[1].radius * 2 * Mathf.PI / 60f * 3.6);
        //Debug.Log(wheels[1].isGrounded);
        //Debug.Log(wheels[1].bounds);
    }

    private void FixedUpdate()
    {
        if (lastPos == Vector3.zero)
        {
            lastPos = transform.position;
        }
        //Vector3 dPos = transform.position - lastPos;
        //float vel = Mathf.Abs(Vector3.Dot(transform.forward, dPos) / Time.fixedDeltaTime);
        lastPos = transform.position;
        foreach (WheelCollider wc in wheels)
        {
            float wheelVel = Mathf.Abs(wc.rpm / 60) * wc.radius * 2 * Mathf.PI;
            wc.motorTorque = -Mathf.Clamp(power / Mathf.Max(wheelVel, 0.0001f), -maxTorque, maxTorque) / wheels.Length;
            //Debug.Log(wc.motorTorque);
            if (power == 0 && Input.GetKey(KeyCode.Space))
            {
                wc.brakeTorque = breakTorque / wheels.Length;
            }
            else
            {
                wc.brakeTorque = 0;
            }
        }
    }
}
