using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCraneControl : MonoBehaviour
{
    public ParticleSystem[] particleSystems;
    public float rate = 100;
    public float rateIdle = 5;

    public Material decoBlue;
    public Material decoRed;
    public MeshRenderer decomr;

    public void setTeam(int team)
    {
        Debug.Log(team);
        if(team == 1)
        {
            decomr.materials[2].CopyPropertiesFromMaterial(decoBlue);
        }
        else if(team == -1)
        {
            decomr.materials[2].CopyPropertiesFromMaterial(decoRed);
        }
    }

    public void StartParticleIdle()
    {
        foreach (var particleSystem in particleSystems)
        {
            var emmmm = particleSystem.emission;
            emmmm.rateOverTime = rateIdle;
        }
    }

    public void StartParticle()
    {
        foreach(var particleSystem in particleSystems)
        {
            var emmmm = particleSystem.emission;
            emmmm.rateOverTime = rate;
        }
    }
    public void EndParticle()
    {
        foreach (var particleSystem in particleSystems)
        {
            var emmmm = particleSystem.emission;
            emmmm.rateOverTime = 0;
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
