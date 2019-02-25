using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPhysics : MonoBehaviour
{

    public float k;
    public float damp;
    public float maxComp;
    public Rigidbody target;
    public bool isOnGround { get; private set; }
    public float compress { get; private set; }

    private float lastCompress = 0;
    //private Vector3 lastPos;

    void Start()
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        //if(lastPos == Vector3.zero)
        //{
        //    lastPos = transform.position;
        //}
        compress = GetCompress();
        float compressVel = (compress - lastCompress) / Time.fixedDeltaTime;
        //Vector3 vel = target.GetPointVelocity(transform.position);
        isOnGround = compress != 0;

        Bounce(compress, compressVel);
        //SideFriction(vel, -compress * k);

        lastCompress = compress;
        //lastPos = transform.position;
    }

    //private void SideFriction(Vector3 vel, float F)
    //{
    //    float sideVel = Vector3.Dot(vel, transform.right);
    //    float sideMiu = GetSideFrictionFactor(sideVel, 0.4f, 1.2f, 0.5f, 0.8f);
    //    Vector3 force = Mathf.Sign(-sideVel) * Mathf.Min(sideMiu * F, Mathf.Abs(sideVel) / Time.fixedDeltaTime * target.mass) * transform.right;
    //    target.AddForceAtPosition(force, transform.position);
    //}

    private void Bounce(float compress, float vel)
    {
        Vector3 force = -transform.up * (compress * k + vel * damp);
        target.AddForceAtPosition(force, transform.position);
    }

    //private float GetSideFrictionFactor(float sideVel, float exSlip, float exValue, float asSlip, float asValue)
    //{
    //    sideVel = Mathf.Abs(sideVel);
    //    if(sideVel < exSlip)
    //    {
    //        //return Mathf.Lerp(0, exValue, Mathf.InverseLerp(0, exSlip, sideVel));
    //        return exValue;
    //    }
    //    else
    //    {
    //        return Mathf.Exp((exSlip - sideVel) / asSlip) * (exValue - asValue) + asValue;
    //    }
    //}

    private float GetCompress()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * maxComp, -transform.up);
        if(Physics.Raycast(ray, out hit, maxComp, (1 << 10) + (1 << 0) + (1 << 13)))
        {
            return hit.distance - maxComp;
        }
        return 0;
    }
}
