using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPhysics : MonoBehaviour
{

    public float power;
    public float brake;
    public float mass = 100;
    //public float speed;
    [Space]
    public float maxForce;
    [Space]
    public float forwardValue;
    public float exSlip;
    public float exValue;
    public float asSlip;
    public float asValue;
    public float angularFriction;
    public Rigidbody target;
    public WheelPhysics[] wheels;

    public void Explode()
    {
        foreach(var wheel in wheels)
        {
            wheel.enabled = false;
        }
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {
        bool areOnGround = false;
        float totalForce = 0;
        foreach(var wheel in wheels)
        {
            areOnGround |= wheel.isOnGround;
            totalForce -= wheel.k * wheel.compress;
        }
        Vector3 vel = target.GetPointVelocity(transform.position);
        SideFriction(vel, totalForce);
        ForwardFriction(vel, totalForce, power, brake);
    }

    private void ForwardFriction(Vector3 vel, float F, float power, float brake)
    {
        float fVel = Vector3.Dot(vel, transform.forward);
        float T = Mathf.Sign(power) * Mathf.Min(Mathf.Abs(power) / Mathf.Max(Mathf.Abs(fVel), 0.0001f), maxForce);
        //float fdVel = fVel - speed;
        //float poweredVel =  speed + T / mass * Time.fixedDeltaTime;
        //float powereddVel = fVel - poweredVel;
        //float miu = GetFrictionFactor(fdVel, exSlip, exValue, asSlip, asValue);
        float miu = forwardValue;
        float maxFriction = miu * F;
        float force = Mathf.Clamp(T - Mathf.Sign(fVel) *  brake, -maxFriction, maxFriction);
        if(force * T < 0)
        {
            float maxbrake = Mathf.Abs(fVel) / Time.fixedDeltaTime * target.mass;
            force = Mathf.Clamp(force, -maxbrake, maxbrake);
        }

        target.AddForceAtPosition(force * transform.forward, transform.position);
    }

    //private void Power(Vector3 vel, float power, float F)
    //{
    //    float miu = GetFrictionFactor(fdVel, exSlip, exValue, asSlip, asValue);
    //    float maxFriction = miu * F;
    //    AddTrackForce(0, maxFriction);
    //    ForwardFriction(fdVel, maxFriction);
    //}

    //private void AddTrackForce(float force, float maxFriction)
    //{
    //    float directOnBody = Mathf.Clamp(force, -maxFriction, maxFriction);
    //    float onTrack = force - directOnBody;
    //    target.AddForceAtPosition(transform.forward * directOnBody, transform.position);
    //    speed += (directOnBody / target.mass + onTrack / mass) * Time.fixedDeltaTime;
    //}

    private void SideFriction(Vector3 vel, float F)
    {
        float sideVel = Vector3.Dot(vel, transform.right);
        float sideMiu = GetFrictionFactor(sideVel, exSlip, exValue, asSlip, asValue);
        Vector3 force = Mathf.Sign(-sideVel) * Mathf.Min(sideMiu * F, Mathf.Abs(sideVel) / Time.fixedDeltaTime * target.mass) * transform.right;
        float yRotVel = Vector3.Dot(target.angularVelocity, transform.up);
        Vector3 torque = Mathf.Sign(-yRotVel) * Mathf.Min(sideMiu * F * angularFriction * Mathf.Max(1, Mathf.Abs(yRotVel)), Mathf.Abs(yRotVel) / Time.fixedDeltaTime * target.mass) * transform.up;
        target.AddForce(force);
        target.AddTorque(torque);
    }

    private float GetFrictionFactor(float vel, float exSlip, float exValue, float asSlip, float asValue)
    {
        vel = Mathf.Abs(vel);
        if (vel < exSlip)
        {
            return exValue;
        }
        else
        {
            return Mathf.Exp((exSlip - vel) / asSlip) * (exValue - asValue) + asValue;
        }
    }
}
