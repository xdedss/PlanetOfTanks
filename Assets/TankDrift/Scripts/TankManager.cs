using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{

    public List<TankControl> tanks = new List<TankControl>();

    public void OnOccupy(PointControl pc, int team)
    {
        foreach(var tc in tanks)
        {
            var teamMark = tc.GetComponent<TeamMark>();
            if(team == teamMark.team)
            {
                //inform 
            }
        }
    }

    private void Awake()
    {
        Singleton<TankManager>.Register(this);
    }

    public void Register(TankControl tc)
    {
        tanks.Add(tc);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
