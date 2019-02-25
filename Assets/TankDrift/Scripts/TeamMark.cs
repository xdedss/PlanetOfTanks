using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMark : MonoBehaviour
{

    public int team;//red-1  blue1

    public MeshRenderer[] marks;
    public MeshRenderer[] mapMarks;
    public Material redMat;
    public Material blueMat;
    public Material redMapMat;
    public Material blueMapMat;
    public Material destroyedMapMat;

    public void DisableMapMark()
    {
        team = 0;
    }

    void Start()
    {

    }

    void Update()
    {
        if (team == 1)
        {
            foreach (var mark in marks)
            {
                mark.sharedMaterial = blueMat;
            }
            foreach (var mark in mapMarks)
            {
                mark.sharedMaterial = blueMapMat;
            }
        }
        else if (team == -1)
        {
            foreach (var mark in marks)
            {
                mark.sharedMaterial = redMat;
            }
            foreach (var mark in mapMarks)
            {
                mark.sharedMaterial = redMapMat;
            }
        }
        else if(team == 0)
        {
            foreach (var mark in mapMarks)
            {
                mark.sharedMaterial = destroyedMapMat;
            }
        }
    }
}
