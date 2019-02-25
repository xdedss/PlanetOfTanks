using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointControl : MonoBehaviour
{
    public static List<PointControl> allPoints = new List<PointControl>();
    [UnityEditor.MenuItem("Point/changemesh")]
    static void change()
    {
        var points = UnityEngine.Object.FindObjectsOfType<PointControl>();
        foreach (var point in points)
        {
            //var sph = point.transform.Find("Sphere").GetComponent<MeshRenderer>();
            //point.centerBall = sph;
            //sph.localScale = new Vector3(25, 25, 25);
            //sph.sharedMesh = point.changemesh;
            //var proj = point.transform.Find("projector");
            //proj.gameObject.SetActive(true);
            //proj.GetComponent<Projector>().enabled = false;

            var t = point.transform.Find("Mark(Clone)");
            DestroyImmediate(t.gameObject);

            t = Instantiate(point.Mark).transform;
            t.parent = point.transform;
            t.localPosition = Vector3.zero;
            point.mark = t.GetComponent<PointMarkControl>();
        }
    }

    [Range(-1, 1)]
    public float progress;//-1red +1blue

    public GameObject Mark;
    public float occupyTime = 10;
    public float radius = 20;
    public MeshRenderer centerBeam;
    public MeshRenderer centerBall;
    public PointMarkControl mark;
    public Material redMat;
    public Material blueMat;
    public Material neutralMat;
    public Transform beamRed;
    public Transform beamBlue;
    public Transform beamNeutral;

    OccupyStat occupy = OccupyStat.None;
    [SerializeField]
    OccupyStat occupied = OccupyStat.None;

    Projector projector;

    private void Awake()
    {
        allPoints.Add(this);
    }

    void Start()
    {
        StartCoroutine(CheckOccupy());
        projector = transform.Find("projector").GetComponent<Projector>();
        mark.mainColor = new Color(1, 1, 1, 1);
        //mark.material = new Material(mark.material);
    }

    void Update()
    {
        switch (occupy)
        {
            case OccupyStat.Blue:
                progress = Mathf.MoveTowards(progress, 1, Time.deltaTime / occupyTime);
                break;
            case OccupyStat.Red:
                progress = Mathf.MoveTowards(progress, -1, Time.deltaTime / occupyTime);
                break;
            case OccupyStat.Conflict:
                //no change
                break;
            case OccupyStat.None:
                if(progress < 0.999 && progress > -0.999)
                {
                    switch (occupied)
                    {
                        case OccupyStat.Blue:
                            progress = Mathf.MoveTowards(progress, 1, Time.deltaTime / occupyTime);
                            break;
                        case OccupyStat.Red:
                            progress = Mathf.MoveTowards(progress, -1, Time.deltaTime / occupyTime);
                            break;
                        case OccupyStat.None:
                            progress = Mathf.MoveTowards(progress, 0, Time.deltaTime / occupyTime);
                            break;
                    }
                }
                break;
        }
        switch (occupied)
        {
            case OccupyStat.Blue:
                if (progress <= 0)
                {
                    occupied = OccupyStat.None;
                    centerBeam.sharedMaterial = neutralMat;
                    centerBall.sharedMaterial = neutralMat;
                    mark.mainColor = new Color(1, 1, 1, 1);
                }
                break;
            case OccupyStat.Red:
                if (progress >= 0)
                {
                    occupied = OccupyStat.None;
                    centerBeam.sharedMaterial = neutralMat;
                    centerBall.sharedMaterial = neutralMat;
                    mark.mainColor = new Color(1, 1, 1, 1);
                }
                break;
            case OccupyStat.None:
                if(progress > 0.999)
                {
                    occupied = OccupyStat.Blue;
                    centerBeam.sharedMaterial = blueMat;
                    centerBall.sharedMaterial = blueMat;
                    mark.mainColor = new Color(0, 0.5f, 1, 1);
                }
                else if(progress < -0.999)
                {
                    occupied = OccupyStat.Red;
                    centerBeam.sharedMaterial = redMat;
                    centerBall.sharedMaterial = redMat;
                    mark.mainColor = new Color(1, 0, 0, 1);
                }
                break;
        }

        if(progress > 0)
        {
            beamRed.localScale = new Vector3(1, 0, 1);
            beamBlue.localScale = new Vector3(1, progress * beamNeutral.localScale.y, 1);
        }
        else
        {
            beamRed.localScale = new Vector3(1, -progress * beamNeutral.localScale.y, 1);
            beamBlue.localScale = new Vector3(1, 0, 1);
        }
    }

    IEnumerator CheckOccupy()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if((transform.position - Camera.main.transform.position).sqrMagnitude < 6 * radius * radius)
            {
                projector.enabled = true;
            }
            else
            {
                projector.enabled = false;
            }
            occupy = GetOccupy();
        }
    }

    public OccupyStat GetOccupy()
    {//who is inside
        var hasred = false;
        var hasblue = false;
        foreach(var tc in Singleton<TankManager>.instance.tanks)
        {
            if (!tc.isDestroyed && IsInRange(tc.transform.position))
            {
                var team = tc.GetComponent<TeamMark>();
                switch (team.team)
                {
                    case -1:
                        hasred = true;
                        if (hasblue)
                        {
                            return OccupyStat.Conflict;
                        }
                        break;
                    case 1:
                        hasblue = true;
                        if (hasred)
                        {
                            return OccupyStat.Conflict;
                        }
                        break;
                }
            }
        }
        if (hasblue)
        {
            return OccupyStat.Blue;
        }
        if (hasred)
        {
            return OccupyStat.Red;
        }
        return OccupyStat.None;
    }

    public bool IsInRange(Vector3 point)
    {
        return (point - transform.position).magnitude < radius;
    }

    public enum OccupyStat
    {
        Blue,
        Red,
        Conflict,
        None,
    }
}
