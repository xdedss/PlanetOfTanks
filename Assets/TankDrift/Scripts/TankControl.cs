using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour
{
    public bool controllable;
    [Space]
    public TrackPhysics leftTrack;
    public TrackPhysics rightTrack;
    public TurrentMotor turrent;
    public TurrentMotor cannon;
    public GameObject shell;
    public Transform cannonMark;
    public Transform frontMark;
    [Space]
    public Transform cornerLFU;
    public Transform cornerRBD;
    [Space]
    Rigidbody rigid;

    public Vector3 turrentTarget;
    public bool leftBrake;
    public bool rightBrake;
    public float leftMove = 0;
    public float rightMove = 0;
    [Space]
    public float maxBrake = 400000;
    public float maxPower = 200000;
    public float reloadTime = 7;
    [Space]
    public float reloadTimer = 0;
    [Space]
    public bool isDestroyed;
    [Space]
    public bool forwardObstacle;
    public bool turnRightObstacle;
    public bool turnLeftObstacle;
    public bool backwardObstacle;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        Singleton<TankManager>.instance.Register(this);
        StartCoroutine(FindObstacleLoop());
    }

    public Vector3 GetWorldPoint(float LRt, float DUt, float BFt)
    {
        Vector3 LFU = cornerLFU.localPosition;
        Vector3 RBD = cornerRBD.localPosition;
        var mat = cornerRBD.parent.localToWorldMatrix;
        return (mat * new Vector3(Mathf.Lerp(LFU.x, RBD.x, LRt), Mathf.Lerp(RBD.y, LFU.y, DUt), Mathf.Lerp(RBD.z, LFU.z, BFt)).V4(1)).V3();
    }

    public Vector3 CalculateAim(Vector3 target)
    {
        var down = RelativeSystemControl.DownAt(transform.position);
        var planearTarget = Vector3.ProjectOnPlane(target - transform.position, down);
        float distance = planearTarget.magnitude;
        var shellControl = shell.GetComponent<ShellControl>();
        float speed = shellControl.speed;
        float g = CenterGravity.g * shell.GetComponent<CenterGravity>().multiplier;
        float t = distance / speed;
        float h = g * t * t / 2;
        Debug.Log(t);
        return target - down * h;
    }

    public bool Predict(float stepT, int maxIterations, out TankControl tc, out Vector3 endPoint)
    {
        float speed = shell.GetComponent<ShellControl>().speed;
        float gravity = shell.GetComponent<CenterGravity>().multiplier * CenterGravity.g;
        var startPoint = cannonMark.position;
        var velocity = cannonMark.forward * speed;
        for(int i = 0; i < maxIterations; i++)
        {
            Vector3 est = startPoint + stepT * velocity;
            Vector3 averageG = (RelativeSystemControl.DownAt(startPoint) + RelativeSystemControl.DownAt(est)).normalized * gravity;
            Vector3 estDescend = averageG * stepT * stepT / 2f;
            Vector3 estDv = averageG * stepT;
            est += estDescend;
            Ray ray = new Ray(startPoint, est - startPoint);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, (est-startPoint).magnitude, (1 << 10) + (1 << 13)))
            {
                Debug.DrawLine(startPoint, hit.point, Color.cyan);
                if(hit.collider.gameObject.layer == 10)
                {//terrain
                    tc = null;
                    endPoint = hit.point;
                    return false;
                }
                else if(hit.collider.gameObject.layer == 13)
                {//tank
                    tc = FindTankControl(hit.collider.transform);
                    endPoint = hit.point;
                    return true;
                }
            }
            else
            {
                Debug.DrawLine(startPoint, est);
                startPoint = est;
                velocity += estDv;
            }
        }
        tc = null;
        endPoint = startPoint;
        return false;
    }

    public bool TryFire()
    {
        if (reloadTimer < 0 && controllable)
        {
            Fire();
            return true;
        }
        return false;
    }

    private void Fire()
    {
        reloadTimer = reloadTime;
        var s = Instantiate(shell);
        var s_rigid = s.GetComponent<Rigidbody>();
        if (s_rigid)
        {
            s_rigid.velocity = cannon.GetComponent<Rigidbody>().GetPointVelocity(cannonMark.position);
        }
        s.transform.parent = null;
        s.transform.position = cannonMark.position;
        s.transform.rotation = cannonMark.rotation;
        s.SetActive(true);
    }


    void Update()
    {
        reloadTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (controllable)
        {
            turrent.controllable = true;
            cannon.controllable = true;
            turrent.worldTarget = turrentTarget;
            cannon.worldTarget = turrentTarget;
            leftTrack.power = maxPower * leftMove;
            rightTrack.power = maxPower * rightMove;
            leftTrack.brake = leftBrake ? maxBrake : 0;
            rightTrack.brake = rightBrake ? maxBrake : 0;
        }
        else
        {
            turrent.controllable = false;
            cannon.controllable = false;
            leftTrack.power = 0;
            rightTrack.power = 0;
            leftTrack.brake = maxBrake / 5;
            rightTrack.brake = maxBrake / 5;
        }
    }

    public void Explode()
    {
        rigid.mass /= 10;
        leftTrack.Explode();
        rightTrack.Explode();
        turrent.Explode();
        //cannon.Explode();
        turrent.GetComponent<Rigidbody>().velocity += turrent.GetComponent<HingeJoint>().axis * 50;

        GetComponent<TeamMark>().DisableMapMark();
    }

    IEnumerator FindObstacleLoop()
    {
        while (true)
        {
            //new GameObject("WTF").transform.position = tc.GetWorldPoint(1, 1, 1);
            //UnityEditor.EditorApplication.isPaused = true;
            //forward
            yield return new WaitForSeconds(0.5f);
            var rayFR = new Ray(GetWorldPoint(1, 0.5f, 1), frontMark.forward);
            var rayFM = new Ray(GetWorldPoint(0.5f, 0.1f, 1), frontMark.forward);
            var rayFL = new Ray(GetWorldPoint(0, 0.5f, 1), frontMark.forward);
            forwardObstacle = Physics.Raycast(rayFR, 1) || Physics.Raycast(rayFL, 1) || Physics.Raycast(rayFM, 0.3f);

            //backward
            yield return new WaitForSeconds(0.5f);
            var rayBR = new Ray(GetWorldPoint(1, 0.5f, 0), -frontMark.forward);
            var rayBL = new Ray(GetWorldPoint(0, 0.5f, 0), -frontMark.forward);
            backwardObstacle = Physics.Raycast(rayBR, 1) || Physics.Raycast(rayBL, 1);

            //turn
            yield return new WaitForSeconds(0.5f);
            //Debug.DrawRay(GetWorldPoint(1, 0.5f, 0.95f), frontMark.right, Color.magenta, 1);
            var rayRF = new Ray(GetWorldPoint(1, 0.5f, 0.95f), frontMark.right);
            var rayRM = new Ray(GetWorldPoint(1, 0.5f, 0.50f), frontMark.right);
            var rayRB = new Ray(GetWorldPoint(1, 0.5f, 0.05f), frontMark.right);
            var rayLF = new Ray(GetWorldPoint(0, 0.5f, 0.95f), -frontMark.right);
            var rayLM = new Ray(GetWorldPoint(0, 0.5f, 0.50f), -frontMark.right);
            var rayLB = new Ray(GetWorldPoint(0, 0.5f, 0.05f), -frontMark.right);
            var middleObstacle = Physics.Raycast(rayLM, 0.5f) || Physics.Raycast(rayRM, 0.5f);
            turnLeftObstacle = Physics.Raycast(rayLF, 1) || Physics.Raycast(rayRB, 1) || middleObstacle;
            turnRightObstacle = Physics.Raycast(rayRF, 1) || Physics.Raycast(rayLB, 1) || middleObstacle;
        }
    }

    public static TankControl FindTankControl(Transform child)
    {
        TankControl tc = child.GetComponent<TankControl>();
        if (tc)
        {
            return tc;
        }
        else
        {
            if (child.parent)
            {
                return FindTankControl(child.parent);
            }
            else
            {
                return null;
            }
        }
    }
}
