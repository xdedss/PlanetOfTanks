using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInputAI : MonoBehaviour
{

    public TankControl tc;
    public PathFindingAgent pf;
    public TankControl attackTarget;
    public PointControl captureTarget;
    public Rigidbody rigid;
    [Space]
    public Vector3 targetDir;
    public float targetVel;


    float refindTimer;

    void Start()
    {
        if (!tc)
        {
            tc = GetComponent<TankControl>();
        }
        if (!pf)
        {
            pf = GetComponent<PathFindingAgent>();
        }
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(FindTargetLoop());
    }

    void Update()
    {
        if (!tc.isDestroyed)
        {
            refindTimer -= Time.deltaTime;


            TankControl hitTC;
            Vector3 hitpoint;
            tc.Predict(0.5f, 4, out hitTC, out hitpoint);
            if (attackTarget && !attackTarget.isDestroyed)
            {
                pf.UpdateTarget(attackTarget.transform.position);
                tc.turrentTarget = tc.CalculateAim(attackTarget.transform.position);
                if (hitTC == attackTarget)
                {
                    tc.TryFire();
                }
                var distance = (attackTarget.transform.position - transform.position).magnitude;
                targetDir = pf.GetVelocityDir(transform.position);
                targetVel = Mathf.Clamp((distance - 30) / 1.5f, 0, 80 / 3.6f);
            }
            else if (captureTarget)
            {
                pf.UpdateTarget(captureTarget.transform.position);
                var distance = (captureTarget.transform.position - transform.position).magnitude;
                targetDir = pf.GetVelocityDir(transform.position);
                targetVel = Mathf.Clamp((distance - 5) / 1.5f, 0, 80 / 3.6f);
            }
            else
            {
                targetVel = 0;
            }
            MoveTowardVel(targetDir * targetVel);
        }
    }

    void MoveTowardVel(Vector3 vel)
    {
        vel = Vector3.ProjectOnPlane(vel, tc.frontMark.up);
        Debug.DrawLine(transform.position, transform.position + vel, Color.magenta);
        float mag = vel.magnitude;
        float angle = Vector3.SignedAngle(tc.frontMark.forward, vel, -RelativeSystemControl.DownAt(transform.position));
        if (mag > 1)
        {
            //if (rigid.velocity.magnitude < 5)
            //{
            //    //if (angle > 150 || angle < -150)
            //    //    Back();
            //    //else 
            //    //if (angle > 90)
            //    //    BackRight();
            //    //else if(angle < -90)
            //    //    BackLeft();
            //    //else if (angle < -10)
            //    //    Left();
            //    //else if (angle > 10)
            //    //    Right();
            //    //else
            //    //{
            //    //    if (Vector3.Dot(rigid.velocity, vel.normalized) > mag)
            //    //        Stop();
            //    //    else
            //    //        Move();
            //    //}
            //}
            //else
            //{
            //    if (angle < -10)
            //        Left();
            //    else if (angle > 10)
            //        Right();
            //    else
            //    {
            //        if (Vector3.Dot(rigid.velocity, vel.normalized) > mag)
            //            Stop();
            //        else
            //            Move();
            //    }
            //}

            if(angle < -10)
            {
                FlexLeft(0);
            }
            else if(angle > 10)
            {
                FlexRight(0);
            }
            else
            {
                if (Vector3.Dot(rigid.velocity, vel.normalized) > mag)
                    Stop();
                else
                    FlexMove(0);
            }
        }
        else
        {
            Stop();
        }
    }
    void FlexMove(ushort flags)//0F 1B 2L 3R
    {
        if (tc.forwardObstacle)
        {
            flags |= (1 << 0);
            if((flags & (1 << 2)) == 0)
                FlexLeft(flags);
            else if((flags & (1 << 3)) == 0)
                FlexRight(flags);
            else if((flags & (1 << 1)) == 0)
                FlexBack(flags);
            else
                Free();
        }
        else
            Move();
    }
    void FlexRight(ushort flags)//0F 1B 2L 3R
    {
        if (tc.turnRightObstacle)
        {
            flags |= (1 << 3);
            if ((flags & (1 << 0)) == 0)
                FlexMove(flags);
            else if ((flags & (1 << 1)) == 0)
                FlexBack(flags);
            else if ((flags & (1 << 2)) == 0)
                FlexLeft(flags);
            else
                Free();
        }
        else
        {
            if (tc.forwardObstacle)
                BackRight();
            else if (tc.backwardObstacle)
                Right();
            else
                BackRight();
        }
    }
    void FlexLeft(ushort flags)//0F 1B 2L 3R
    {
        if (tc.turnLeftObstacle)
        {
            flags |= (1 << 2);
            if ((flags & (1 << 0)) == 0)
                FlexMove(flags);
            else if ((flags & (1 << 1)) == 0)
                FlexBack(flags);
            else if ((flags & (1 << 3)) == 0)
                FlexRight(flags);
            else
                Free();
        }
        else
        {
            if (tc.forwardObstacle)
                BackLeft();
            else if (tc.backwardObstacle)
                Left();
            else
                BackLeft();
        }
    }
    void FlexBack(ushort flags)//0F 1B 2L 3R
    {
        if (tc.forwardObstacle)
        {
            flags |= (1 << 1);
            if ((flags & (1 << 2)) == 0)
                FlexLeft(flags);
            else if ((flags & (1 << 3)) == 0)
                FlexRight(flags);
            else if ((flags & (1 << 0)) == 0)
                FlexMove(flags);
            else
                Free();
        }
        else
            Back();
    }
    void Stop() { tc.leftBrake = true; tc.rightBrake = true; tc.leftMove = 0; tc.rightMove = 0; }
    void Move() { tc.leftBrake = false; tc.rightBrake = false; tc.leftMove = 1; tc.rightMove = 1; }
    void Back() { tc.leftBrake = false; tc.rightBrake = false; tc.leftMove = -1; tc.rightMove = -1; }
    void Left() { tc.leftBrake = true; tc.rightBrake = false; tc.leftMove = 0; tc.rightMove = 1; }
    void Right() { tc.leftBrake = false; tc.rightBrake = true; tc.leftMove = 1; tc.rightMove = 0; }
    void BackLeft() { tc.leftBrake = false; tc.rightBrake = true; tc.leftMove = -1; tc.rightMove = 0; }
    void BackRight() { tc.leftBrake = true; tc.rightBrake = false; tc.leftMove = 0; tc.rightMove = -1; }
    void Free() { tc.leftBrake = false; tc.rightBrake = false; tc.leftMove = 0; tc.rightMove = 0; }

    IEnumerator FindTargetLoop()
    {
        while (true)
        {
            yield return 0;
            if (!attackTarget || attackTarget.isDestroyed || !Visible(attackTarget) || refindTimer < 0)
            {
                refindTimer = 10;
                attackTarget = FindTankTarget();
                if (attackTarget == null)
                {
                    yield return new WaitForSeconds(2);

                    captureTarget = FindPointTarget();
                }
            }
        }
    }

    PointControl FindPointTarget()
    {
        float minDistance = 10000;
        PointControl min = null;
        foreach(var pointControl in PointControl.allPoints)
        {
            var occupyStat = pointControl.progress;
            if(occupyStat * GetComponent<TeamMark>().team < 0.999)
            {// is not ours
                var distance = (pointControl.transform.position - transform.position).magnitude;
                if(distance < minDistance)
                {
                    minDistance = distance;
                    min = pointControl;
                }
            }
        }
        return min;
    }

    TankControl FindTankTarget()
    {
        float minDistance = 10000;
        TankControl min = null;
        foreach(var tankControl in Singleton<TankManager>.instance.tanks)
        {
            var teamMark = tankControl.GetComponent<TeamMark>();
            if (teamMark.team != GetComponent<TeamMark>().team)
            {
                if (!tankControl.isDestroyed && Visible(tankControl))
                {
                    var distance = (tankControl.transform.position - transform.position).magnitude;
                    if(distance < minDistance)
                    {
                        minDistance = distance;
                        min = tankControl;
                    }
                }
            }
        }
        return min;
    }

    bool Visible(TankControl tankControl)
    {
        Ray ray = new Ray(transform.position, tankControl.transform.position - transform.position);
        if(Physics.Raycast(ray, (tankControl.transform.position - transform.position).magnitude, 1 << 10))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
