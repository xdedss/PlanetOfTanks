using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyOfTank : MonoBehaviour
{
    public static GameObject CopyTank(GameObject tank, Vector3 position, Quaternion rotation, string name, bool ai, int team)
    {
        var t = Instantiate(tank);
        t.SetActive(true);
        t.transform.position = position;
        t.transform.rotation = rotation;
        t.name = name;
        if (ai)
        {
            t.GetComponent<TankInputAI>().enabled = true;
        }
        else
        {
            t.GetComponent<TankInputPlayer>().enabled = true;
        }
        t.GetComponent<TeamMark>().team = team;
        return t;
    }

    public GameObject target;
    public bool ai;
    public int team;

    void Start()
    {
        CopyTank(target, transform.position, transform.rotation, gameObject.name, ai, team);
        Destroy(gameObject);
    }

    void Update()
    {

    }
}
