using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTank : MonoBehaviour
{

    public static Vector3 SamplePath01(Vector3 start, Vector3 end, float t)
    {
        t = Mathf.Clamp01(t);
        float startH = start.magnitude;
        float endH = end.magnitude;
        Vector3 up = Vector3.Cross(end, start);
        Quaternion startRot = Quaternion.LookRotation(start, up);
        Quaternion endRot = Quaternion.LookRotation(end, up);
        float tRot = Mathf.Sqrt(t);
        float tH = t * t;
        float H = Mathf.Lerp(startH, endH, tH);
        Quaternion Rot = Quaternion.Lerp(startRot, endRot, tRot);
        return Rot * Vector3.forward * H;
    }

    IEnumerator Drop(GameObject tank, Vector3 start, Vector3 end, float totalTime)
    {
        var sc = Instantiate(skyCrane).GetComponent<SkyCraneControl>();
        var scfollow = sc.gameObject.AddComponent<Follow>();
        scfollow.target = tank.transform;
        scfollow.lateUpdate = true;
        scfollow.fixedUpdate = true;
        scfollow.translation = true;
        scfollow.rotation = true;
        sc.setTeam(team);
        sc.StartParticleIdle();
        var rigid = tank.GetComponent<Rigidbody>();
        var tc = tank.GetComponent<TankControl>();
        tc.controllable = false;
        rigid.isKinematic = true;
        var lastPos = start;
        var vel = Vector3.zero;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / totalTime)
        {
            //Debug.Log(t);
            var currentPos = SamplePath01(start, end, t);
            vel = (currentPos - lastPos) / Time.fixedDeltaTime;
            var worldDown = RelativeSystemControl.DownAt(tank.transform.position);
            var lookAtAxis = Vector3.Cross(vel.normalized, worldDown);
            var whereIShouldLookAt = Quaternion.Lerp(Quaternion.LookRotation(vel.normalized, lookAtAxis), Quaternion.LookRotation(worldDown, lookAtAxis), t * t * t / 1.5f) * Vector3.forward;
            tank.transform.LookAt(whereIShouldLookAt, -worldDown, Vector3.down, Vector3.back);
            rigid.velocity = vel;
            rigid.angularVelocity = Vector3.zero;
            tank.transform.position = currentPos;
            lastPos = currentPos;
            yield return new WaitForFixedUpdate();
        }
        rigid.isKinematic = false;
        rigid.velocity = vel;
        tc.cannon.GetComponent<Rigidbody>().velocity = vel;
        tc.turrent.GetComponent<Rigidbody>().velocity = vel;
        sc.StartParticle();
        yield return Land(tank);
        //sc.EndParticle();
        scfollow.enabled = false;
        sc.gameObject.AddComponent<Rigidbody>();
        sc.gameObject.AddComponent<ConstantForce>().force = sc.transform.up * 40;
        sc.gameObject.AddComponent<SelfDestruction>().life = 8;
        tc.controllable = true;
    }

    IEnumerator Land(GameObject tank)
    {
        var tc = tank.GetComponent<TankControl>();
        var rigid = tank.GetComponent<Rigidbody>();
        var cannon = tc.cannon.GetComponent<Rigidbody>();
        var turrent = tc.turrent.GetComponent<Rigidbody>();
        var vel = rigid.velocity;
        Ray ray = new Ray(tank.transform.position, vel);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 50, 1 << 10))
        {
            var height = Mathf.Max(hit.distance - 2, 1);
            var acc = vel.sqrMagnitude / (2 * height);
            var time = vel.magnitude / acc;
            for(float t = 0; t < time; t += Time.fixedDeltaTime)
            {
                var velDir = rigid.velocity.normalized;
                var a = -acc * velDir - RelativeSystemControl.DownAt(tank.transform.position) * CenterGravity.g;
                rigid.angularVelocity = Vector3.zero;
                rigid.AddForce(a, ForceMode.Acceleration);
                turrent.AddForce(a, ForceMode.Acceleration);
                cannon.AddForce(a, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public GameObject skyCrane;
    public GameObject target;
    public bool ai;
    public int team;
    public float delayTime;
    public float time;
    public Transform DropStart;

    float timer;
    bool dropped = false;

    void Start()
    {
        timer = delayTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0 && !dropped)
        {
            dropped = true;
            var t = CopyOfTank.CopyTank(target, transform.position, transform.rotation, gameObject.name, ai, team);
            StartCoroutine(Drop(t, DropStart.position, transform.position, time));
        }
    }
}