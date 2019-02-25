using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticlePair[] particlePairs;
    private Dictionary<string, GameObject> particlePresets = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Singleton<ParticleManager>.Register(this);
        foreach(var particlePair in particlePairs)
        {
            if (particlePair.key != "" && !particlePresets.ContainsKey(particlePair.key))
            {
                particlePresets.Add(particlePair.key, particlePair.particleObject);
            }
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Emit(string key, Vector3 pos, Quaternion rot)
    {
        if (particlePresets.ContainsKey(key))
        {
            var p = Instantiate(particlePresets[key]);
            p.transform.position = pos;
            p.transform.rotation = rot;
        }
    }

    public void Emit(string key, Transform pos, Quaternion rot) { Emit(key, pos.position, rot); }
    public void Emit(string key, Vector3 pos, Transform rot) { Emit(key, pos, rot.rotation); }
    public void Emit(string key, Transform pos_rot) { Emit(key, pos_rot.position, pos_rot.rotation); }

    public void Emit(string key, Vector3 pos, Quaternion rot, float delay)
    {
        Singleton<TimeManager>.instance.DelayFunc(() => Emit(key, pos, rot), delay);
    }


    [Serializable]
    public class ParticlePair
    {
        public string key;
        public GameObject particleObject;
    }
}
