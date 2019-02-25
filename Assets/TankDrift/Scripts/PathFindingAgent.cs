using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;


public class PathFindingAgent : MonoBehaviour
{
    //public Transform target;
    public Vector3 target;

    private Seeker seeker;
    private FunnelModifier funnel;
    public Path path;

    float findInterval = 3;
    float findTimer;

    //public float nextWaypointDistance = 3;
    //private int currentWaypoint = 0;

    public void UpdateTarget(Vector3 newTarget)
    {
        if(newTarget != target)
        {
            if ((newTarget - target).magnitude > 1f && findTimer < 0)
            {
                findTimer = findInterval;
                target = newTarget;
                seeker.StartPath(transform.position, target, OnPathComplete);
            }
        }
    }

    public Vector3 GetVelocityDir(Vector3 myPoint)
    {
        if (path != null)
        {
            Vector3 nearestDir = Vector3.zero;
            float nearestDist = 10000;
            var waypoints = path.vectorPath;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Vector3 down = RelativeSystemControl.DownAt(waypoints[i + 1]);
                Vector3 myHeight = Vector3.Project((waypoints[i + 1] - myPoint), down);
                Vector3 myPointOnPlane = myPoint + myHeight;
                float distance = (waypoints[i] - myPointOnPlane).magnitude;
                if (distance < nearestDist)
                {
                    Vector3 dir = (waypoints[i + 1] - myPointOnPlane).normalized;
                    nearestDir = dir;
                }
            }
            return nearestDir;
        }
        return Vector3.zero;
    }
    
    private void Awake()
    {
        //if (!target)
        //{
        //    //target = new GameObject("target-" + gameObject.name).transform;
        //}
    }

    public void Start()
    {
        target = transform.position;
        seeker = GetComponent<Seeker>();
        funnel = GetComponent<FunnelModifier>();
        //StartCoroutine(findfindfind());
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Debug.Log(gameObject.name + "-- found path");
            path = p;
        }
        else
        {
            Debug.LogError(p.errorLog);
        }
    }

    void Update()
    {
        findTimer -= Time.deltaTime;
        if((target - transform.position).magnitude > 600)
        {
            funnel.enabled = false;
        }
        else
        {
            funnel.enabled = true;
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    seeker.StartPath(transform.position, target);
        //}
    }

    IEnumerator findfindfind()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            seeker.StartPath(transform.position, target, OnPathComplete);
        }
    }

    public void FixedUpdate()
    {
        //if (path == null)
        //{
        //    //We have no path to move after yet
        //    return;
        //}

        //if (currentWaypoint >= path.vectorPath.Count)
        //{
        //    Debug.Log("End Of Path Reached");
        //    return;
        //}

        ////Direction to the next waypoint
        //Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        //dir *= speed * Time.fixedDeltaTime;
        //controller.SimpleMove(dir);

        ////Check if we are close enough to the next waypoint
        ////If we are, proceed to follow the next waypoint
        //if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        //{
        //    currentWaypoint++;
        //    return;
        //}
    }
}